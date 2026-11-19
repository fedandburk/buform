using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Buform;

internal sealed class FormRowGapDecoration : RecyclerView.ItemDecoration
{
    public override void GetItemOffsets(
        Rect outRect,
        View view,
        RecyclerView parent,
        RecyclerView.State state
    )
    {
        ArgumentNullException.ThrowIfNull(outRect);
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(parent);

        var position = parent.GetChildAdapterPosition(view);

        if (position == RecyclerView.NoPosition || parent.GetAdapter() is not IFormRowSource rows)
        {
            return;
        }

        var row = rows.GetRow(position);

        if (row.Type != FormRowType.Item || row.ItemIndex >= row.GroupItemCount - 1)
        {
            return;
        }

        var resources = view.Resources
            ?? throw new InvalidOperationException("The view has no resources.");

        outRect.Bottom = resources.GetDimensionPixelSize(Resource.Dimension.form_item_gap);
    }
}
