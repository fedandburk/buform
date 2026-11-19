using Android.Runtime;
using Android.Views;
using Google.Android.Material.Button;

namespace Buform.Example.Droid;

public sealed class RandomNumberGeneratorViewHolder : TextFormViewHolder
{
    private MaterialButton? _generateButton;

    public RandomNumberGeneratorViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        /* Required constructor */
    }

    public RandomNumberGeneratorViewHolder(View itemView)
        : base(itemView)
    {
        /* Required constructor */
    }

    private void OnGenerateButtonClick(object? sender, EventArgs e)
    {
        if (Data is not RandomNumberGeneratorItem item)
        {
            return;
        }

        item.GenerateCommand.Execute(null);
    }

    protected override void Initialize()
    {
        base.Initialize();

        _generateButton = ItemView.FindViewById<MaterialButton>(Resource.Id.GenerateButton);

        _generateButton?.Click += OnGenerateButtonClick;
    }

    protected override void UpdateReadOnlyState()
    {
        base.UpdateReadOnlyState();

        _generateButton?.Enabled = !(Data?.IsReadOnly ?? true);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _generateButton?.Click -= OnGenerateButtonClick;

            _generateButton = null;
        }

        base.Dispose(disposing);
    }
}
