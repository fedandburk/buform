using AwesomeAssertions;
using Xunit;

namespace Buform.Tests;

public static class SegmentsFormItemSpecs
{
    public abstract class Context
    {
        protected const string First = "first";
        protected const string Second = "second";
        protected const string Prefix = "formatted ";

        protected Context()
        {
            Target = new Model();

            Sut = new SegmentsFormItem(() => Target.Name) { Source = [First, Second] };

            _ = new Form(Target) { new TextFormGroup { Sut } };
        }

        protected Model Target { get; }

        protected SegmentsFormItem Sut { get; }
    }

    public static class Items
    {
        public sealed class ItemsReadTwice : Context
        {
            private readonly IReadOnlyList<SegmentsOptionFormItem<string?>> _first;
            private readonly IReadOnlyList<SegmentsOptionFormItem<string?>> _second;

            public ItemsReadTwice()
            {
                _first = Sut.Items.ToList();
                _second = Sut.Items.ToList();
            }

            [Fact]
            public void ReturnsSameOptions()
            {
                _second.Should().Equal(_first, ReferenceEquals);
            }
        }
    }

    public static class Formatter
    {
        public sealed class FormatterSetAfterSource : Context
        {
            private readonly List<string?> _propertyChanges;

            public FormatterSetAfterSource()
            {
                _propertyChanges = [];

                Sut.PropertyChanged += (_, args) => _propertyChanges.Add(args.PropertyName);

                Sut.Formatter = value => Prefix + value;
            }

            [Fact]
            public void FormatsOptions()
            {
                Sut.Items.Select(item => item.FormattedValue)
                    .Should()
                    .Equal(Prefix + First, Prefix + Second);
            }

            [Fact]
            public void RaisesItemsChange()
            {
                _propertyChanges.Should().Contain(nameof(SegmentsFormItem.Items));
            }
        }
    }
}
