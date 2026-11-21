using System.Diagnostics.CodeAnalysis;

namespace Buform;

internal sealed class MauiFormItemRegistry
{
    private readonly Dictionary<Type, Holder> _holders;

    public MauiFormItemRegistry()
    {
        _holders = [];
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

    private bool TryGetHolder(Type itemType, out Holder? holder)
    {
        foreach (var registrationType in itemType.GetRegistrationTypes())
        {
            if (_holders.TryGetValue(registrationType, out holder))
            {
                return true;
            }
        }

        holder = null;

        return false;
    }

    private sealed class Holder
    {
        public Holder(
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                Type viewType
        )
        {
            ViewType = viewType;
        }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public Type ViewType { get; }
    }
}
