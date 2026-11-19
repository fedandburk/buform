namespace Buform;

public interface IFormOption
{
    object? Value { get; }

    string? FormattedValue { get; }

    string? FilterValue { get; }
}
