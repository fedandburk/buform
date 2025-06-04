using Buform.Extensions;

namespace Buform;

[Preserve(AllMembers = true)]
internal sealed class FormGroupRegistry
{
    private enum RegistrationType
    {
        Class,
        Nib
    }

    private enum HolderType
    {
        Header,
        Footer
    }

    private sealed class Holder
    {
        public Type ViewType { get; }
        public RegistrationType RegistrationType { get; }

        public Holder(Type viewType, RegistrationType registrationType)
        {
            ArgumentNullException.ThrowIfNull(viewType);

            ViewType = viewType;
            RegistrationType = registrationType;
        }
    }

    private readonly IDictionary<(Type, HolderType), Holder> _holders;

    private readonly Dictionary<Type, Type> _handlers;

    public FormGroupRegistry()
    {
        _holders = new Dictionary<(Type, HolderType), Holder>();
        _handlers = new Dictionary<Type, Type>();
    }

    private bool TryGetHolder(Type groupType, HolderType holderType, out Holder? holder)
    {
        if (_holders.TryGetValue((groupType, holderType), out holder))
        {
            return true;
        }

        var interfaceTypes = groupType.GetInterfacesTopDown();

        foreach (var interfaceType in interfaceTypes)
        {
            if (_holders.TryGetValue((interfaceType, holderType), out holder))
            {
                return true;
            }
        }

        return false;
    }

    public void RegisterGroupHandler<TGroup, THandler>()
        where TGroup : class, IFormGroup
        where THandler : FormGroupHandler<TGroup>
    {
        _handlers[typeof(TGroup)] = typeof(THandler);
    }

    public IFormGroupHandler GetGroupHandler(IFormGroup group)
    {
        if (_handlers.TryGetValue(group.GetType(), out var type))
        {
            return (IFormGroupHandler)Activator.CreateInstance(type)!;
        }

        var interfaceTypes = group.GetType().GetInterfacesTopDown();

        foreach (var interfaceType in interfaceTypes)
        {
            if (_handlers.TryGetValue(interfaceType, out var handlerType))
            {
                return (IFormGroupHandler)Activator.CreateInstance(handlerType)!;
            }
        }

        return new FormGroupHandler<IFormGroup>();
    }

    public void RegisterGroupHeaderClass<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooter<TGroup>
    {
        _holders[(typeof(TGroup), HolderType.Header)] = new Holder(
            typeof(TGroupView),
            RegistrationType.Class
        );

        var t = typeof(TGroupView).ToString();
    }

    public void RegisterGroupFooterClass<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooter<TGroup>
    {
        _holders[(typeof(TGroup), HolderType.Footer)] = new Holder(
            typeof(TGroupView),
            RegistrationType.Class
        );
    }

    public void RegisterGroupHeaderNib<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooter<TGroup>
    {
        _holders[(typeof(TGroup), HolderType.Header)] = new Holder(
            typeof(TGroupView),
            RegistrationType.Nib
        );
    }

    public void RegisterGroupFooterNib<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooter<TGroup>
    {
        _holders[(typeof(TGroup), HolderType.Footer)] = new Holder(
            typeof(TGroupView),
            RegistrationType.Nib
        );
    }

    public void Register(UITableView tableView)
    {
        ArgumentNullException.ThrowIfNull(tableView);

        foreach (var holder in _holders.Values)
        {
            switch (holder.RegistrationType)
            {
                case RegistrationType.Class:
                    tableView.RegisterClassForHeaderFooterViewReuse(
                        holder.ViewType,
                        holder.ViewType.Name
                    );
                    break;
                case RegistrationType.Nib:
                    tableView.RegisterNibForHeaderFooterViewReuse(
                        UINib.FromName(holder.ViewType.Name, NSBundle.MainBundle),
                        holder.ViewType.Name
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public bool TryGetHeaderReuseIdentifier(Type groupType, out string? reuseIdentifier)
    {
        ArgumentNullException.ThrowIfNull(groupType);

        var result = TryGetHolder(groupType, HolderType.Header, out var holder);

        if (result)
        {
            reuseIdentifier = holder!.ViewType.Name;

            return true;
        }

        reuseIdentifier = null;

        return false;
    }

    public bool TryGetFooterReuseIdentifier(Type groupType, out string? reuseIdentifier)
    {
        ArgumentNullException.ThrowIfNull(groupType);

        var result = TryGetHolder(groupType, HolderType.Footer, out var holder);

        if (result)
        {
            reuseIdentifier = holder!.ViewType.Name;

            return true;
        }

        reuseIdentifier = null;

        return false;
    }
}
