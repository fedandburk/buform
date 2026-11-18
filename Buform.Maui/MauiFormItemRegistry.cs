using System.Diagnostics.CodeAnalysis;
using Buform.Extensions;

namespace Buform;

internal sealed class MauiFormItemRegistry
{
    private sealed class Holder
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public Type CellType { get; }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public Type? ExpandedCellType { get; }

        public Holder(
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                Type cellType,
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                Type? expandedCellType
        )
        {
            CellType = cellType ?? throw new ArgumentNullException(nameof(cellType));
            ExpandedCellType = expandedCellType;
        }
    }

    private readonly IDictionary<Type, Holder> _holders;

    public MauiFormItemRegistry()
    {
        _holders = new Dictionary<Type, Holder>();
    }

    private bool TryGetHolder(Type itemType, out Holder? holder)
    {
        if (_holders.TryGetValue(itemType, out holder))
        {
            return true;
        }

        var interfaceTypes = itemType.GetInterfacesTopDown();

        foreach (var interfaceType in interfaceTypes)
        {
            if (_holders.TryGetValue(interfaceType, out holder))
            {
                return true;
            }
        }

        return false;
    }

    public void RegisterItem<
        TItem,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            TItemView
    >()
        where TItem : class, IFormItem
        where TItemView : FormItemView<TItem>
    {
        _holders[typeof(TItem)] = new Holder(typeof(TItemView), null);
    }

    public void RegisterItem<
        TItem,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            TItemView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            TExpandedItemView
    >()
        where TItem : class, IFormItem
        where TItemView : FormItemView<TItem>
        where TExpandedItemView : FormItemView<TItem>
    {
        _holders[typeof(TItem)] = new Holder(typeof(TItemView), typeof(TExpandedItemView));
    }

    public bool TryGetCellViewType(
        Type itemType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            out Type? viewType
    )
    {
        ArgumentNullException.ThrowIfNull(itemType);

        var result = TryGetHolder(itemType, out var holder);

        if (result)
        {
            viewType = holder!.CellType;

            return true;
        }

        viewType = null;

        return false;
    }

    public bool TryGetExpandedCellViewType(
        Type itemType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            out Type? viewType
    )
    {
        ArgumentNullException.ThrowIfNull(itemType);

        var result = TryGetHolder(itemType, out var holder);

        if (result)
        {
            viewType = holder!.ExpandedCellType;

            return viewType != null;
        }

        viewType = null;

        return false;
    }
}
