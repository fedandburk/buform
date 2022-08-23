using Xamarin.Forms;

namespace Buform
{
    public class FormsFormCell : ContentView
    {
    }

    public class FormsFormCell<TItem> : FormsFormCell
        where TItem : class, IFormItem
    {
        protected TItem? Item => BindingContext as TItem;
    }
}