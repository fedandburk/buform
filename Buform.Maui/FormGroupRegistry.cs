using Microsoft.Maui.Controls.Internals;

namespace Buform;

[Preserve(AllMembers = true)]
internal sealed class FormGroupRegistry
{
    private enum HolderType
    {
        Header,
        Footer
    }

    private readonly IDictionary<(Type, HolderType), Type> _groups;

    public FormGroupRegistry()
    {
        _groups = new Dictionary<(Type, HolderType), Type>();
    }

    private bool TryGetViewType(Type groupType, HolderType holderType, out Type? viewType)
    {
        if (_groups.TryGetValue((groupType, holderType), out viewType))
        {
            return true;
        }

        var interfaceTypes = groupType
            .GetInterfaces()
            .Except(groupType.GetInterfaces().SelectMany(item => item.GetInterfaces()));

        foreach (var interfaceType in interfaceTypes)
        {
            if (_groups.TryGetValue((interfaceType, holderType), out viewType))
            {
                return true;
            }
        }

        return false;
    }

    public void RegisterGroupHeader<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooterView<TGroup>
    {
        _groups[(typeof(TGroup), HolderType.Header)] = typeof(TGroupView);
    }

    public void RegisterGroupFooter<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooterView<TGroup>
    {
        _groups[(typeof(TGroup), HolderType.Footer)] = typeof(TGroupView);
    }

    public bool TryGetHeaderViewType(Type groupType, out Type? viewType)
    {
        ArgumentNullException.ThrowIfNull(groupType);

        return TryGetViewType(groupType, HolderType.Header, out viewType);
    }

    public bool TryGetFooterViewType(Type groupType, out Type? viewType)
    {
        ArgumentNullException.ThrowIfNull(groupType);

        return TryGetViewType(groupType, HolderType.Footer, out viewType);
    }
}
