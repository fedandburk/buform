namespace Buform;

public abstract class FormHeaderFooterView : ContentView { }

public abstract class FormHeaderFooterView<TGroup> : FormHeaderFooterView
    where TGroup : class, IFormGroup
{
    protected TGroup? Group => BindingContext as TGroup;
}
