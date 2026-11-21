using System.Collections.Specialized;
using AwesomeAssertions;
using DynamicData;
using Xunit;

namespace Buform.Tests;

public static class FormGroupSpecs
{
    public static class IsVisible
    {
        public sealed class HideMiddleItem : Context
        {
            public HideMiddleItem()
            {
                SecondItem.IsVisible = false;
            }

            [Fact]
            public void RemovesItemFromVisibleItems()
            {
                VisibleValues.Should().Equal(First, Third);
            }

            [Fact]
            public void AddsItemToHiddenItems()
            {
                HiddenValues.Should().Equal(Second);
            }

            [Fact]
            public void RaisesRemoveAtVisibleIndex()
            {
                Changes
                    .Select(change => (change.Action, change.OldStartingIndex))
                    .Should()
                    .Equal((NotifyCollectionChangedAction.Remove, 1));
            }
        }

        public sealed class ShowItemBeforeOtherHiddenItem : Context
        {
            public ShowItemBeforeOtherHiddenItem()
            {
                SecondItem.IsVisible = false;
                FirstItem.IsVisible = false;

                Changes.Clear();

                SecondItem.IsVisible = true;
            }

            [Fact]
            public void RestoresOriginalOrder()
            {
                VisibleValues.Should().Equal(Second, Third);
            }

            [Fact]
            public void RaisesAddAtVisibleIndex()
            {
                Changes
                    .Select(change => (change.Action, change.NewStartingIndex))
                    .Should()
                    .Equal((NotifyCollectionChangedAction.Add, 0));
            }
        }
    }

    public static class CollectionChanged
    {
        public sealed class HandlerThrowsOnVisibilityChange : Context
        {
            private readonly Exception? _exception;

            public HandlerThrowsOnVisibilityChange()
            {
                Sut.CollectionChanged += (_, _) => throw new InvalidOperationException();

                _exception = Record.Exception(() => SecondItem.IsVisible = false);
            }

            [Fact]
            public void PropagatesException()
            {
                _exception.Should().BeOfType<InvalidOperationException>();
            }
        }
    }

    public static class Insert
    {
        public sealed class AfterHiddenItem : Context
        {
            public AfterHiddenItem()
            {
                FirstItem.IsVisible = false;

                Changes.Clear();

                Sut.Insert(1, new TextFormItem<string>(Inserted));
            }

            [Fact]
            public void InsertsAtSourceIndex()
            {
                VisibleValues.Should().Equal(Inserted, Second, Third);
            }

            [Fact]
            public void RaisesAddAtVisibleIndex()
            {
                Changes
                    .Select(change => (change.Action, change.NewStartingIndex))
                    .Should()
                    .Equal((NotifyCollectionChangedAction.Add, 0));
            }
        }
    }

    public static class Remove
    {
        public sealed class VisibleItemAfterHiddenItem : Context
        {
            public VisibleItemAfterHiddenItem()
            {
                FirstItem.IsVisible = false;

                Changes.Clear();

                Sut.Remove(ThirdItem);
            }

            [Fact]
            public void RemovesItem()
            {
                VisibleValues.Should().Equal(Second);
            }

            [Fact]
            public void RaisesRemoveAtVisibleIndex()
            {
                Changes
                    .Select(change => (change.Action, change.OldStartingIndex))
                    .Should()
                    .Equal((NotifyCollectionChangedAction.Remove, 1));
            }

            [Fact]
            public void DisposesItem()
            {
                ThirdItem.Value.Should().BeNull();
            }
        }

        public sealed class HiddenItem : Context
        {
            public HiddenItem()
            {
                FirstItem.IsVisible = false;

                Changes.Clear();

                Sut.Remove(FirstItem);
            }

            [Fact]
            public void ForgetsHiddenItem()
            {
                HiddenValues.Should().BeEmpty();
            }

            [Fact]
            public void RaisesNoCollectionChange()
            {
                Changes.Should().BeEmpty();
            }
        }
    }

    public static class Move
    {
        public sealed class AcrossHiddenItem : Context
        {
            public AcrossHiddenItem()
            {
                SecondItem.IsVisible = false;

                Sut.Move(2, 0);
            }

            [Fact]
            public void ShowsItemsInNewOrder()
            {
                VisibleValues.Should().Equal(Third, First);
            }
        }
    }

    public static class Clear
    {
        public sealed class WithHiddenItem : Context
        {
            public WithHiddenItem()
            {
                SecondItem.IsVisible = false;

                Sut.Clear();
            }

            [Fact]
            public void RemovesVisibleItems()
            {
                VisibleValues.Should().BeEmpty();
            }

            [Fact]
            public void RemovesHiddenItems()
            {
                HiddenValues.Should().BeEmpty();
            }

            [Fact]
            public void DisposesAllItems()
            {
                AllValues.Should().AllSatisfy(value => value.Should().BeNull());
            }
        }
    }

    public static class Connect
    {
        public sealed class WithHiddenItem : Context
        {
            private readonly IReadOnlyList<IFormItem> _items;

            public WithHiddenItem()
            {
                SecondItem.IsVisible = false;

                using var subscription = Sut.Connect().Bind(out var items).Subscribe();

                _items = items.ToList();
            }

            [Fact]
            public void EmitsAllItems()
            {
                _items.Should().Equal(FirstItem, SecondItem, ThirdItem);
            }
        }
    }

    public static class Dispose
    {
        public sealed class WithHiddenItem : Context
        {
            public WithHiddenItem()
            {
                SecondItem.IsVisible = false;

                Sut.Dispose();
            }

            [Fact]
            public void DisposesAllItems()
            {
                AllValues.Should().AllSatisfy(value => value.Should().BeNull());
            }
        }
    }

    public static class PropertyChanged
    {
        public sealed class ItemHidden : Context
        {
            public ItemHidden()
            {
                SecondItem.IsVisible = false;
            }

            [Fact]
            public void RaisesCountChanged()
            {
                PropertyChanges.Should().Contain(nameof(Sut.Count));
            }
        }
    }

    public abstract class Context
    {
        protected const string First = "first";
        protected const string Second = "second";
        protected const string Third = "third";
        protected const string Inserted = "inserted";

        protected Context()
        {
            FirstItem = new TextFormItem<string>(First);
            SecondItem = new TextFormItem<string>(Second);
            ThirdItem = new TextFormItem<string>(Third);

            Sut = new LabeledFormGroup { FirstItem, SecondItem, ThirdItem };

            Changes = [];
            PropertyChanges = [];

            Sut.CollectionChanged += (_, args) => Changes.Add(args);
            Sut.PropertyChanged += (_, args) => PropertyChanges.Add(args.PropertyName);
        }

        protected TextFormItem<string> FirstItem { get; }
        protected TextFormItem<string> SecondItem { get; }
        protected TextFormItem<string> ThirdItem { get; }

        protected LabeledFormGroup Sut { get; }

        protected List<NotifyCollectionChangedEventArgs> Changes { get; }

        protected List<string?> PropertyChanges { get; }

        protected IReadOnlyList<object?> AllValues =>
            [FirstItem.Value, SecondItem.Value, ThirdItem.Value];

        protected IReadOnlyList<object?> VisibleValues => Sut.Select(item => item.Value).ToList();

        protected IReadOnlyList<object?> HiddenValues
        {
            get
            {
                using var subscription = Sut.Connect().Bind(out var items).Subscribe();

                return items.Where(item => !item.IsVisible).Select(item => item.Value).ToList();
            }
        }
    }
}
