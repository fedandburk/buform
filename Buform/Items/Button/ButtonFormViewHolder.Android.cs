using Android.Graphics;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Content.Res;

namespace Buform;

public abstract class ButtonFormViewHolder<TItem> : FormViewHolder<TItem>
    where TItem : ButtonFormItem
{
    protected ButtonFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    protected ButtonFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    protected virtual TextView? LabelView { get; set; }

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Label);

        ItemView.Click += OnItemViewClick;
    }

    protected virtual void UpdateReadOnlyState()
    {
        var isEnabled = !(Item?.IsReadOnly ?? true);

        ItemView.Enabled = isEnabled;

        LabelView?.Enabled = isEnabled;
    }

    protected virtual void UpdateLabel()
    {
        LabelView?.Text = Item?.Label;
    }

    protected virtual void UpdateStyle()
    {
        if (LabelView == null || Item == null)
        {
            return;
        }

        var context = LabelView.Context
            ?? throw new InvalidOperationException("The label has no context.");

        var colorResourceId = Item.Style switch
        {
            ButtonStyle.Default => Resource.Color.form_button_text,
            ButtonStyle.Done => Resource.Color.form_button_text,
            ButtonStyle.Destructive => Resource.Color.form_button_destructive_text,
            _ => throw new ArgumentOutOfRangeException(
                nameof(ButtonFormItem.Style),
                Item.Style,
                null
            ),
        };

        var textColors = AppCompatResources.GetColorStateList(context, colorResourceId);

        LabelView.SetTextColor(textColors);

        var typefaceStyle =
            Item.Style == ButtonStyle.Done ? TypefaceStyle.Bold : TypefaceStyle.Normal;

        LabelView.SetTypeface(Typeface.Create(LabelView.Typeface, typefaceStyle), typefaceStyle);
    }

    protected override void OnItemSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateStyle();
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ButtonFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(ButtonFormItem.Label):
                UpdateLabel();
                break;
            case nameof(ButtonFormItem.Style):
                UpdateStyle();
                break;
            case null:
                UpdateReadOnlyState();
                UpdateLabel();
                UpdateStyle();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ItemView.Click -= OnItemViewClick;

            LabelView = null;
        }

        base.Dispose(disposing);
    }

    private void OnItemViewClick(object? sender, EventArgs e)
    {
        if (Item == null || Item.IsReadOnly)
        {
            return;
        }

        Item.Value.SafeExecute();
    }
}

public sealed class ButtonFormViewHolder : ButtonFormViewHolder<ButtonFormItem>
{
    public ButtonFormViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public ButtonFormViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }
}
