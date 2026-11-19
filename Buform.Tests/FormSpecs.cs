using System.Collections.ObjectModel;
using AwesomeAssertions;
using NSubstitute;
using Xunit;

namespace Buform.Tests;

public static class FormSpecs
{
    public abstract class Context
    {
        protected const string First = "first";
        protected const string Changed = "changed";

        protected Context()
        {
            Target = new Model();
            Sut = new Form(Target);

            ValueChanges = [];

            Sut.ValueChanged += (_, args) => ValueChanges.Add(args.PropertyName);
        }

        protected Model Target { get; }

        protected Form Sut { get; }

        protected List<string> ValueChanges { get; }

        protected TextFormItem<string?> CreateNameItem()
        {
            return new TextFormItem<string?>(() => Target.Name);
        }

        protected TextFormItem<string?> CreateHiddenNameItem()
        {
            return new TextFormItem<string?>(() => Target.Name) { IsVisible = false };
        }
    }

    public static class Initialization
    {
        public sealed class ItemAddedToReadOnlyForm : Context
        {
            private readonly TextFormItem<string> _addedItem;

            public ItemAddedToReadOnlyForm()
            {
                var source = new ObservableCollection<string>();
                var group = new ListFormGroup<string, TextFormItem<string>>(
                    value => new TextFormItem<string>(value)
                )
                {
                    Source = source,
                };

                Sut.Add(group);
                Sut.IsReadOnly = true;

                source.Add(First);

                _addedItem = group.Cast<TextFormItem<string>>().Single();
            }

            [Fact]
            public void AppliesReadOnly()
            {
                _addedItem.IsReadOnly.Should().BeTrue();
            }

            [Fact]
            public void InitializesTarget()
            {
                _addedItem.Target.Should().BeSameAs(Target);
            }
        }

        public sealed class NameItemAdded : Context
        {
            public NameItemAdded()
            {
                Sut.Add(new LabeledFormGroup { CreateNameItem() });
            }

            [Fact]
            public void RaisesValueChangedForInitialValue()
            {
                ValueChanges.Should().Equal(nameof(Model.Name));
            }
        }

        public sealed class HiddenItemAdded : Context
        {
            private readonly TextFormItem<string?> _hiddenItem;

            public HiddenItemAdded()
            {
                _hiddenItem = CreateHiddenNameItem();

                Sut.Add(new LabeledFormGroup { _hiddenItem });
            }

            [Fact]
            public void InitializesTarget()
            {
                _hiddenItem.Target.Should().BeSameAs(Target);
            }
        }
    }

    public static class IsReadOnly
    {
        public sealed class SetAfterItemsAdded : Context
        {
            private readonly TextFormItem<string?> _visibleItem;
            private readonly TextFormItem<string?> _hiddenItem;

            public SetAfterItemsAdded()
            {
                _visibleItem = CreateNameItem();
                _hiddenItem = CreateHiddenNameItem();

                Sut.Add(new LabeledFormGroup { _visibleItem, _hiddenItem });

                Sut.IsReadOnly = true;
            }

            [Fact]
            public void AppliesToVisibleItem()
            {
                _visibleItem.IsReadOnly.Should().BeTrue();
            }

            [Fact]
            public void AppliesToHiddenItem()
            {
                _hiddenItem.IsReadOnly.Should().BeTrue();
            }
        }
    }

    public static class GetItem
    {
        public sealed class HiddenItem : Context
        {
            private readonly TextFormItem<string?> _hiddenItem;
            private readonly IFormItem? _resultByExpression;
            private readonly IFormItem? _resultByName;

            public HiddenItem()
            {
                _hiddenItem = CreateHiddenNameItem();

                Sut.Add(new LabeledFormGroup { _hiddenItem });

                _resultByExpression = Sut.GetItem(() => Target.Name);
                _resultByName = Sut[nameof(Model.Name)];
            }

            [Fact]
            public void ReturnsHiddenItemByExpression()
            {
                _resultByExpression.Should().BeSameAs(_hiddenItem);
            }

            [Fact]
            public void ReturnsHiddenItemByName()
            {
                _resultByName.Should().BeSameAs(_hiddenItem);
            }
        }
    }

    public static class ValueChanged
    {
        public sealed class TargetPropertyChanges : Context
        {
            public TargetPropertyChanges()
            {
                Sut.Add(new LabeledFormGroup { CreateNameItem() });

                ValueChanges.Clear();

                Target.Name = Changed;
            }

            [Fact]
            public void RaisesValueChangedForProperty()
            {
                ValueChanges.Should().Equal(nameof(Model.Name));
            }
        }

        public sealed class ItemRemovedFromGroup : Context
        {
            public ItemRemovedFromGroup()
            {
                var item = CreateNameItem();
                var group = new LabeledFormGroup { item };

                Sut.Add(group);

                group.Remove(item);

                ValueChanges.Clear();

                Target.Name = Changed;
            }

            [Fact]
            public void StopsRaisingValueChanged()
            {
                ValueChanges.Should().BeEmpty();
            }
        }

        public sealed class HandlerThrowsOnce : Context
        {
            private readonly Exception? _exception;

            public HandlerThrowsOnce()
            {
                Sut.Add(new LabeledFormGroup { CreateNameItem() });

                var shouldThrow = true;

                Sut.ValueChanged += (_, _) =>
                {
                    if (!shouldThrow)
                    {
                        return;
                    }

                    shouldThrow = false;

                    throw new InvalidOperationException();
                };

                _exception = Record.Exception(() => Target.Name = First);

                ValueChanges.Clear();

                Target.Name = Changed;
            }

            [Fact]
            public void PropagatesException()
            {
                _exception.Should().BeOfType<InvalidOperationException>();
            }

            [Fact]
            public void KeepsRaisingValueChanged()
            {
                ValueChanges.Should().Equal(nameof(Model.Name));
            }
        }

        public sealed class GroupRemoved : Context
        {
            public GroupRemoved()
            {
                var group = new LabeledFormGroup { CreateNameItem() };

                Sut.Add(group);
                Sut.Remove(group);

                ValueChanges.Clear();

                Target.Name = Changed;
            }

            [Fact]
            public void StopsRaisingValueChanged()
            {
                ValueChanges.Should().BeEmpty();
            }
        }

        public sealed class GroupReplaced : Context
        {
            public GroupReplaced()
            {
                Sut.Add(new LabeledFormGroup { CreateNameItem() });

                Sut[0] = new LabeledFormGroup();

                ValueChanges.Clear();

                Target.Name = Changed;
            }

            [Fact]
            public void StopsRaisingValueChangedFromReplacedGroup()
            {
                ValueChanges.Should().BeEmpty();
            }
        }
    }

    public static class Dispose
    {
        public sealed class FormWithItem : Context
        {
            private readonly IFormItem _item;

            public FormWithItem()
            {
                _item = Substitute.For<IFormItem>();
                _item.IsVisible.Returns(true);

                Sut.Add(new LabeledFormGroup { _item });

                Sut.Dispose();
            }

            [Fact]
            public void DisposesItemOnce()
            {
                _item.Received(1).Dispose();
            }
        }

        public sealed class FormWithGroup : Context
        {
            private readonly IFormGroup _group;

            public FormWithGroup()
            {
                _group = Substitute.For<IFormGroup>();

                Sut.Add(_group);

                Sut.Dispose();
            }

            [Fact]
            public void DisposesGroupOnce()
            {
                _group.Received(1).Dispose();
            }
        }
    }
}
