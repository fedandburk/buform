namespace Buform;

public interface ILabeledFormGroup : IFormGroup
{
    string? HeaderLabel { get; }
    string? FooterLabel { get; }
}
