using Android.Views;

namespace Buform;

public class DialogFragmentPickerPresenter<TItem> : PickerPresenter<TItem>
    where TItem : class, IOptionsPickerFormItem
{
    private const string FragmentTag = nameof(PickerDialogFragment);

    private readonly Func<TItem, PickerDialogFragment> _fragmentFactory;

    public DialogFragmentPickerPresenter(Func<TItem, PickerDialogFragment> fragmentFactory)
    {
        ArgumentNullException.ThrowIfNull(fragmentFactory);

        _fragmentFactory = fragmentFactory;
    }

    public override void Pick(View anchorView, TItem item)
    {
        ArgumentNullException.ThrowIfNull(anchorView);
        ArgumentNullException.ThrowIfNull(item);

        var fragmentManager = anchorView.GetFragmentActivity().SupportFragmentManager;

        if (fragmentManager.FindFragmentByTag(FragmentTag) != null)
        {
            return;
        }

        var fragment = _fragmentFactory(item);

        fragment.Show(fragmentManager, FragmentTag);
    }
}
