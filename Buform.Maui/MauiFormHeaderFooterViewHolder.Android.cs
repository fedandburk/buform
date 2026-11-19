using Android.Widget;

namespace Buform;

internal sealed class MauiFormHeaderFooterViewHolder : MauiFormViewHolder<FormHeaderFooterView>
{
    public MauiFormHeaderFooterViewHolder(FrameLayout container, IMauiContext mauiContext)
        : base(container, mauiContext)
    {
        /* Required constructor */
    }

    public void Bind(IFormGroup group, FormRowType rowType)
    {
        ArgumentNullException.ThrowIfNull(group);

        var groupType = group.GetType();

        switch (rowType)
        {
            case FormRowType.Header:
                if (MauiFormPlatform.TryGetHeaderViewType(groupType, out var headerViewType))
                {
                    BindMauiView(group, headerViewType!);
                }
                else if (FormPlatform.TryGetGroupHeaderViewType(groupType, out var viewType))
                {
                    BindNativeView(viewType!.Value, group);
                }
                else
                {
                    throw new FormViewNotFoundException(groupType);
                }

                break;
            case FormRowType.Footer:
                if (MauiFormPlatform.TryGetFooterViewType(groupType, out var footerViewType))
                {
                    BindMauiView(group, footerViewType!);
                }
                else if (FormPlatform.TryGetGroupFooterViewType(groupType, out var viewType))
                {
                    BindNativeView(viewType!.Value, group);
                }
                else
                {
                    throw new FormViewNotFoundException(groupType);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(rowType), rowType, null);
        }
    }
}
