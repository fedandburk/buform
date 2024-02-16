using System.Text.RegularExpressions;
using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class ListFormGroupHeader : FormHeaderFooter<IListFormGroup>
{
    public ListFormGroupHeader()
    {
        /* Required constructor */
    }

    public ListFormGroupHeader(NativeHandle handle) : base(handle)
    {
        /* Required constructor */
    }

    private void UpdateLabel()
    {
        var tableView = GetTableView();

        tableView?.BeginUpdates();

        TextLabel.Text = Group?.HeaderLabel;

        tableView?.EndUpdates();
    }

    protected override void OnGroupSet()
    {
        UpdateLabel();
    }

    protected override void OnGroupPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Group.HeaderLabel):
                UpdateLabel();
                break;
        }
    }
}