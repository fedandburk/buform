using ObjCRuntime;

namespace Buform;

public sealed class ListFormGroupFooter : FormHeaderFooter<IListFormGroup>
{
    public ListFormGroupFooter()
    {
        /* Required constructor */
    }

    public ListFormGroupFooter(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    private void UpdateLabel()
    {
        var configuration = DefaultContentConfiguration;
        configuration.Text = Group?.FooterLabel;

        ContentConfiguration = configuration;

        GetTableView()?.PerformBatchUpdates(null, null);
    }

    protected override void OnGroupSet()
    {
        UpdateLabel();
    }

    protected override void OnGroupPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Group.FooterLabel):
                UpdateLabel();
                break;
        }
    }
}
