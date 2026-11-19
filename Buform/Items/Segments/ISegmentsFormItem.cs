namespace Buform;

public interface ISegmentsFormItem : IValidatableFormItem
{
    string? Label { get; }

    IReadOnlyList<IFormOption> Options { get; }
}
