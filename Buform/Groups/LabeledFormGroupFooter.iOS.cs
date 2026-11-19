using ObjCRuntime;

namespace Buform;

public class LabeledFormGroupFooter : FormHeaderFooter<ILabeledFormGroup>
{
    public LabeledFormGroupFooter()
    {
        /* Required constructor */
    }

    public LabeledFormGroupFooter(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected virtual void UpdateLabel()
    {
        var configuration = DefaultContentConfiguration;
        configuration.Text = Group?.FooterLabel;

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
            case nameof(ILabeledFormGroup.FooterLabel):
            case null:
                UpdateLabel();
                break;
        }
    }
}
