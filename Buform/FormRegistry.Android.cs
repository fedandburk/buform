using System.Diagnostics.CodeAnalysis;

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
    private readonly IDictionary<Type, Func<IFormGroupHandler>> _handlers;

    public FormRegistry()
    {
        _holders = new Dictionary<(Type, FormRowType), Holder>();
        _handlers = new Dictionary<Type, Func<IFormGroupHandler>>();
    }

    public void RegisterItem<
        TItem,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewHolder
    >(int resourceId)
        where TItem : class, IFormItem
        where TViewHolder : FormViewHolder<TItem>
    {
        _holders[(typeof(TItem), FormRowType.Item)] = new Holder(typeof(TViewHolder), resourceId);
    }

    public void RegisterGroupHeader<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewHolder
    >(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormHeaderFooterViewHolder<TGroup>
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
        where TViewHolder : FormHeaderFooterViewHolder<TGroup>
    {
        _holders[(typeof(TGroup), FormRowType.Footer)] = new Holder(
            typeof(TViewHolder),
            resourceId
        );
    }

    public void RegisterGroupHandler<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            THandler
    >()
        where TGroup : class, IFormGroup
        where THandler : FormGroupHandler<TGroup>
    {
        _handlers[typeof(TGroup)] = Activator.CreateInstance<THandler>;
    }

    public IFormGroupHandler GetGroupHandler(IFormGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        foreach (var lookupType in group.GetType().GetRegistrationTypes())
        {
            if (_handlers.TryGetValue(lookupType, out var createHandler))
            {
                return createHandler();
            }
        }

        return new FormGroupHandler<IFormGroup>();
    }

    private bool TryGetHolder(Type dataType, FormRowType viewHolderType, out Holder? holder)
    {
        foreach (var lookupType in dataType.GetRegistrationTypes())
        {
            if (_holders.TryGetValue((lookupType, viewHolderType), out holder))
            {
                return true;
            }
        }

        holder = null;

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

    public bool TryGetItemViewType(Type itemType, out int? viewType)
    {
        return TryGetViewType(itemType, FormRowType.Item, out viewType);
    }

    public bool TryGetGroupHeaderViewType(Type groupType, out int? viewType)
    {
        return TryGetViewType(groupType, FormRowType.Header, out viewType);
    }

    public bool TryGetGroupFooterViewType(Type groupType, out int? viewType)
    {
        return TryGetViewType(groupType, FormRowType.Footer, out viewType);
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
