namespace Buform;

internal sealed class PickerOptionState
{
    public IFormOption Option { get; }

    public string? Text { get; }

    public bool IsPicked { get; }

    public PickerOptionState(IFormOption option, string? text, bool isPicked)
    {
        ArgumentNullException.ThrowIfNull(option);

        Option = option;
        Text = text;
        IsPicked = isPicked;
    }
}
