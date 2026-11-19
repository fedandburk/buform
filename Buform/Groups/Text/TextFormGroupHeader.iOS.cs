using ObjCRuntime;

namespace Buform;

public sealed class TextFormGroupHeader : FormHeaderFooter<TextFormGroup>
{
    public TextFormGroupHeader()
    {
        /* Required constructor */
    }

    public TextFormGroupHeader(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    private void UpdateLabel()
    {
        var configuration = DefaultContentConfiguration;
        configuration.Text = Group?.HeaderLabel;

        ContentConfiguration = configuration;
    }

    protected override void OnGroupSet()
    {
        UpdateLabel();
    }

    protected override void OnGroupPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Group.HeaderLabel):
                UpdateLabel();
                break;
        }
    }
}
