namespace Buform;

public interface IAsyncPickerFormItem : IOptionsPickerFormItem
{
    AsyncPickerLoadingState State { get; }

    Exception? LoadException { get; }

    Task LoadOptionsAsync(CancellationToken cancellationToken);
}
