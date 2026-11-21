using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Buform;

public abstract class FormItem<TValue> : IFormItem
{
    private PropertyInfo? _propertyInfo;
    private Expression<Func<TValue>>? _property;
    private TValue? _value;
    private bool _isVisible;
    private bool _shouldSkipValueChangedCallback;

    protected FormItem(Expression<Func<TValue>> targetProperty)
    {
        ArgumentNullException.ThrowIfNull(targetProperty);

        _property = targetProperty;

        var member = _property.GetMember();

        PropertyName = member.Name;
        _propertyInfo = member as PropertyInfo;

        _isVisible = true;
    }

    protected FormItem(TValue value)
    {
        _value = value;

        _isVisible = true;
    }

    public event EventHandler<FormValueChangedEventArgs>? ValueChanged;

    public event PropertyChangedEventHandler? PropertyChanged;

    public object? Target { get; private set; }

    public string? PropertyName { get; }

    public virtual bool IsReadOnly
    {
        get;
        set
        {
            field = value;

            NotifyPropertyChanged();
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            if (_isVisible == value)
            {
                return;
            }

            _isVisible = value;

            NotifyPropertyChanged();
        }
    }

    object? IFormItem.Value
    {
        get => Value;
        set => Value = (TValue?)value;
    }

    public TValue? Value
    {
        get => GetValue();
        set => SetValue(value);
    }

    public virtual Action<Form, TValue?>? ValueChangedCallback { get; set; }

    protected Form? Form { get; private set; }

    protected virtual bool IsValueChanged { get; set; }

    public void Initialize(Form form, object target)
    {
        ArgumentNullException.ThrowIfNull(form);
        ArgumentNullException.ThrowIfNull(target);

        if (_propertyInfo?.DeclaringType?.IsInstanceOfType(target) == false)
        {
            throw new ArgumentException(
                $"Target of type '{target.GetType()}' does not declare property '{PropertyName}'.",
                nameof(target)
            );
        }

        Form = form;

        if (Target is INotifyPropertyChanged oldNotifyPropertyChanged)
        {
            oldNotifyPropertyChanged.PropertyChanged -= OnTargetPropertyChanged;
        }

        Target = target;

        if (Target is INotifyPropertyChanged newNotifyPropertyChanged)
        {
            newNotifyPropertyChanged.PropertyChanged += OnTargetPropertyChanged;
        }

        _shouldSkipValueChangedCallback = true;

        OnValueChanged();

        _shouldSkipValueChangedCallback = false;
    }

    public void Dispose()
    {
        Dispose(true);
    }

    protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnValueChanged()
    {
        if (Form == null)
        {
            return;
        }

        NotifyPropertyChanged(nameof(Value));

        ValueChanged?.Invoke(this, new FormValueChangedEventArgs(PropertyName ?? string.Empty));

        if (_shouldSkipValueChangedCallback)
        {
            return;
        }

        ValueChangedCallback?.Invoke(Form, Value);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (!isDisposing)
        {
            return;
        }

        _propertyInfo = null;
        _property = null;

        if (Target is INotifyPropertyChanged notifyPropertyChanged)
        {
            notifyPropertyChanged.PropertyChanged -= OnTargetPropertyChanged;
        }

        Target = null;

        _value = default;
    }

    private TValue? GetValue()
    {
        if (_propertyInfo == null)
        {
            return _value;
        }

        return Target == null ? default : (TValue?)_propertyInfo.GetValue(Target);
    }

    private void SetValue(TValue? value)
    {
        if (!Equals(value, Value))
        {
            IsValueChanged = true;
        }

        if (_propertyInfo == null)
        {
            _value = value;

            OnValueChanged();

            return;
        }

        if (Target == null)
        {
            return;
        }

        _shouldSkipValueChangedCallback = true;

        _propertyInfo.SetValue(Target, value);

        _shouldSkipValueChangedCallback = false;

        OnValueChanged();
    }

    private void OnTargetPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != PropertyName && !string.IsNullOrWhiteSpace(e.PropertyName))
        {
            return;
        }

        OnValueChanged();
    }
}
