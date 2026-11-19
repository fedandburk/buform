using Android.Views;

namespace Buform;

internal sealed class ListRowClickListener : Java.Lang.Object, View.IOnClickListener
{
    private readonly IListFormGroup _group;
    private readonly IFormItem _item;

    private ListRowClickListener(IListFormGroup group, IFormItem item)
    {
        _group = group;
        _item = item;
    }

    public void OnClick(View? view)
    {
        _group.SelectCommand.SafeExecute(_item.Value);
    }

    public static void Attach(View view, FormRow row)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(row);

        if (row is not { Group: IListFormGroup group, Item: { } item })
        {
            return;
        }

        view.SetOnClickListener(new ListRowClickListener(group, item));
    }
}
