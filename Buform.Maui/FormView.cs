using Microsoft.Maui.Controls.Internals;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class FormView : View
{
    public static readonly BindableProperty FormProperty = BindableProperty.Create(
        nameof(Form),
        typeof(Form),
        typeof(FormView)
    );

    public Form? Form
    {
        get => (Form?)GetValue(FormProperty);
        set => SetValue(FormProperty, value);
    }
}
