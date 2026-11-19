namespace Buform.Example.Droid;

[Activity(Label = "New Connection")]
public sealed class CreateConnectionActivity : FormActivity
{
    private readonly CreateConnectionViewModel _viewModel;

    protected override Form Form => _viewModel.Form;

    public CreateConnectionActivity()
    {
        _viewModel = new CreateConnectionViewModel();
    }
}
