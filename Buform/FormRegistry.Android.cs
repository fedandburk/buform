using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Buform.Extensions;

namespace Buform;

internal sealed class FormRegistry
{
    private sealed class Holder
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
        public Type ViewHolderType { get; }
        public int ResourceId { get; }

        public Holder(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                Type viewHolderType,
            int resourceId
        )
        {
            ArgumentNullException.ThrowIfNull(viewHolderType);

            ViewHolderType = viewHolderType;
            ResourceId = resourceId;
        }
    }

    private readonly IDictionary<(Type, FormRowType), Holder> _holders;

    public FormRegistry()
    {
        _holders = new Dictionary<(Type, FormRowType), Holder>();
    }

    public void RegisterItem<
        TData,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewHolder
    >(int resourceId)
        where TData : class, INotifyPropertyChanged, IDisposable
        where TViewHolder : FormViewHolder
    {
        _holders[(typeof(TData), FormRowType.Item)] = new Holder(typeof(TViewHolder), resourceId);
    }

    public void RegisterGroupHeader<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewHolder
    >(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormViewHolder
    {
        _holders[(typeof(TGroup), FormRowType.Header)] = new Holder(
            typeof(TViewHolder),
            resourceId
        );
    }

    public void RegisterGroupFooter<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewHolder
    >(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormViewHolder
    {
        _holders[(typeof(TGroup), FormRowType.Footer)] = new Holder(
            typeof(TViewHolder),
            resourceId
        );
    }

    private bool TryGetHolder(Type dataType, FormRowType viewHolderType, out Holder? holder)
    {
        if (_holders.TryGetValue((dataType, viewHolderType), out holder))
        {
            return true;
        }

        var interfaceTypes = dataType.GetInterfacesTopDown();

        foreach (var interfaceType in interfaceTypes)
        {
            if (_holders.TryGetValue((interfaceType, viewHolderType), out holder))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryGetViewType(Type dataType, FormRowType viewHolderType, out int? viewType)
    {
        ArgumentNullException.ThrowIfNull(dataType);

        var result = TryGetHolder(dataType, viewHolderType, out var holder);

        if (!result)
        {
            viewType = default;

            return false;
        }

        var index = _holders.Values.ToList().IndexOf(holder!);

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
        return TryGetViewType(dataType, FormRowType.Item, out viewType);
    }

    public bool TryGetGroupHeaderViewType(Type dataType, out int? viewType)
    {
        return TryGetViewType(dataType, FormRowType.Header, out viewType);
    }

    public bool TryGetGroupFooterViewType(Type dataType, out int? viewType)
    {
        return TryGetViewType(dataType, FormRowType.Footer, out viewType);
    }

    public bool TryGetViewHolderAndResourceId(
        int viewType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
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
