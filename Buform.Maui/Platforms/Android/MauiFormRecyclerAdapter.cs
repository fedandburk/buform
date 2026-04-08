using System.Collections.Specialized;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Fedandburk.Common.Extensions;
using Object = Java.Lang.Object;

namespace Buform;

[Preserve(AllMembers = true)]
internal sealed class MauiFormRecyclerAdapter : RecyclerView.Adapter
{
    private readonly Context _context;
    private readonly IMauiContext _mauiContext;

    private Form? _form;
    private IEnumerable<IFormItem>? _items;
    private IDisposable? _formSubscription;
    private IDisposable? _itemsSubscription;

    public MauiFormRecyclerAdapter(Context context, IMauiContext mauiContext)
    {
        _context = context;
        _mauiContext = mauiContext;
    }

    public Form? Form
    {
        get => _form;
        set
        {
            if (ReferenceEquals(_form, value))
            {
                return;
            }

            ResetState();
            _form = value;

            if (_form == null)
            {
                NotifyDataSetChanged();
                return;
            }

            _formSubscription = _form.WeakSubscribe(OnFormChanged);
            _items = _form.ObservableFlatten();

            if (_items is INotifyCollectionChanged notifyCollectionChanged)
            {
                _itemsSubscription = notifyCollectionChanged.WeakSubscribe(OnItemsChanged);
            }

            NotifyDataSetChanged();
        }
    }

    public override int ItemCount => GetItemsCount();

    public override long GetItemId(int position) => position;

    public override int GetItemViewType(int position)
    {
        if (!TryGetData(position, out _, out var holderType) || holderType == null)
        {
            throw new FormViewNotFoundException(position);
        }

        return (int)holderType.Value;
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var container = CreateContainer(parent.Context!);

        return (FormViewHolderType)viewType switch
        {
            FormViewHolderType.Item => new MauiFormItemViewHolder(container, _mauiContext),
            FormViewHolderType.Header
                => new MauiFormHeaderFooterViewHolder(container, _mauiContext),
            FormViewHolderType.Footer
                => new MauiFormHeaderFooterViewHolder(container, _mauiContext),
            _ => throw new ArgumentOutOfRangeException(nameof(viewType), viewType, null),
        };
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        if (
            !TryGetData(position, out var data, out var holderType)
            || data == null
            || holderType == null
        )
        {
            throw new FormViewNotFoundException(position);
        }

        switch (holderType.Value)
        {
            case FormViewHolderType.Item:
                BindItemHolder(holder, data, position);
                break;

            case FormViewHolderType.Header:
            case FormViewHolderType.Footer:
                BindHeaderFooterHolder(holder, data, holderType.Value, position);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void OnViewRecycled(Object holder)
    {
        UnbindHolder(holder);
        base.OnViewRecycled(holder);
    }

    public override bool OnFailedToRecycleView(Object holder)
    {
        UnbindHolder(holder);
        return base.OnFailedToRecycleView(holder);
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            base.Dispose(disposing);
            return;
        }

        ResetState();
        _form = null;

        base.Dispose(disposing);
    }

    private void OnFormChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (_form == null)
        {
            return;
        }

        NotifyDataSetChanged();
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (_items == null)
        {
            return;
        }

        NotifyDataSetChanged();
    }

    private void ResetState()
    {
        _formSubscription?.Dispose();
        _itemsSubscription?.Dispose();

        if (_items is IDisposable disposable)
        {
            disposable.Dispose();
        }

        _formSubscription = null;
        _itemsSubscription = null;
        _items = null;
    }

    private void BindItemHolder(RecyclerView.ViewHolder holder, object data, int position)
    {
        if (holder is not MauiFormItemViewHolder itemHolder)
        {
            throw new InvalidOperationException(
                $"Expected {nameof(MauiFormItemViewHolder)} for position {position}."
            );
        }

        itemHolder.Unbind();
        itemHolder.Bind(_context, data);
    }

    private void BindHeaderFooterHolder(
        RecyclerView.ViewHolder holder,
        object data,
        FormViewHolderType holderType,
        int position
    )
    {
        if (holder is not MauiFormHeaderFooterViewHolder headerFooterHolder)
        {
            throw new InvalidOperationException(
                $"Expected {nameof(MauiFormHeaderFooterViewHolder)} for {holderType} at position {position}."
            );
        }

        headerFooterHolder.Unbind();
        headerFooterHolder.Bind(_context, data, holderType);
    }

    private static void UnbindHolder(Object holder)
    {
        if (holder is MauiFormItemViewHolder itemHolder)
        {
            itemHolder.Unbind();
        }
        else if (holder is MauiFormHeaderFooterViewHolder headerFooterHolder)
        {
            headerFooterHolder.Unbind();
        }
    }

    private int GetItemsCount()
    {
        if (_form == null || _items == null)
        {
            return 0;
        }

        var rowsCount = _items switch
        {
            ICollection<IFormItem> collection => collection.Count,
            IReadOnlyCollection<IFormItem> readOnlyCollection => readOnlyCollection.Count,
            _ => _items.Count()
        };

        var chromeCount = 0;

        foreach (var section in _form)
        {
            if (HasHeader(section))
            {
                chromeCount++;
            }

            if (HasFooter(section))
            {
                chromeCount++;
            }
        }

        return rowsCount + chromeCount;
    }

    private bool TryGetData(int position, out object? data, out FormViewHolderType? viewHolderType)
    {
        if (_form == null)
        {
            data = null;
            viewHolderType = null;
            return false;
        }

        var index = 0;

        foreach (var section in _form)
        {
            var hasHeader = HasHeader(section);
            var hasFooter = HasFooter(section);

            if (hasHeader)
            {
                if (index == position)
                {
                    data = section;
                    viewHolderType = FormViewHolderType.Header;
                    return true;
                }

                index++;
            }

            foreach (var item in section)
            {
                if (index == position)
                {
                    data = item;
                    viewHolderType = FormViewHolderType.Item;
                    return true;
                }

                index++;
            }

            if (hasFooter)
            {
                if (index == position)
                {
                    data = section;
                    viewHolderType = FormViewHolderType.Footer;
                    return true;
                }

                index++;
            }
        }

        data = null;
        viewHolderType = null;
        return false;
    }

    private static FrameLayout CreateContainer(Context context)
    {
        var container = new FrameLayout(context);
        container.LayoutParameters = new RecyclerView.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent
        );

        return container;
    }

    private static bool HasHeader(object section)
    {
        var sectionType = section.GetType();

        return !string.IsNullOrWhiteSpace(FormReflectionHelper.GetHeaderLabel(section))
            || MauiFormPlatform.TryGetHeaderViewType(sectionType, out _);
    }

    private static bool HasFooter(object section)
    {
        var sectionType = section.GetType();

        return !string.IsNullOrWhiteSpace(FormReflectionHelper.GetFooterLabel(section))
            || MauiFormPlatform.TryGetFooterViewType(sectionType, out _);
    }
}
