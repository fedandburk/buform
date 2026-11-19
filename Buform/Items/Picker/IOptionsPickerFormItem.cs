namespace Buform;

public interface IOptionsPickerFormItem : IValidatableFormItem
{
    string? Label { get; }

    string? Message { get; }

    PickerInputType InputType { get; }

    bool CanBeCleared { get; }

    public string? FormattedValue { get; }

    public string? FilterQuery { get; set; }

    IEnumerable<IPickerOptionFormItem> Options { get; }

    void Pick(IPickerOptionFormItem? item);

    bool IsPicked(IPickerOptionFormItem item);
}
