using AwesomeAssertions;
using Xunit;

namespace Buform.Tests;

public static class FormItemSpecs
{
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

    public static class ValueOfValueConstructedItem
    {
        public sealed class ValueWritten
        {
            private const string Initial = "initial";
            private const string Changed = "changed";

            private readonly TextFormItem<string?> _sut;
            private readonly List<string?> _propertyChanges;

            public ValueWritten()
            {
                var target = new Model();

                _sut = new TextFormItem<string?>(Initial);

                _ = new Form(target) { new LabeledFormGroup { _sut } };

                _propertyChanges = [];

                _sut.PropertyChanged += (_, args) => _propertyChanges.Add(args.PropertyName);

                _sut.Value = Changed;
            }

            [Fact]
            public void ReturnsWrittenValue()
            {
                _sut.Value.Should().Be(Changed);
            }

            [Fact]
            public void RaisesValueChange()
            {
                _propertyChanges.Should().Contain(nameof(TextFormItem<string?>.Value));
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
                    new Form(new object()) { new LabeledFormGroup { item } }
                );
            }

            [Fact]
            public void Throws()
            {
                _exception.Should().BeOfType<ArgumentException>();
            }
        }
    }

    public abstract class Context
    {
        protected const string First = "first";
        protected const string Changed = "changed";

        protected Context()
        {
            Target = new Model { Name = First };

            Sut = new TextFormItem<string?>(() => Target.Name);

            _ = new Form(Target) { new LabeledFormGroup { Sut } };
        }

        protected Model Target { get; }

        protected TextFormItem<string?> Sut { get; }
    }
}
