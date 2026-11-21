using ObjCRuntime;

namespace Buform;

public class LabeledFormGroupHeader : FormHeaderFooter<ILabeledFormGroup>
{
    public LabeledFormGroupHeader()
    {
        /* Required constructor */
    }

    public LabeledFormGroupHeader(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected virtual void UpdateLabel()
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
            case nameof(ILabeledFormGroup.HeaderLabel):
            case null:
                UpdateLabel();
                break;
        }
    }
}
