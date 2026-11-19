using System.ComponentModel;
using AwesomeAssertions;
using NSubstitute;
using Xunit;

namespace Buform.Tests;

public static class ValidatableFormSpecs
{
    public sealed class ErrorsForm : ValidatableForm
    {
        public ErrorsForm(object target)
            : base(target)
        {
            Errors = [];
        }

        public List<FormValidationError> Errors { get; }

        protected override IReadOnlyList<FormValidationError> GetValidationErrors()
        {
            return Errors;
        }
    }

    public abstract class Context
    {
        protected const string VisibleProperty = "Visible";
        protected const string HiddenProperty = "Hidden";
        protected const string ErrorMessage = "error";

        protected Context(params string[] invalidProperties)
        {
            VisibleError = new FormValidationError(VisibleProperty, ErrorMessage);
            HiddenError = new FormValidationError(HiddenProperty, ErrorMessage);

            VisibleItem = CreateItem(VisibleProperty, true);
            HiddenItem = CreateItem(HiddenProperty, false);

            Sut = new ErrorsForm(new Model());
            Sut.Errors.AddRange(
                new[] { VisibleError, HiddenError }.Where(error =>
                    invalidProperties.Contains(error.PropertyName)
                )
            );

            PropertyChanges = [];

            ((INotifyPropertyChanged)Sut).PropertyChanged += (_, args) =>
                PropertyChanges.Add(args.PropertyName);

            Sut.Add(new LabeledFormGroup { VisibleItem, HiddenItem });
        }

        protected FormValidationError VisibleError { get; }

        protected FormValidationError HiddenError { get; }

        protected IValidatableFormItem VisibleItem { get; }

        protected IValidatableFormItem HiddenItem { get; }

        protected ErrorsForm Sut { get; }

        protected List<string?> PropertyChanges { get; }

        private static IValidatableFormItem CreateItem(string propertyName, bool isVisible)
        {
            var item = Substitute.For<IValidatableFormItem>();

            item.PropertyName.Returns(propertyName);
            item.IsVisible.Returns(isVisible);
            item.When(substitute => substitute.Initialize(Arg.Any<Form>(), Arg.Any<object>()))
                .Do(_ =>
                    item.ValueChanged += Raise.EventWith(
                        item,
                        new FormValueChangedEventArgs(propertyName)
                    )
                );

            return item;
        }
    }

    public static class Initialization
    {
        public sealed class InvalidItemsAdded : Context
        {
            public InvalidItemsAdded()
                : base(VisibleProperty) { }

            [Fact]
            public void IsInvalid()
            {
                Sut.IsValid.Should().BeFalse();
            }

            [Fact]
            public void SetsErrorOnItem()
            {
                VisibleItem.Received().SetValidationError(VisibleError, false);
            }
        }

        public sealed class ValidItemsAdded : Context
        {
            [Fact]
            public void IsValid()
            {
                Sut.IsValid.Should().BeTrue();
            }

            [Fact]
            public void RaisesNoValidityChange()
            {
                PropertyChanges.Should().NotContain(nameof(Sut.IsValid));
            }
        }
    }

    public static class Validate
    {
        public sealed class HiddenItemInvalid : Context
        {
            public HiddenItemInvalid()
                : base(HiddenProperty)
            {
                Sut.Validate();
            }

            [Fact]
            public void SetsErrorOnHiddenItem()
            {
                HiddenItem.Received().SetValidationError(HiddenError, false);
            }

            [Fact]
            public void ClearsErrorOnVisibleItem()
            {
                VisibleItem.Received().SetValidationError(null, false);
            }
        }
    }

    public static class ValidateAll
    {
        public sealed class VisibleItemInvalid : Context
        {
            private readonly bool _result;

            public VisibleItemInvalid()
                : base(VisibleProperty)
            {
                _result = Sut.ValidateAll();
            }

            [Fact]
            public void ReturnsFalse()
            {
                _result.Should().BeFalse();
            }

            [Fact]
            public void ShowsErrorOfUnchangedItem()
            {
                VisibleItem.Received().SetValidationError(VisibleError, true);
            }
        }
    }

    public static class ResetValidation
    {
        public sealed class HiddenItemInvalid : Context
        {
            public HiddenItemInvalid()
                : base(HiddenProperty)
            {
                Sut.ResetValidation();
            }

            [Fact]
            public void ResetsHiddenItem()
            {
                HiddenItem.Received(1).ResetValidationError();
            }

            [Fact]
            public void KeepsFormInvalid()
            {
                Sut.IsValid.Should().BeFalse();
            }
        }
    }
}
