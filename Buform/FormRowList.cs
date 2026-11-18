using System.Collections;
using System.Collections.Specialized;

namespace Buform;

internal sealed class FormRowList : IReadOnlyList<FormRow>, INotifyCollectionChanged, IDisposable
{
    private const int GroupRowCount = 2;

    private readonly Form _form;
    private readonly List<IFormGroup> _groups;

    public int Count => _groups.Sum(group => group.Count + GroupRowCount);

    public FormRow this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);

            var rowIndex = index;

            foreach (var group in _groups)
            {
                if (rowIndex == 0)
                {
                    return FormRow.Header(group);
                }

                rowIndex--;

                if (rowIndex < group.Count)
                {
                    return FormRow.ForItem(group, group[rowIndex]);
                }

                rowIndex -= group.Count;

                if (rowIndex == 0)
                {
                    return FormRow.Footer(group);
                }

                rowIndex--;
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public FormRowList(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        _form = form;

        _groups = [];

        Synchronize();

        _form.CollectionChanged += OnGroupsChanged;
    }

    private int GetOffset(int groupIndex)
    {
        return _groups.Take(groupIndex).Sum(group => group.Count + GroupRowCount);
    }

    private static List<FormRow> CreateRows(IFormGroup group)
    {
        return
        [
            FormRow.Header(group),
            .. group.Select(item => FormRow.ForItem(group, item)),
            FormRow.Footer(group),
        ];
    }

    private static List<FormRow> CreateRows(IFormGroup group, IList? items)
    {
        return items?.Cast<IFormItem>().Select(item => FormRow.ForItem(group, item)).ToList() ?? [];
    }

    private void Synchronize()
    {
        foreach (var group in _groups)
        {
            group.CollectionChanged -= OnGroupItemsChanged;
        }

        _groups.Clear();
        _groups.AddRange(_form);

        foreach (var group in _groups)
        {
            group.CollectionChanged += OnGroupItemsChanged;
        }
    }

    private void OnGroupsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            {
                var groups = e.NewItems!.Cast<IFormGroup>().ToList();
                var offset = GetOffset(e.NewStartingIndex);

                _groups.InsertRange(e.NewStartingIndex, groups);

                foreach (var group in groups)
                {
                    group.CollectionChanged += OnGroupItemsChanged;
                }

                var rows = groups.SelectMany(CreateRows).ToList();

                Raise(new NotifyCollectionChangedEventArgs(e.Action, rows, offset));

                break;
            }
            case NotifyCollectionChangedAction.Remove:
            {
                var groups = e.OldItems!.Cast<IFormGroup>().ToList();
                var offset = GetOffset(e.OldStartingIndex);
                var rows = groups.SelectMany(CreateRows).ToList();

                foreach (var group in groups)
                {
                    group.CollectionChanged -= OnGroupItemsChanged;
                }

                _groups.RemoveRange(e.OldStartingIndex, groups.Count);

                Raise(new NotifyCollectionChangedEventArgs(e.Action, rows, offset));

                break;
            }
            default:
            {
                Synchronize();

                Raise(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                break;
            }
        }
    }

    private void OnGroupItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is not IFormGroup group)
        {
            return;
        }

        var offset = GetOffset(_groups.IndexOf(group)) + 1;

        var args = e.Action switch
        {
            NotifyCollectionChangedAction.Add => new NotifyCollectionChangedEventArgs(
                e.Action,
                CreateRows(group, e.NewItems),
                e.NewStartingIndex + offset
            ),
            NotifyCollectionChangedAction.Remove => new NotifyCollectionChangedEventArgs(
                e.Action,
                CreateRows(group, e.OldItems),
                e.OldStartingIndex + offset
            ),
            NotifyCollectionChangedAction.Replace => new NotifyCollectionChangedEventArgs(
                e.Action,
                CreateRows(group, e.NewItems),
                CreateRows(group, e.OldItems),
                e.NewStartingIndex + offset
            ),
            NotifyCollectionChangedAction.Move => new NotifyCollectionChangedEventArgs(
                e.Action,
                CreateRows(group, e.NewItems),
                e.NewStartingIndex + offset,
                e.OldStartingIndex + offset
            ),
            _ => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset),
        };

        Raise(args);
    }

    private void Raise(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    public IEnumerator<FormRow> GetEnumerator()
    {
        foreach (var group in _groups)
        {
            foreach (var row in CreateRows(group))
            {
                yield return row;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        _form.CollectionChanged -= OnGroupsChanged;

        foreach (var group in _groups)
        {
            group.CollectionChanged -= OnGroupItemsChanged;
        }

        _groups.Clear();
    }
}
