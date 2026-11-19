using Android.Views;

namespace Buform;

internal sealed class FormItemClickListener : Java.Lang.Object, View.IOnClickListener
{
    private readonly IFormGroupHandler _groupHandler;
    private readonly IFormItem _item;

    private FormItemClickListener(IFormGroupHandler groupHandler, IFormItem item)
    {
        _groupHandler = groupHandler;
        _item = item;
    }

    public void OnClick(View? view)
    {
        _groupHandler.SelectItem(_item);
    }

    public static void Attach(View view, IFormGroupHandler groupHandler, IFormItem item)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(groupHandler);
        ArgumentNullException.ThrowIfNull(item);

        if (!groupHandler.CanSelectItem(item))
        {
            Detach(view);

            return;
        }

        var listener = new FormItemClickListener(groupHandler, item);

        view.SetOnClickListener(listener);
        view.SetTag(Resource.Id.FormItemClickListener, listener);
    }

    public static void Detach(View view)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (view.GetTag(Resource.Id.FormItemClickListener) == null)
        {
            return;
        }

        view.SetOnClickListener(null);
        view.SetTag(Resource.Id.FormItemClickListener, null);

        view.Clickable = false;
    }
}
