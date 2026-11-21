using Android.Views;

namespace Buform.Example.Droid;

[Activity(Label = "New Event")]
public sealed class CreateEventActivity : FormActivity
{
    private const string AddTitle = "Add";

    private readonly CreateEventViewModel _viewModel;

    private IMenuItem? _addItem;

    public CreateEventActivity()
    {
        _viewModel = new CreateEventViewModel();
        _viewModel.CreateCommand.CanExecuteChanged += OnCreateCanExecuteChanged;
    }

    protected override Form Form => _viewModel.Form;

    protected override void OnCreateMenu(IMenu menu)
    {
        _addItem = menu.Add(AddTitle)!;
        _addItem.SetShowAsAction(ShowAsAction.Always);
        _addItem.SetEnabled(_viewModel.CreateCommand.CanExecute(null));
        _addItem.SetOnMenuItemClickListener(
            new MenuItemClickListener(() => _viewModel.CreateCommand.Execute(null))
        );
    }

    protected override void OnDestroy()
    {
        _viewModel.CreateCommand.CanExecuteChanged -= OnCreateCanExecuteChanged;

        base.OnDestroy();
    }

    private void OnCreateCanExecuteChanged(object? sender, EventArgs e)
    {
        _addItem?.SetEnabled(_viewModel.CreateCommand.CanExecute(null));
    }
}
