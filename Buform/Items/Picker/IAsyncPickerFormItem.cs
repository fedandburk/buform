namespace Buform;

public interface IAsyncPickerFormItem : IOptionsPickerFormItem
{
    AsyncPickerLoadingState State { get; }

    Task LoadItemsAsync(CancellationToken cancellationToken);
}
