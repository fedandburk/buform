using System.Collections.Specialized;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Color;
using Google.Android.Material.ListItem;
using Object = Java.Lang.Object;

namespace Buform;

internal static class FormRowAppearance
{
    private const string PayloadName = "FormRowAppearance";

    private static readonly Object Payload = new Java.Lang.String(PayloadName);

    public static void ApplyFormStyle(RecyclerView formView)
    {
        ArgumentNullException.ThrowIfNull(formView);

        var color = MaterialColors.GetColor(formView, Resource.Attribute.colorSurfaceContainerHigh);

        formView.SetBackgroundColor(new Color(color));
        formView.AddItemDecoration(new FormRowGapDecoration());
    }

    public static void Apply(View itemView, FormRow row)
    {
        ArgumentNullException.ThrowIfNull(itemView);
        ArgumentNullException.ThrowIfNull(row);

        if (row.Type != FormRowType.Item || itemView is not ListItemLayout layout)
        {
            return;
        }

        layout.UpdateAppearance(row.ItemIndex, row.GroupItemCount);
    }

    public static ListItemLayout InflateCard(ViewGroup parent, out ListItemCardView card)
    {
        ArgumentNullException.ThrowIfNull(parent);

        var inflater = LayoutInflater.From(parent.Context)
            ?? throw new InvalidOperationException("The parent view has no layout inflater.");

        var layout =
            inflater.Inflate(Resource.Layout.FormItemCardLayout, parent, false) as ListItemLayout
            ?? throw new InvalidOperationException("The card layout is not inflated.");

        card = layout.FindViewById<ListItemCardView>(Resource.Id.Card)
            ?? throw new InvalidOperationException("The card layout has no card.");

        return layout;
    }

    public static bool IsAppearanceUpdate(IList<Object> payloads)
    {
        ArgumentNullException.ThrowIfNull(payloads);

        return payloads.Count > 0 && payloads.All(payload => ReferenceEquals(payload, Payload));
    }

    public static void NotifyRowsChanged(
        RecyclerView.Adapter adapter,
        NotifyCollectionChangedEventArgs args
    )
    {
        ArgumentNullException.ThrowIfNull(adapter);
        ArgumentNullException.ThrowIfNull(args);

        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
            {
                var count = args.NewItems!.Count;

                adapter.NotifyItemRangeInserted(args.NewStartingIndex, count);

                NotifyNeighbors(adapter, args.NewStartingIndex - 1, args.NewStartingIndex + count);

                break;
            }
            case NotifyCollectionChangedAction.Remove:
            {
                adapter.NotifyItemRangeRemoved(args.OldStartingIndex, args.OldItems!.Count);

                NotifyNeighbors(adapter, args.OldStartingIndex - 1, args.OldStartingIndex);

                break;
            }
            case NotifyCollectionChangedAction.Replace:
            {
                adapter.NotifyItemRangeChanged(args.NewStartingIndex, args.NewItems!.Count);

                break;
            }
            case NotifyCollectionChangedAction.Move:
            {
                for (var index = 0; index < args.NewItems!.Count; index++)
                {
                    adapter.NotifyItemMoved(
                        args.OldStartingIndex + index,
                        args.NewStartingIndex + index
                    );
                }

                adapter.NotifyItemRangeChanged(0, adapter.ItemCount, Payload);

                break;
            }
            case NotifyCollectionChangedAction.Reset:
            {
                adapter.NotifyDataSetChanged();

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(args), args.Action, null);
        }
    }

    private static void NotifyNeighbors(
        RecyclerView.Adapter adapter,
        int previousPosition,
        int nextPosition
    )
    {
        if (previousPosition >= 0)
        {
            adapter.NotifyItemChanged(previousPosition, Payload);
        }

        if (nextPosition < adapter.ItemCount)
        {
            adapter.NotifyItemChanged(nextPosition, Payload);
        }
    }
}
