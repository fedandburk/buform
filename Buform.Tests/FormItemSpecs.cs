using System.Reflection;
using AwesomeAssertions;
using Xunit;

namespace Buform.Tests;

public static class FormItemSpecs
{
    public abstract class Context
    {
        protected const string First = "first";
        protected const string Changed = "changed";

        protected Context(object target)
        {
            Model = new Model();
            Sut = new TextFormItem<string?>(() => Model.Name);

            _ = new Form(target) { new TextFormGroup { Sut } };
        }

        protected Model Model { get; }

        protected TextFormItem<string?> Sut { get; }
    }

    public static class Value
    {
        public sealed class TargetPropertyRead : Context
        {
            private readonly Model _target;
            private readonly string? _value;

            public TargetPropertyRead()
                : this(new Model { Name = First }) { }

            private TargetPropertyRead(Model target)
                : base(target)
            {
                _target = target;

                _value = Sut.Value;
            }

            [Fact]
            public void ReturnsTargetProperty()
            {
                _value.Should().Be(_target.Name);
            }
        }

        public sealed class ValueWritten : Context
        {
            private readonly Model _target;

            public ValueWritten()
                : this(new Model()) { }

            private ValueWritten(Model target)
                : base(target)
            {
                _target = target;

                Sut.Value = Changed;
            }

            [Fact]
            public void WritesTargetProperty()
            {
                _target.Name.Should().Be(Changed);
            }
        }

        public sealed class TargetOfOtherType : Context
        {
            private readonly Exception? _exception;

            public TargetOfOtherType()
                : base(new object())
            {
                _exception = Record.Exception(() => Sut.Value);
            }

            [Fact]
            public void Throws()
            {
                _exception.Should().BeOfType<TargetException>();
            }
        }
    }
}
