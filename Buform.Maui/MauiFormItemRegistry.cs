using System.Diagnostics.CodeAnalysis;

namespace Buform;

internal sealed class MauiFormItemRegistry
{
    private sealed class Holder
    {
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public Type ViewType { get; }

        public Holder(
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                Type viewType
        )
        {
            ViewType = viewType;
        }
    }

    private readonly Dictionary<Type, Holder> _holders;

    public MauiFormItemRegistry()
    {
        _holders = [];
    }

    private bool TryGetHolder(Type itemType, out Holder? holder)
    {
        if (_holders.TryGetValue(itemType, out holder))
        {
            return true;
        }

        foreach (var interfaceType in itemType.GetInterfacesTopDown())
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
        _holders[typeof(TItem)] = new Holder(typeof(TItemView));
    }

    public bool TryGetCellViewType(
        Type itemType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            out Type? viewType
    )
    {
        ArgumentNullException.ThrowIfNull(itemType);

        if (TryGetHolder(itemType, out var holder))
        {
            viewType = holder!.ViewType;

            return true;
        }

        viewType = null;

        return false;
    }
}
