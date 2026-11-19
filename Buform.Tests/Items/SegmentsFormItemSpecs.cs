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

            _ = new Form(Target) { new LabeledFormGroup { Sut } };
        }

        protected Model Target { get; }

        protected SegmentsFormItem Sut { get; }
    }

    public static class Options
    {
        public sealed class OptionsReadTwice : Context
        {
            private readonly IReadOnlyList<IFormOption> _first;
            private readonly IReadOnlyList<IFormOption> _second;

            public OptionsReadTwice()
            {
                _first = Sut.Options.ToList();
                _second = Sut.Options.ToList();
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
                Sut.Options.Select(item => item.FormattedValue)
                    .Should()
                    .Equal(Prefix + First, Prefix + Second);
            }

            [Fact]
            public void RaisesOptionsChange()
            {
                _propertyChanges.Should().Contain(nameof(SegmentsFormItem.Options));
            }
        }
    }
}
