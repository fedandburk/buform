namespace Buform;

internal sealed class FormItemRegistry
{
    private enum RegistrationType
    {
        Class,
        Nib,
    }

    private sealed class Holder
    {
        public Type CellType { get; }
        public RegistrationType RegistrationType { get; }

        public Holder(Type cellType, RegistrationType registrationType)
        {
            ArgumentNullException.ThrowIfNull(cellType);

            CellType = cellType;
            RegistrationType = registrationType;
        }
    }

    private readonly IDictionary<Type, Holder> _holders;

    public FormItemRegistry()
    {
        _holders = new Dictionary<Type, Holder>();
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

    public void RegisterItem<TItem, TItemView>()
        where TItem : class, IFormItem
        where TItemView : FormCell<TItem>
    {
        _holders[typeof(TItem)] = new Holder(typeof(TItemView), RegistrationType.Class);
    }

    public void RegisterItemNib<TItem, TItemView>()
        where TItem : class, IFormItem
        where TItemView : FormCell<TItem>
    {
        _holders[typeof(TItem)] = new Holder(typeof(TItemView), RegistrationType.Nib);
    }

    public void Register(UITableView tableView)
    {
        ArgumentNullException.ThrowIfNull(tableView);

        foreach (var holder in _holders.Values)
        {
            switch (holder.RegistrationType)
            {
                case RegistrationType.Class:
                    tableView.RegisterClassForCellReuse(holder.CellType, holder.CellType.Name);
                    break;
                case RegistrationType.Nib:
                    tableView.RegisterNibForCellReuse(
                        UINib.FromName(holder.CellType.Name, NSBundle.MainBundle),
                        holder.CellType.Name
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public bool TryGetReuseIdentifier(Type itemType, out string? reuseIdentifier)
    {
        ArgumentNullException.ThrowIfNull(itemType);

        var result = TryGetHolder(itemType, out var holder);

        if (result)
        {
            reuseIdentifier = holder!.CellType.Name;

            return true;
        }

        reuseIdentifier = null;

        return false;
    }
}
