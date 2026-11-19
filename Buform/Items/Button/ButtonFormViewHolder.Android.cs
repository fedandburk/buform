using Android.Graphics;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Content.Res;

namespace Buform;

public class ButtonFormViewHolder : FormViewHolder<ButtonFormItem>
{
    protected virtual TextView? LabelView { get; set; }

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

    private void OnItemViewClick(object? sender, EventArgs e)
    {
        if (Data == null || Data.IsReadOnly)
        {
            return;
        }

        Data.Value.SafeExecute();
    }

    protected override void Initialize()
    {
        LabelView = ItemView.FindViewById<TextView>(Resource.Id.Label);

        ItemView.Click += OnItemViewClick;
    }

    protected virtual void UpdateReadOnlyState()
    {
        var isEnabled = !(Data?.IsReadOnly ?? true);

        ItemView.Enabled = isEnabled;

        LabelView?.Enabled = isEnabled;
    }

    protected virtual void UpdateLabel()
    {
        LabelView?.Text = Data?.Label;
    }

    protected virtual void UpdateInputType()
    {
        if (LabelView == null || Data == null)
        {
            return;
        }

        var context = LabelView.Context
            ?? throw new InvalidOperationException("The label has no context.");

        var colorResourceId = Data.InputType switch
        {
            ButtonInputType.Default => Resource.Color.form_button_text,
            ButtonInputType.Done => Resource.Color.form_button_text,
            ButtonInputType.Destructive => Resource.Color.form_button_destructive_text,
            _ => throw new ArgumentOutOfRangeException(
                nameof(ButtonFormItem.InputType),
                Data.InputType,
                null
            ),
        };

        var textColors = AppCompatResources.GetColorStateList(context, colorResourceId);

        LabelView.SetTextColor(textColors);

        var typefaceStyle =
            Data.InputType == ButtonInputType.Done ? TypefaceStyle.Bold : TypefaceStyle.Normal;

        LabelView.SetTypeface(Typeface.Create(LabelView.Typeface, typefaceStyle), typefaceStyle);
    }

    protected override void OnDataSet()
    {
        UpdateReadOnlyState();
        UpdateLabel();
        UpdateInputType();
    }

    protected override void OnDataPropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(ButtonFormItem.IsReadOnly):
                UpdateReadOnlyState();
                break;
            case nameof(ButtonFormItem.Label):
                UpdateLabel();
                break;
            case nameof(ButtonFormItem.InputType):
                UpdateInputType();
                break;
            case null:
                OnDataSet();
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
}
