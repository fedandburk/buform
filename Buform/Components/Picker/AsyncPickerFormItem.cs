using System.Linq.Expressions;

namespace Buform;

public class AsyncPickerFormItem<TValue> : PickerFormItemBase<TValue>, IAsyncPickerFormItem
{
    private Func<TValue?, string?>? _formatter;
    private IEnumerable<IPickerOptionFormItem> _options;
    private Func<TValue?, string?>? _optionsFilterValueFactory;
    private Func<CancellationToken, Task<IEnumerable<TValue>>>? _sourceFactory;

    public virtual Func<TValue?, string?>? Formatter
    {
        get => _formatter;
        set
        {
            _formatter = value;

            foreach (var option in Options.OfType<PickerOptionFormItem<TValue>>())
            {
                option.Formatter = Formatter;
            }

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(FormattedValue));
        }
    }

    public virtual Func<TValue?, string?>? OptionsFilterValueFactory
    {
        get => _optionsFilterValueFactory;
        set
        {
            _optionsFilterValueFactory = value;

            foreach (var option in Options.OfType<PickerOptionFormItem<TValue>>())
            {
                option.FilterValueFactory = _optionsFilterValueFactory;
            }

            NotifyPropertyChanged();
            UpdateOptions();
        }
    }

    public override string? FilterString
    {
        get => base.FilterString;
        set
        {
            base.FilterString = value;

            NotifyPropertyChanged();

            UpdateOptions();
        }
    }

    private void UpdateOptions()
    {
        if (!string.IsNullOrEmpty(FilterString))
        {
            Options = _options.Where(option =>
                option.FilterValue?.Contains(FilterString, StringComparison.OrdinalIgnoreCase)
                ?? false
            );
        }
        else
        {
            Options = _options;
        }

        NotifyPropertyChanged(nameof(Options));
    }

    public virtual Func<CancellationToken, Task<IEnumerable<TValue>>>? SourceFactory
    {
        get => _sourceFactory;
        set
        {
            _sourceFactory = value;

            NotifyPropertyChanged();
        }
    }

    public virtual AsyncPickerLoadingState State { get; private set; }

    public override string? FormattedValue => Formatter?.Invoke(Value) ?? Value?.ToString();

    public AsyncPickerFormItem(Expression<Func<TValue>> targetProperty)
        : base(targetProperty)
    {
        _options = Array.Empty<IPickerOptionFormItem>();
    }

    protected virtual IPickerOptionFormItem CreateOption(TValue value)
    {
        return new PickerOptionFormItem<TValue>(value)
        {
            Formatter = Formatter,
            FilterValueFactory = OptionsFilterValueFactory
        };
    }

    public virtual async Task LoadItemsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (State != AsyncPickerLoadingState.None)
            {
                return;
            }

            if (SourceFactory == null)
            {
                State = AsyncPickerLoadingState.Loaded;
                NotifyPropertyChanged(nameof(State));

                return;
            }

            State = AsyncPickerLoadingState.Loading;
            NotifyPropertyChanged(nameof(State));

            var source =
                SourceFactory == null
                    ? null
                    : await SourceFactory(cancellationToken).ConfigureAwait(false);

            _options = source?.Select(CreateOption) ?? Array.Empty<IPickerOptionFormItem>();

            UpdateOptions();

            State = AsyncPickerLoadingState.Loaded;
            NotifyPropertyChanged(nameof(State));
        }
        catch
        {
            State = AsyncPickerLoadingState.Failed;
            NotifyPropertyChanged(nameof(State));
        }
    }

    public override void Pick(IPickerOptionFormItem? item)
    {
        Value = (TValue?)item?.Value;
    }

    public override bool IsPicked(IPickerOptionFormItem item)
    {
        return Equals(Value, item.Value);
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _formatter = null;
        }

        base.Dispose(isDisposing);
    }
}

public class AsyncPickerFormItem : AsyncPickerFormItem<string?>
{
    public AsyncPickerFormItem(Expression<Func<string?>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }
}
