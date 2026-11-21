namespace Buform;

public abstract class FormItemView : ContentView;

public abstract class FormItemView<TItem> : FormItemView
    where TItem : class, IFormItem
{
    protected TItem? Item => BindingContext as TItem;
}
