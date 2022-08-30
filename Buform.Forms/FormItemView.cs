using Xamarin.Forms;

namespace Buform
{
    public class FormItemView : ContentView
    {
    }
    public class FormItemView<TItem> : FormItemView
        where TItem : class, IFormItem
    {
        protected TItem? Item => BindingContext as TItem;
    }
}