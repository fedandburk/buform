using System.Collections.Specialized;

namespace Buform;

public class FormTableViewSource : UITableViewDelegate
{
    private readonly FormDataSource _dataSource;
    private readonly Dictionary<IFormGroup, IFormGroupHandler> _groupHandlers;
    private readonly List<IFormGroup> _groups;

    private Dictionary<object, FormIdentifier> _identifiers;
    private bool _isSnapshotApplied;

    protected UITableView TableView { get; }

    public Form? Form
    {
        get;
        set
        {
            if (ReferenceEquals(field, value))
            {
                return;
            }

            if (field != null)
            {
                field.CollectionChanged -= OnGroupsChanged;
            }

            field = value;

            if (field != null)
            {
                field.CollectionChanged += OnGroupsChanged;
            }

            _isSnapshotApplied = false;

            ApplySnapshot();
        }
    }

    public FormTableViewSource(UITableView tableView)
    {
        ArgumentNullException.ThrowIfNull(tableView);

        TableView = tableView;

        _groupHandlers = [];
        _groups = [];
        _identifiers = new Dictionary<object, FormIdentifier>(ReferenceEqualityComparer.Instance);
        _dataSource = new FormDataSource(this, tableView);

        FormPlatform.Register(tableView);

        tableView.Delegate = this;
    }

    private void OnGroupsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ApplySnapshot();
    }

    private void OnGroupItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ApplySnapshot();
    }

    private void ApplySnapshot()
    {
        if (!NSThread.IsMain)
        {
            BeginInvokeOnMainThread(ApplySnapshot);

            return;
        }

        foreach (var group in _groups)
        {
            group.CollectionChanged -= OnGroupItemsChanged;
        }

        _groups.Clear();
        _groups.AddRange(Form ?? Enumerable.Empty<IFormGroup>());

        var identifiers = new Dictionary<object, FormIdentifier>(
            ReferenceEqualityComparer.Instance
        );
        var snapshot = new NSDiffableDataSourceSnapshot<FormIdentifier, FormIdentifier>();

        foreach (var group in _groups)
        {
            group.CollectionChanged += OnGroupItemsChanged;

            var groupIdentifier = GetIdentifier(group, identifiers);
            var itemIdentifiers = group.Select(item => GetIdentifier(item, identifiers)).ToArray();

            snapshot.AppendSections([groupIdentifier]);
            snapshot.AppendItems(itemIdentifiers, groupIdentifier);
        }

        _identifiers = identifiers;

        _dataSource.ApplySnapshot(snapshot, _isSnapshotApplied);

        _isSnapshotApplied = true;
    }

    private FormIdentifier GetIdentifier(
        object value,
        Dictionary<object, FormIdentifier> identifiers
    )
    {
        if (!_identifiers.TryGetValue(value, out var identifier))
        {
            identifier = new FormIdentifier(value);
        }

        identifiers[value] = identifier;

        return identifier;
    }

    private IFormItem GetItem(NSIndexPath indexPath)
    {
        return _groups[(int)indexPath.Section][(int)indexPath.Row];
    }

    private IFormGroup GetGroup(nint section)
    {
        return _groups[(int)section];
    }

    private IFormGroupHandler GetGroupHandler(nint section)
    {
        var group = GetGroup(section);

        if (_groupHandlers.TryGetValue(group, out var groupHandler))
        {
            return groupHandler;
        }

        groupHandler = FormPlatform.GetGroupHandler(group);
        groupHandler.Initialize(group, TableView);

        _groupHandlers[group] = groupHandler;

        return groupHandler;
    }

    private FormCell? FindCell(NSIndexPath indexPath)
    {
        return TableView.CellAt(indexPath) as FormCell;
    }

    protected virtual UITableViewCell GetCell(
        UITableView tableView,
        NSIndexPath indexPath,
        IFormItem item
    )
    {
        ArgumentNullException.ThrowIfNull(tableView);
        ArgumentNullException.ThrowIfNull(indexPath);
        ArgumentNullException.ThrowIfNull(item);

        if (!FormPlatform.TryGetReuseIdentifier(item.GetType(), out var reuseIdentifier))
        {
            throw new FormViewNotFoundException(item.GetType());
        }

        var cell = tableView.DequeueReusableCell(reuseIdentifier!, indexPath);

        if (cell is FormCell formCell)
        {
            GetGroupHandler(indexPath.Section).InitializeCell(formCell, indexPath, item);
        }

        return cell;
    }

    protected virtual UITableViewHeaderFooterView GetHeaderView(
        UITableView tableView,
        IFormGroup group
    )
    {
        ArgumentNullException.ThrowIfNull(tableView);
        ArgumentNullException.ThrowIfNull(group);

        if (!FormPlatform.TryGetHeaderReuseIdentifier(group.GetType(), out var reuseIdentifier))
        {
            throw new FormViewNotFoundException(group.GetType());
        }

        var view = tableView.DequeueReusableHeaderFooterView(reuseIdentifier!);

        if (view == null)
        {
            throw new InvalidOperationException(
                $"The table view has no header or footer view for '{reuseIdentifier}'."
            );
        }

        (view as FormHeaderFooter)?.Initialize(group);

        return view;
    }

    protected virtual UITableViewHeaderFooterView GetFooterView(
        UITableView tableView,
        IFormGroup group
    )
    {
        ArgumentNullException.ThrowIfNull(tableView);
        ArgumentNullException.ThrowIfNull(group);

        if (!FormPlatform.TryGetFooterReuseIdentifier(group.GetType(), out var reuseIdentifier))
        {
            throw new FormViewNotFoundException(group.GetType());
        }

        var view = tableView.DequeueReusableHeaderFooterView(reuseIdentifier!);

        if (view == null)
        {
            throw new InvalidOperationException(
                $"The table view has no header or footer view for '{reuseIdentifier}'."
            );
        }

        (view as FormHeaderFooter)?.Initialize(group);

        return view;
    }

    public override UIView GetViewForHeader(UITableView tableView, nint section)
    {
        return GetHeaderView(tableView, GetGroup(section));
    }

    public override UIView GetViewForFooter(UITableView tableView, nint section)
    {
        return GetFooterView(tableView, GetGroup(section));
    }

    public override NSIndexPath? WillSelectRow(UITableView tableView, NSIndexPath indexPath)
    {
        var item = GetItem(indexPath);

        return GetGroupHandler(indexPath.Section).CanSelectItem(item) ? indexPath : null;
    }

    public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
    {
        var item = GetItem(indexPath);
        var groupHandler = GetGroupHandler(indexPath.Section);

        if (groupHandler.ShouldAutomaticallyDeselectItem(item))
        {
            tableView.DeselectRow(indexPath, true);
        }

        groupHandler.OnItemSelected(item);

        FindCell(indexPath)?.OnSelected();
    }

    public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
    {
        GetGroupHandler(indexPath.Section).OnItemDeselected(GetItem(indexPath));
    }

    public override UITableViewCellEditingStyle EditingStyleForRow(
        UITableView tableView,
        NSIndexPath indexPath
    )
    {
        return GetGroupHandler(indexPath.Section).EditingStyleForItem(GetItem(indexPath));
    }

    public override bool ShouldIndentWhileEditing(UITableView tableView, NSIndexPath indexPath)
    {
        return GetGroupHandler(indexPath.Section).CanEditItem(GetItem(indexPath));
    }

    public override NSIndexPath CustomizeMoveTarget(
        UITableView tableView,
        NSIndexPath sourceIndexPath,
        NSIndexPath proposedIndexPath
    )
    {
        var sourceGroupHandler = GetGroupHandler(sourceIndexPath.Section);
        var proposedGroupHandler = GetGroupHandler(proposedIndexPath.Section);

        if (sourceGroupHandler == proposedGroupHandler)
        {
            return proposedIndexPath;
        }

        var item = GetItem(sourceIndexPath);

        if (
            sourceGroupHandler.CanMoveItemIntoGroup(item, GetGroup(proposedIndexPath.Section))
            && proposedGroupHandler.CanInsertItem(item)
        )
        {
            return proposedIndexPath;
        }

        return sourceIndexPath;
    }

    private bool CanEditRow(NSIndexPath indexPath)
    {
        return GetGroupHandler(indexPath.Section).CanEditItem(GetItem(indexPath));
    }

    private void CommitEditingStyle(UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
    {
        GetGroupHandler(indexPath.Section)
            .CommitEditingStyleForItem(editingStyle, GetItem(indexPath));
    }

    private bool CanMoveRow(NSIndexPath indexPath)
    {
        return GetGroupHandler(indexPath.Section).CanMoveItem(GetItem(indexPath));
    }

    private void MoveRow(NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
    {
        var sourceGroupHandler = GetGroupHandler(sourceIndexPath.Section);
        var destinationGroupHandler = GetGroupHandler(destinationIndexPath.Section);

        var item = GetItem(sourceIndexPath);

        if (sourceGroupHandler == destinationGroupHandler)
        {
            sourceGroupHandler.MoveItem(item, sourceIndexPath.Row, destinationIndexPath.Row);

            return;
        }

        sourceGroupHandler.RemoveItem(item);
        destinationGroupHandler.InsertItem(item, destinationIndexPath.Row);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Form = null;

            foreach (var group in _groups)
            {
                group.CollectionChanged -= OnGroupItemsChanged;
            }

            _groups.Clear();
        }

        base.Dispose(disposing);
    }

    private sealed class FormIdentifier : NSObject
    {
        public object Value { get; }

        public FormIdentifier(object value)
        {
            Value = value;
        }
    }

    private sealed class FormDataSource
        : UITableViewDiffableDataSource<FormIdentifier, FormIdentifier>
    {
        private readonly FormTableViewSource _owner;

        public FormDataSource(FormTableViewSource owner, UITableView tableView)
            : base(tableView, (view, indexPath, _) =>
                owner.GetCell(view, indexPath, owner.GetItem(indexPath))
            )
        {
            _owner = owner;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return _owner.CanEditRow(indexPath);
        }

        public override void CommitEditingStyle(
            UITableView tableView,
            UITableViewCellEditingStyle editingStyle,
            NSIndexPath indexPath
        )
        {
            _owner.CommitEditingStyle(editingStyle, indexPath);
        }

        public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
        {
            return _owner.CanMoveRow(indexPath);
        }

        public override void MoveRow(
            UITableView tableView,
            NSIndexPath sourceIndexPath,
            NSIndexPath destinationIndexPath
        )
        {
            _owner.MoveRow(sourceIndexPath, destinationIndexPath);
        }
    }
}
