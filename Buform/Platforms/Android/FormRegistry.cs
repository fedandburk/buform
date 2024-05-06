using System.ComponentModel;
using Android.Runtime;
using Fedandburk.Common.Extensions;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class FormRegistry
{
    private sealed class Holder
    {
        public Type ViewHolderType { get; }
        public int ResourceId { get; }

        public Holder(Type viewHolderType, int resourceId)
        {
            ArgumentNullException.ThrowIfNull(viewHolderType);

            ViewHolderType = viewHolderType;
            ResourceId = resourceId;
        }
    }

    private readonly IDictionary<(Type, FormViewHolderType), Holder> _holders;

    public FormRegistry()
    {
        _holders = new Dictionary<(Type, FormViewHolderType), Holder>();
    }

    public void RegisterItem<TData, TViewHolder>(int resourceId)
        where TData : class, INotifyPropertyChanged, IDisposable
        where TViewHolder : FormViewHolder
    {
        _holders[(typeof(TData), FormViewHolderType.Item)] = new Holder(
            typeof(TViewHolder),
            resourceId
        );
    }

    public void RegisterGroupHeader<TGroup, TViewHolder>(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormViewHolder
    {
        _holders[(typeof(TGroup), FormViewHolderType.Header)] = new Holder(
            typeof(TViewHolder),
            resourceId
        );
    }

    public void RegisterGroupFooter<TGroup, TViewHolder>(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormViewHolder
    {
        _holders[(typeof(TGroup), FormViewHolderType.Footer)] = new Holder(
            typeof(TViewHolder),
            resourceId
        );
    }

    private bool TryGetHolder(Type dataType, FormViewHolderType viewHolderType, out Holder? holder)
    {
        if (_holders.TryGetValue((dataType, viewHolderType), out holder))
        {
            return true;
        }

        var interfaceTypes = dataType
            .GetInterfaces()
            .Except(dataType.GetInterfaces().SelectMany(item => item.GetInterfaces()));

        foreach (var interfaceType in interfaceTypes)
        {
            if (_holders.TryGetValue((interfaceType, viewHolderType), out holder))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryGetViewType(Type dataType, FormViewHolderType viewHolderType, out int? viewType)
    {
        ArgumentNullException.ThrowIfNull(dataType);

        var result = TryGetHolder(dataType, viewHolderType, out var holder);

        if (!result)
        {
            viewType = default;

            return false;
        }

        var index = _holders.Values.IndexOf(holder);

        if (index < 0)
        {
            viewType = default;

            return false;
        }

        viewType = index;

        return true;
    }

    public bool TryGetItemViewType(Type dataType, out int? viewType)
    {
        return TryGetViewType(dataType, FormViewHolderType.Item, out viewType);
    }

    public bool TryGetGroupHeaderViewType(Type dataType, out int? viewType)
    {
        return TryGetViewType(dataType, FormViewHolderType.Header, out viewType);
    }

    public bool TryGetGroupFooterViewType(Type dataType, out int? viewType)
    {
        return TryGetViewType(dataType, FormViewHolderType.Footer, out viewType);
    }

    public bool TryGetViewHolderAndResourceId(
        int viewType,
        out Type? viewHolderType,
        out int? resourceId
    )
    {
        var holder = _holders.Values.ElementAtOrDefault(viewType);

        if (holder == null)
        {
            viewHolderType = null;
            resourceId = null;

            return false;
        }

        viewHolderType = holder.ViewHolderType;
        resourceId = holder.ResourceId;

        return true;
    }
}
