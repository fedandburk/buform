using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using NSubstitute;
using Xunit;

namespace Buform.FluentValidation.Tests;

public static class FluentValidationFormSpecs
{
    public static class Validate
    {
        public sealed class TwoFailuresForOneProperty : Context
        {
            public TwoFailuresForOneProperty()
            {
                Validator
                    .Validate(Target)
                    .Returns(
                        new ValidationResult([
                            new ValidationFailure(nameof(Model.Name), FirstMessage),
                            new ValidationFailure(nameof(Model.Name), SecondMessage),
                        ])
                    );

                Sut.Add(new LabeledFormGroup { Item });
            }

            [Fact]
            public void JoinsMessagesIntoOneError()
            {
                Item.Received()
                    .SetValidationError(
                        Arg.Is<FormValidationError>(error =>
                            error.ErrorMessage == FirstMessage + Environment.NewLine + SecondMessage
                        ),
                        false
                    );
            }

            [Fact]
            public void IsInvalid()
            {
                Sut.IsValid.Should().BeFalse();
            }
        }
    }

    public abstract class Context
    {
        protected const string FirstMessage = "first";
        protected const string SecondMessage = "second";

        protected Context()
        {
            Target = new Model();

            Validator = Substitute.For<IValidator<Model>>();

            Item = Substitute.For<IValidatableFormItem>();
            Item.PropertyName.Returns(nameof(Model.Name));
            Item.IsVisible.Returns(true);
            Item.When(item => item.Initialize(Arg.Any<Form>(), Arg.Any<object>()))
                .Do(_ =>
                    Item.ValueChanged += Raise.EventWith(
                        Item,
                        new FormValueChangedEventArgs(nameof(Model.Name))
                    )
                );

            Sut = new FluentValidationForm<Model>(Target, Validator);
        }

        protected Model Target { get; }

        protected IValidator<Model> Validator { get; }

        protected IValidatableFormItem Item { get; }

        protected FluentValidationForm<Model> Sut { get; }
    }
}
