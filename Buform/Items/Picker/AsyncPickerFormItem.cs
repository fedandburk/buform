using System.Linq.Expressions;

namespace Buform;

public class AsyncPickerFormItem<TValue> : OptionsPickerFormItem<TValue>, IAsyncPickerFormItem
{
    public virtual Func<TValue?, string?>? Formatter
    {
        get;
        set
        {
            field = value;

            UpdateOptionFormatters();

            NotifyPropertyChanged();
            NotifyPropertyChanged(nameof(FormattedValue));
        }
    }

    public virtual Func<TValue?, string?>? OptionFilterValueFactory
    {
        get;
        set
        {
            field = value;

            UpdateOptionFormatters();

            NotifyPropertyChanged();

            UpdateOptions();
        }
    }

    public virtual Func<CancellationToken, Task<IEnumerable<TValue>>>? SourceFactory
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public AsyncPickerLoadingState State { get; private set; }

    public Exception? LoadException { get; private set; }

    public override string? FormattedValue => Formatter?.Invoke(Value) ?? Value?.ToString();

    public AsyncPickerFormItem(Expression<Func<TValue>> targetProperty)
        : base(targetProperty)
    {
        /* Required constructor */
    }

    protected virtual IFormOption CreateOption(TValue value)
    {
        return new FormOption<TValue>(value)
        {
            Formatter = Formatter,
            FilterValueFactory = OptionFilterValueFactory,
        };
    }

    protected virtual IEnumerable<IFormOption> CreateOptions(IEnumerable<TValue> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return source.Select(CreateOption);
    }

    private void UpdateOptionFormatters()
    {
        foreach (var option in AllOptions.OfType<FormOption<TValue>>())
        {
            option.Formatter = Formatter;
            option.FilterValueFactory = OptionFilterValueFactory;
        }

        NotifyPropertyChanged(nameof(Options));
    }

    private void SetState(AsyncPickerLoadingState state, Exception? loadException)
    {
        State = state;
        LoadException = loadException;

        NotifyPropertyChanged(nameof(State));
        NotifyPropertyChanged(nameof(LoadException));
    }

    public virtual async Task LoadOptionsAsync(CancellationToken cancellationToken)
    {
        if (State != AsyncPickerLoadingState.None)
        {
            return;
        }

        if (SourceFactory == null)
        {
            SetState(AsyncPickerLoadingState.Loaded, null);

            return;
        }

        SetState(AsyncPickerLoadingState.Loading, null);

        try
        {
            var source = await SourceFactory(cancellationToken);

            SetOptions(CreateOptions(source));

            SetState(AsyncPickerLoadingState.Loaded, null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            SetState(AsyncPickerLoadingState.None, null);

            throw;
        }
        catch (Exception exception)
        {
            SetState(AsyncPickerLoadingState.Failed, exception);
        }
    }

    public override void Pick(IFormOption? option)
    {
        Value = option?.Value is TValue value ? value : default;
    }

    public override bool IsPicked(IFormOption option)
    {
        ArgumentNullException.ThrowIfNull(option);

        return Equals(Value, option.Value);
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
