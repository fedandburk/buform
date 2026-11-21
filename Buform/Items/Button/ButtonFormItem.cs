using System.Windows.Input;

namespace Buform;

public class ButtonFormItem : FormItem<ICommand>
{
    private ICommand? _command;

    public ButtonFormItem(ICommand value)
        : base(value)
    {
        ArgumentNullException.ThrowIfNull(value);

        SubscribeToCommand(value);
    }

    public override bool IsReadOnly
    {
        get => base.IsReadOnly || !Value.SafeCanExecute();
        set => base.IsReadOnly = value;
    }

    public virtual string? Label
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public virtual ButtonStyle Style
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    protected virtual void OnCanExecuteChanged(object? sender, EventArgs e)
    {
        NotifyPropertyChanged(nameof(IsReadOnly));
    }

    protected override void OnValueChanged()
    {
        SubscribeToCommand(Value);

        base.OnValueChanged();

        NotifyPropertyChanged(nameof(IsReadOnly));
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            SubscribeToCommand(null);
        }

        base.Dispose(isDisposing);
    }

    private void SubscribeToCommand(ICommand? command)
    {
        if (ReferenceEquals(_command, command))
        {
            return;
        }

        _command?.CanExecuteChanged -= OnCanExecuteChanged;

        _command = command;

        _command?.CanExecuteChanged += OnCanExecuteChanged;
    }
}
