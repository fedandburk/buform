using AwesomeAssertions;
using NSubstitute;
using Xunit;

namespace Buform.Tests.Validation;

public static class ValidatableFormSpecs
{
    public sealed class ErrorsForm : ValidatableForm
    {
        public ErrorsForm(object target)
            : base(target)
        {
            /* Required constructor */
        }

        public void Apply(params FormValidationError[] validationErrors)
        {
            SetValidationErrors(false, validationErrors);
        }

        public override bool PerformValidation()
        {
            return true;
        }

        public override bool PerformForcedValidation()
        {
            return true;
        }

        public override void ResetValidation()
        {
            /* No validation state */
        }
    }

    public abstract class Context
    {
        protected const string VisibleProperty = "Visible";
        protected const string HiddenProperty = "Hidden";
        protected const string ErrorMessage = "error";

        protected Context()
        {
            VisibleItem = CreateItem(VisibleProperty, true);
            HiddenItem = CreateItem(HiddenProperty, false);

            Sut = new ErrorsForm(new Model())
            {
                new TextFormGroup { VisibleItem, HiddenItem },
            };
        }

        protected IValidatableFormItem VisibleItem { get; }

        protected IValidatableFormItem HiddenItem { get; }

        protected ErrorsForm Sut { get; }

        private static IValidatableFormItem CreateItem(string propertyName, bool isVisible)
        {
            var item = Substitute.For<IValidatableFormItem>();

            item.PropertyName.Returns(propertyName);
            item.IsVisible.Returns(isVisible);

            return item;
        }
    }

    public static class SetValidationErrors
    {
        public sealed class ErrorsForVisibleAndHiddenItems : Context
        {
            public ErrorsForVisibleAndHiddenItems()
            {
                Sut.Apply(
                    new FormValidationError(VisibleProperty, ErrorMessage),
                    new FormValidationError(HiddenProperty, ErrorMessage)
                );
            }

            [Fact]
            public void SetsErrorOnVisibleItem()
            {
                VisibleItem.Received(1).SetValidationError(Arg.Any<FormValidationError?>(), false);
            }

            [Fact]
            public void SkipsHiddenItem()
            {
                HiddenItem.DidNotReceiveWithAnyArgs().SetValidationError(default, default);
            }
        }
    }
}
