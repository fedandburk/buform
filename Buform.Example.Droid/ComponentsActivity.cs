using Android.Views;

namespace Buform.Example.Droid;

[Activity(Label = "Components")]
public sealed class ComponentsActivity : FormActivity
{
    private const string ReadOnlyTitle = "Read-only";

    private readonly ComponentsViewModel _viewModel;

    public ComponentsActivity()
    {
        _viewModel = new ComponentsViewModel();
    }

    protected override Form Form => _viewModel.Form;

    protected override void OnCreateMenu(IMenu menu)
    {
        var readOnlyItem = menu.Add(ReadOnlyTitle)!;

        readOnlyItem.SetCheckable(true);
        readOnlyItem.SetShowAsAction(ShowAsAction.Never);
        readOnlyItem.SetOnMenuItemClickListener(
            new MenuItemClickListener(() =>
            {
                _viewModel.ToggleReadOnlyModeCommand.Execute(null);

                readOnlyItem.SetChecked(_viewModel.Form.IsReadOnly);
            })
        );
    }
}
