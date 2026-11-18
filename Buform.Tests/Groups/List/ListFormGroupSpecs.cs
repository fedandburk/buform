using System.Collections.ObjectModel;
using AwesomeAssertions;
using Xunit;

namespace Buform.Tests.Groups.List;

public static class ListFormGroupSpecs
{
    public abstract class Context
    {
        protected const string First = "first";
        protected const string Second = "second";
        protected const string Third = "third";
        protected const string Inserted = "inserted";
        protected const string Replacement = "replacement";

        protected Context(params string[] values)
        {
            Source = new ObservableCollection<string>(values);
            Sut = new ListFormGroup<string, TextFormItem<string>>(value => new TextFormItem<string>(
                value
            ))
            {
                Source = Source,
            };
        }

        protected ObservableCollection<string> Source { get; }

        protected ListFormGroup<string, TextFormItem<string>> Sut { get; }

        protected IReadOnlyList<string?> VisibleValues =>
            Sut.Cast<TextFormItem<string>>().Select(item => item.Value).ToList();

        protected IReadOnlyList<object?> HiddenValues =>
            Sut.HiddenItems.Select(item => item.Value).ToList();

        protected TextFormItem<string> VisibleItem(string value)
        {
            return Sut.Cast<TextFormItem<string>>().Single(item => item.Value == value);
        }
    }

    public static class Source
    {
        public sealed class InsertAfterHiddenItem : Context
        {
            public InsertAfterHiddenItem()
                : base(First, Second, Third)
            {
                VisibleItem(First).IsVisible = false;

                base.Source.Insert(1, Inserted);
            }

            [Fact]
            public void ShowsVisibleItemsInSourceOrder()
            {
                VisibleValues.Should().Equal(Inserted, Second, Third);
            }

            [Fact]
            public void KeepsHiddenItemHidden()
            {
                HiddenValues.Should().Equal(First);
            }
        }

        public sealed class RemoveAfterHiddenItem : Context
        {
            public RemoveAfterHiddenItem()
                : base(First, Second, Third)
            {
                VisibleItem(First).IsVisible = false;

                base.Source.RemoveAt(2);
            }

            [Fact]
            public void ShowsRemainingVisibleItem()
            {
                VisibleValues.Should().Equal(Second);
            }
        }

        public sealed class RemoveHiddenItem : Context
        {
            private readonly TextFormItem<string> _removedItem;

            public RemoveHiddenItem()
                : base(First, Second)
            {
                _removedItem = VisibleItem(First);
                _removedItem.IsVisible = false;

                base.Source.RemoveAt(0);
            }

            [Fact]
            public void KeepsVisibleItem()
            {
                VisibleValues.Should().Equal(Second);
            }

            [Fact]
            public void ForgetsHiddenItem()
            {
                HiddenValues.Should().BeEmpty();
            }

            [Fact]
            public void DisposesRemovedItem()
            {
                _removedItem.Value.Should().BeNull();
            }
        }

        public sealed class MoveThenToggleVisibility : Context
        {
            public MoveThenToggleVisibility()
                : base(First, Second, Third)
            {
                base.Source.Move(0, 2);

                var movedItem = VisibleItem(First);
                movedItem.IsVisible = false;
                movedItem.IsVisible = true;
            }

            [Fact]
            public void KeepsMovedPosition()
            {
                VisibleValues.Should().Equal(Second, Third, First);
            }
        }

        public sealed class Replace : Context
        {
            private readonly TextFormItem<string> _replacedItem;

            public Replace()
                : base(First, Second)
            {
                _replacedItem = VisibleItem(First);

                base.Source[0] = Replacement;
            }

            [Fact]
            public void ShowsReplacement()
            {
                VisibleValues.Should().Equal(Replacement, Second);
            }

            [Fact]
            public void DisposesReplacedItem()
            {
                _replacedItem.Value.Should().BeNull();
            }
        }

        public sealed class Clear : Context
        {
            private readonly IReadOnlyList<TextFormItem<string>> _clearedItems;

            public Clear()
                : base(First, Second)
            {
                _clearedItems = [VisibleItem(First), VisibleItem(Second)];

                base.Source.Clear();
            }

            [Fact]
            public void RemovesAllItems()
            {
                VisibleValues.Should().BeEmpty();
            }

            [Fact]
            public void DisposesClearedItems()
            {
                _clearedItems
                    .Select(item => item.Value)
                    .Should()
                    .AllSatisfy(value => value.Should().BeNull());
            }
        }

        public sealed class ReplaceSource : Context
        {
            private readonly IReadOnlyList<TextFormItem<string>> _oldItems;

            public ReplaceSource()
                : base(First, Second)
            {
                _oldItems = [VisibleItem(First), VisibleItem(Second)];

                var oldSource = base.Source;

                Sut.Source = new ObservableCollection<string>([Third]);

                oldSource.Add(Inserted);
            }

            [Fact]
            public void ShowsOnlyNewSourceItems()
            {
                VisibleValues.Should().Equal(Third);
            }

            [Fact]
            public void DisposesOldItems()
            {
                _oldItems
                    .Select(item => item.Value)
                    .Should()
                    .AllSatisfy(value => value.Should().BeNull());
            }
        }

        public sealed class NullSource : Context
        {
            public NullSource()
                : base(First, Second)
            {
                Sut.Source = null;
            }

            [Fact]
            public void RemovesAllItems()
            {
                VisibleValues.Should().BeEmpty();
            }
        }

        public sealed class ReadOnlyObservableSource : Context
        {
            public ReadOnlyObservableSource()
            {
                var values = new ObservableCollection<string>([First]);

                Sut.Source = new ReadOnlyObservableCollection<string>(values);

                values.Add(Second);
            }

            [Fact]
            public void FollowsSourceChanges()
            {
                VisibleValues.Should().Equal(First, Second);
            }
        }

        public sealed class PlainEnumerableSource : Context
        {
            public PlainEnumerableSource()
            {
                var values = new List<string> { First };

                Sut.Source = values;

                values.Add(Second);
            }

            [Fact]
            public void ReadsSourceOnce()
            {
                VisibleValues.Should().Equal(First);
            }
        }
    }

    public static class Dispose
    {
        public sealed class Disposed : Context
        {
            private readonly IReadOnlyList<TextFormItem<string>> _items;

            public Disposed()
                : base(First, Second)
            {
                _items = [VisibleItem(First), VisibleItem(Second)];

                Sut.Dispose();

                Source.Add(Third);
            }

            [Fact]
            public void StopsFollowingSource()
            {
                Sut.Count.Should().Be(2);
            }

            [Fact]
            public void DisposesItems()
            {
                _items
                    .Select(item => item.Value)
                    .Should()
                    .AllSatisfy(value => value.Should().BeNull());
            }
        }
    }
}
