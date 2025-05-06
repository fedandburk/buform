using System.Windows.Input;

namespace Buform;

public abstract class FormGroup<TFormItem> : FormCollection<TFormItem>, IFormGroup
    where TFormItem : IFormItem
{
    private readonly Dictionary<TFormItem, int> _hiddenItems = new();
    private readonly List<TFormItem> _originalItems = [];

    private ICommand? _removeCommand;
    private ICommand? _moveCommand;
    private ICommand? _insertCommand;

    public IEnumerable<IFormItem> HiddenItems => _hiddenItems.Keys.Cast<IFormItem>();

    public virtual ICommand? RemoveCommand
    {
        get => _removeCommand;
        set
        {
            _removeCommand = value;
            NotifyPropertyChanged();
        }
    }

    public virtual ICommand? MoveCommand
    {
        get => _moveCommand;
        set
        {
            _moveCommand = value;
            NotifyPropertyChanged();
        }
    }

    public virtual ICommand? InsertCommand
    {
        get => _insertCommand;
        set
        {
            _insertCommand = value;
            NotifyPropertyChanged();
        }
    }

    protected override void InsertItem(int index, TFormItem item)
    {
        var insertInOriginalIndex = GetOriginalInsertIndex(index);
        _originalItems.Insert(insertInOriginalIndex, item);

        Subscribe(item);

        if (item.IsVisible)
        {
            base.InsertItem(index, item);
        }
        else
        {
            _hiddenItems[item] = index;
        }
    }

    protected override void SetItem(int index, TFormItem item)
    {
        var oldItem = this[index];
        Unsubscribe(oldItem);

        base.SetItem(index, item);
        _originalItems[IndexOfOriginal(oldItem)] = item;

        Subscribe(item);
    }

    protected override void RemoveItem(int index)
    {
        var item = this[index];
        Unsubscribe(item);

        _originalItems.Remove(item);

        if (item.IsVisible)
        {
            base.RemoveItem(index);
        }
        else
        {
            _hiddenItems.Remove(item);
        }
    }

    protected override void ClearItems()
    {
        foreach (var item in _originalItems)
        {
            Unsubscribe(item);
        }

        _originalItems.Clear();
        _hiddenItems.Clear();

        base.ClearItems();
    }

    IEnumerator<IFormItem> IEnumerable<IFormItem>.GetEnumerator()
    {
        return (IEnumerator<IFormItem>)base.GetEnumerator();
    }

    protected virtual void OnItemValueChanged(object? sender, FormValueChangedEventArgs e)
    {
        NotifyValueChanged(e);
    }

    protected virtual void OnItemVisibilityChanged(object? sender, EventArgs e)
    {
        if (sender is not TFormItem item)
        {
            return;
        }

        if (item.IsVisible)
        {
            if (!_hiddenItems.Remove(item))
            {
                return;
            }

            var originalIndex = IndexOfOriginal(item);
            var insertIndex = GetVisibleInsertIndex(originalIndex);

            base.InsertItem(insertIndex, item);
        }
        else
        {
            var index = IndexOf(item);
            
            if (index < 0)
            {
                return;
            }

            base.RemoveItem(index);
            _hiddenItems[item] = index;
        }
    }

    protected void AddItem(TFormItem item)
    {
        InsertItem(Count, item);
    }

    public virtual IFormItem? GetItem(string propertyName)
    {
        return this.FirstOrDefault<TFormItem>(i => i.PropertyName == propertyName)
            ?? HiddenItems.FirstOrDefault(i => i.PropertyName == propertyName);
    }

    private void Subscribe(TFormItem item)
    {
        item.ValueChanged += OnItemValueChanged;
        item.VisibilityChanged += OnItemVisibilityChanged;
    }

    private void Unsubscribe(TFormItem item)
    {
        item.ValueChanged -= OnItemValueChanged;
        item.VisibilityChanged -= OnItemVisibilityChanged;
    }

    private int IndexOfOriginal(TFormItem item) => _originalItems.IndexOf(item);

    private int GetOriginalInsertIndex(int visibleIndex)
    {
        if (visibleIndex == Count)
        {
            return _originalItems.Count;
        }

        var nextVisibleItem = this[visibleIndex];
        return IndexOfOriginal(nextVisibleItem);
    }

    private int GetVisibleInsertIndex(int originalIndex)
    {
        var count = 0;

        for (var i = 0; i < originalIndex; i++)
        {
            var item = _originalItems[i];
            
            if (item.IsVisible)
            {
                count++;
            }
        }
        
        return count;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        foreach (var item in _originalItems)
        {
            Unsubscribe(item);
            item.Dispose();
        }
    }
}
