using Android.Views;

namespace Buform;

public class DialogPickerPresenter<TItem> : PickerPresenter<TItem>
    where TItem : class, IOptionsPickerFormItem
{
    private const string FragmentTag = nameof(PickerDialogFragment);

    public DialogPickerPresenter(Func<TItem, PickerDialogFragment> fragmentFactory)
    {
        ArgumentNullException.ThrowIfNull(fragmentFactory);

        FragmentFactory = fragmentFactory;
    }

    protected Func<TItem, PickerDialogFragment> FragmentFactory { get; }

    public override Task PickAsync(View sourceView, TItem item)
    {
        ArgumentNullException.ThrowIfNull(sourceView);
        ArgumentNullException.ThrowIfNull(item);

        var fragmentManager = sourceView.GetFragmentActivity().SupportFragmentManager;

        if (fragmentManager.FindFragmentByTag(FragmentTag) != null)
        {
            return Task.CompletedTask;
        }

        var fragment = FragmentFactory(item);

        fragment.Show(fragmentManager, FragmentTag);

        return Task.CompletedTask;
    }
}
