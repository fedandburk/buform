using AwesomeAssertions;
using Xunit;

namespace Buform.Tests;

public static class FormItemSpecs
{
    public abstract class Context
    {
        protected const string First = "first";
        protected const string Changed = "changed";

        protected Context()
        {
            Target = new Model { Name = First };

            Sut = new TextFormItem<string?>(() => Target.Name);

            _ = new Form(Target) { new TextFormGroup { Sut } };
        }

        protected Model Target { get; }

        protected TextFormItem<string?> Sut { get; }
    }

    public static class Value
    {
        public sealed class TargetPropertyRead : Context
        {
            private readonly string? _value;

            public TargetPropertyRead()
            {
                _value = Sut.Value;
            }

            [Fact]
            public void ReturnsTargetProperty()
            {
                _value.Should().Be(First);
            }
        }

        public sealed class ValueWritten : Context
        {
            public ValueWritten()
            {
                Sut.Value = Changed;
            }

            [Fact]
            public void WritesTargetProperty()
            {
                Target.Name.Should().Be(Changed);
            }
        }
    }

    public static class Initialize
    {
        public sealed class TargetOfOtherType
        {
            private readonly Exception? _exception;

            public TargetOfOtherType()
            {
                var model = new Model();
                var item = new TextFormItem<string?>(() => model.Name);

                _exception = Record.Exception(() =>
                    new Form(new object()) { new TextFormGroup { item } }
                );
            }

            [Fact]
            public void Throws()
            {
                _exception.Should().BeOfType<ArgumentException>();
            }
        }
    }
}
