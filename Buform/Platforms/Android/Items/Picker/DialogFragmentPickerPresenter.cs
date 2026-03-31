using Android.Views;

namespace Buform;

public sealed class DialogFragmentPickerPresenter<TItem> : PickerPresenterBase<TItem>
    where TItem : class, IPickerFormItemBase
{
    private readonly Func<TItem, PickerDialogFragment> _fragmentFactory;
    private PickerDialogFragment? _fragment;

    public DialogFragmentPickerPresenter(Func<TItem, PickerDialogFragment> fragmentFactory)
    {
        ArgumentNullException.ThrowIfNull(fragmentFactory);
        _fragmentFactory = fragmentFactory;
    }

    public override void Pick(View anchorView, TItem item)
    {
        var activity = anchorView.Context?.GetFragmentActivity();
        if (activity == null)
        {
            return;
        }

        if (_fragment?.Dialog?.IsShowing == true)
        {
            return;
        }

        _fragment = _fragmentFactory(item);
        _fragment.Show(activity.SupportFragmentManager, nameof(PickerDialogFragment));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_fragment?.Dialog?.IsShowing == true)
            {
                _fragment.DismissAllowingStateLoss();
            }

            _fragment = null;
        }

        base.Dispose(disposing);
    }
}
