using Microsoft.Maui.Controls.Internals;

namespace Buform;

[Preserve(AllMembers = true)]
public abstract class FormItemView : ContentView { }

[Preserve(AllMembers = true)]
public abstract class FormItemView<TItem> : FormItemView
    where TItem : class, IFormItem
{
    protected TItem? Item => BindingContext as TItem;
}
