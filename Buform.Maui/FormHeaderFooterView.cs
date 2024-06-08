using Microsoft.Maui.Controls.Internals;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class FormHeaderFooterView : ContentView { }

[Preserve(AllMembers = true)]
public abstract class FormHeaderFooterView<TGroup> : FormHeaderFooterView
    where TGroup : class, IFormGroup
{
    protected TGroup? Group => BindingContext as TGroup;
}
