namespace Buform;

public interface IOptionsPickerFormItem : IValidatableFormItem
{
    string? Label { get; }

    string? Message { get; }

    PickerPresentation Presentation { get; }

    bool IsClearable { get; }

    string? FormattedValue { get; }

    string? FilterQuery { get; set; }

    IReadOnlyList<IFormOption> Options { get; }

    void Pick(IFormOption? option);

    bool IsPicked(IFormOption option);
}
