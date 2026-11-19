namespace Buform;

internal sealed class PickerOptionState
{
    public IPickerOptionFormItem Option { get; }

    public string? Text { get; }

    public bool IsPicked { get; }

    public PickerOptionState(IPickerOptionFormItem option, string? text, bool isPicked)
    {
        ArgumentNullException.ThrowIfNull(option);

        Option = option;
        Text = text;
        IsPicked = isPicked;
    }
}
