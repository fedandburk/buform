using System.Windows.Input;

namespace Buform;

public class ButtonFormItem : FormItem<ICommand>
{
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

    public virtual ButtonInputType InputType
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public ButtonFormItem(ICommand value)
        : base(value)
    {
        if (Value == null)
        {
            return;
        }

        Value.CanExecuteChanged += OnCanExecuteChanged;
    }

    protected virtual void OnCanExecuteChanged(object? sender, EventArgs e)
    {
        NotifyPropertyChanged(nameof(IsReadOnly));
    }

    protected override void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            if (Value != null)
            {
                Value.CanExecuteChanged -= OnCanExecuteChanged;
            }
        }

        base.Dispose(isDisposing);
    }
}
