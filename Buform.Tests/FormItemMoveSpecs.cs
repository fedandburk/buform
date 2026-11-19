using AwesomeAssertions;
using Xunit;

namespace Buform.Tests;

public static class FormItemMoveSpecs
{
    public abstract class Context
    {
        protected const string First = "first";
        protected const string Hidden = "hidden";
        protected const string Second = "second";
        protected const string Third = "third";

        protected Context()
        {
            Group = new TextFormGroup
            {
                new TextFormItem<string>(First),
                new TextFormItem<string>(Hidden) { IsVisible = false },
                new TextFormItem<string>(Second),
                new TextFormItem<string>(Third),
            };
        }

        protected TextFormGroup Group { get; }
    }

    public static class Create
    {
        public sealed class FirstRowMovedToLast : Context
        {
            private readonly FormItemMove _move;

            public FirstRowMovedToLast()
            {
                _move = FormItemMove.Create(Group, 0, 2);
            }

            [Fact]
            public void CarriesMovedValue()
            {
                _move.Value.Should().Be(First);
            }

            [Fact]
            public void UsesSourceIndices()
            {
                (_move.OldIndex, _move.NewIndex).Should().Be((0, 3));
            }
        }

        public sealed class LastRowMovedToFirst : Context
        {
            private readonly FormItemMove _move;

            public LastRowMovedToFirst()
            {
                _move = FormItemMove.Create(Group, 2, 0);
            }

            [Fact]
            public void UsesSourceIndices()
            {
                (_move.OldIndex, _move.NewIndex).Should().Be((3, 0));
            }
        }
    }
}
