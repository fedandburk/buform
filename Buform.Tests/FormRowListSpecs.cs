using System.Collections.Specialized;
using AwesomeAssertions;
using Xunit;

namespace Buform.Tests;

public static class FormRowListSpecs
{
    public abstract class Context
    {
        protected const string First = "first";
        protected const string Second = "second";
        protected const string Third = "third";
        protected const string Added = "added";

        protected Context()
        {
            FirstItem = new TextFormItem<string>(First);
            SecondItem = new TextFormItem<string>(Second);
            ThirdItem = new TextFormItem<string>(Third);

            FirstGroup = new LabeledFormGroup { FirstItem, SecondItem };
            SecondGroup = new LabeledFormGroup { ThirdItem };

            Form = new Form(new Model()) { FirstGroup, SecondGroup };

            Sut = new FormRowList(Form);

            Changes = [];

            Sut.CollectionChanged += (_, args) => Changes.Add(args);
        }

        protected TextFormItem<string> FirstItem { get; }
        protected TextFormItem<string> SecondItem { get; }
        protected TextFormItem<string> ThirdItem { get; }

        protected LabeledFormGroup FirstGroup { get; }
        protected LabeledFormGroup SecondGroup { get; }

        protected Form Form { get; }

        internal FormRowList Sut { get; }

        protected List<NotifyCollectionChangedEventArgs> Changes { get; }

        internal IReadOnlyList<(FormRowType, IDisposable)> Rows =>
            Sut.Select(row => (row.Type, row.Data)).ToList();

        protected IReadOnlyList<(NotifyCollectionChangedAction, int, int)> NewChanges =>
            Changes
                .Select(change =>
                    (change.Action, change.NewStartingIndex, change.NewItems?.Count ?? 0)
                )
                .ToList();

        protected IReadOnlyList<(NotifyCollectionChangedAction, int, int)> OldChanges =>
            Changes
                .Select(change =>
                    (change.Action, change.OldStartingIndex, change.OldItems?.Count ?? 0)
                )
                .ToList();
    }

    public static class Enumeration
    {
        public sealed class TwoGroups : Context
        {
            [Fact]
            public void ListsHeaderItemsAndFooterOfEachGroup()
            {
                Rows.Should()
                    .Equal(
                        (FormRowType.Header, FirstGroup),
                        (FormRowType.Item, FirstItem),
                        (FormRowType.Item, SecondItem),
                        (FormRowType.Footer, FirstGroup),
                        (FormRowType.Header, SecondGroup),
                        (FormRowType.Item, ThirdItem),
                        (FormRowType.Footer, SecondGroup)
                    );
            }

            [Fact]
            public void GivesEachItemRowItsPositionInItsGroup()
            {
                Sut.Where(row => row.Type == FormRowType.Item)
                    .Select(row => (row.ItemIndex, row.GroupItemCount))
                    .Should()
                    .Equal((0, 2), (1, 2), (0, 1));
            }
        }

        public sealed class ItemHiddenInFirstGroup : Context
        {
            public ItemHiddenInFirstGroup()
            {
                FirstItem.IsVisible = false;
            }

            [Fact]
            public void CountsOnlyVisibleItemsOfTheGroup()
            {
                Sut.Where(row => row.Type == FormRowType.Item)
                    .Select(row => (row.ItemIndex, row.GroupItemCount))
                    .Should()
                    .Equal((0, 1), (0, 1));
            }
        }
    }

    public static class CollectionChanged
    {
        public sealed class ItemAddedToSecondGroup : Context
        {
            public ItemAddedToSecondGroup()
            {
                SecondGroup.Add(new TextFormItem<string>(Added));
            }

            [Fact]
            public void RaisesAddAtRowIndex()
            {
                NewChanges.Should().Equal((NotifyCollectionChangedAction.Add, 6, 1));
            }
        }

        public sealed class ItemHiddenInFirstGroup : Context
        {
            public ItemHiddenInFirstGroup()
            {
                FirstItem.IsVisible = false;
            }

            [Fact]
            public void RaisesRemoveAtRowIndex()
            {
                OldChanges.Should().Equal((NotifyCollectionChangedAction.Remove, 1, 1));
            }
        }

        public sealed class ItemMovedInFirstGroup : Context
        {
            public ItemMovedInFirstGroup()
            {
                FirstGroup.Move(1, 0);
            }

            [Fact]
            public void RaisesMoveBetweenRowIndices()
            {
                Changes
                    .Select(change =>
                        (change.Action, change.OldStartingIndex, change.NewStartingIndex)
                    )
                    .Should()
                    .Equal((NotifyCollectionChangedAction.Move, 2, 1));
            }
        }

        public sealed class GroupAdded : Context
        {
            public GroupAdded()
            {
                Form.Add(new LabeledFormGroup { new TextFormItem<string>(Added) });
            }

            [Fact]
            public void RaisesAddOfGroupRows()
            {
                NewChanges.Should().Equal((NotifyCollectionChangedAction.Add, 7, 3));
            }
        }

        public sealed class GroupRemoved : Context
        {
            public GroupRemoved()
            {
                Form.Remove(FirstGroup);
            }

            [Fact]
            public void RaisesRemoveOfGroupRows()
            {
                OldChanges.Should().Equal((NotifyCollectionChangedAction.Remove, 0, 4));
            }
        }

        public sealed class ItemAddedToRemovedGroup : Context
        {
            public ItemAddedToRemovedGroup()
            {
                Form.Remove(FirstGroup);

                Changes.Clear();

                FirstGroup.Add(new TextFormItem<string>(Added));
            }

            [Fact]
            public void RaisesNothing()
            {
                Changes.Should().BeEmpty();
            }
        }
    }

    public static class Dispose
    {
        public sealed class Disposed : Context
        {
            public Disposed()
            {
                Sut.Dispose();

                SecondGroup.Add(new TextFormItem<string>(Added));
                Form.Add(new LabeledFormGroup());
            }

            [Fact]
            public void RaisesNothing()
            {
                Changes.Should().BeEmpty();
            }
        }
    }
}
