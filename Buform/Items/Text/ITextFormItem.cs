namespace Buform;

public interface ITextFormItem : IFormItem
{
    string? FormattedValue { get; }
}
