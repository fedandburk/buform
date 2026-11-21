namespace Buform.Example.Droid;

[Activity(Label = "New Connection")]
public sealed class CreateConnectionActivity : FormActivity
{
    private readonly CreateConnectionViewModel _viewModel;

    public CreateConnectionActivity()
    {
        _viewModel = new CreateConnectionViewModel();
    }

    protected override Form Form => _viewModel.Form;
}
