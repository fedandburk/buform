using System.Diagnostics.CodeAnalysis;

namespace Buform;

internal sealed class MauiFormGroupRegistry
{
    private enum HolderType
    {
        Header,
        Footer,
    }

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

    private readonly IDictionary<(Type, HolderType), Holder> _groups;

    public MauiFormGroupRegistry()
    {
        _groups = new Dictionary<(Type, HolderType), Holder>();
    }

    private bool TryGetViewType(
        Type groupType,
        HolderType holderType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            out Type? viewType
    )
    {
        if (_groups.TryGetValue((groupType, holderType), out var holder))
        {
            viewType = holder.ViewType;

            return true;
        }

        var interfaceTypes = groupType.GetInterfacesTopDown();

        foreach (var interfaceType in interfaceTypes)
        {
            if (_groups.TryGetValue((interfaceType, holderType), out holder))
            {
                viewType = holder.ViewType;

                return true;
            }
        }

        viewType = null;

        return false;
    }

    public void RegisterGroupHeader<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            TGroupView
    >()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooterView<TGroup>
    {
        _groups[(typeof(TGroup), HolderType.Header)] = new Holder(typeof(TGroupView));
    }

    public void RegisterGroupFooter<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            TGroupView
    >()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooterView<TGroup>
    {
        _groups[(typeof(TGroup), HolderType.Footer)] = new Holder(typeof(TGroupView));
    }

    public bool TryGetHeaderViewType(
        Type groupType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            out Type? viewType
    )
    {
        ArgumentNullException.ThrowIfNull(groupType);

        return TryGetViewType(groupType, HolderType.Header, out viewType);
    }

    public bool TryGetFooterViewType(
        Type groupType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            out Type? viewType
    )
    {
        ArgumentNullException.ThrowIfNull(groupType);

        return TryGetViewType(groupType, HolderType.Footer, out viewType);
    }
}
