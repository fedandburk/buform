using System.Windows.Input;
using AwesomeAssertions;
using Buform.Extensions;
using NSubstitute;
using Xunit;

namespace Buform.Tests.Extensions;

public static class CommandExtensionsSpecs
{
    public abstract class Context
    {
        protected const string Parameter = "parameter";

        protected Context(bool canExecute)
        {
            Command = Substitute.For<ICommand>();
            Command.CanExecute(Parameter).Returns(canExecute);
        }

        protected ICommand Command { get; }
    }

    public static class SafeCanExecute
    {
        public sealed class NullCommand
        {
            private readonly bool _result;

            public NullCommand()
            {
                ICommand? command = null;

                _result = command.SafeCanExecute();
            }

            [Fact]
            public void ReturnsFalse()
            {
                _result.Should().BeFalse();
            }
        }

        public sealed class AllowedCommand : Context
        {
            private readonly bool _result;

            public AllowedCommand()
                : base(true)
            {
                _result = Command.SafeCanExecute(Parameter);
            }

            [Fact]
            public void ReturnsTrue()
            {
                _result.Should().BeTrue();
            }
        }
    }

    public static class SafeExecute
    {
        public sealed class AllowedCommand : Context
        {
            public AllowedCommand()
                : base(true)
            {
                Command.SafeExecute(Parameter);
            }

            [Fact]
            public void ExecutesCommand()
            {
                Command.Received(1).Execute(Parameter);
            }
        }

        public sealed class BlockedCommand : Context
        {
            public BlockedCommand()
                : base(false)
            {
                Command.SafeExecute(Parameter);
            }

            [Fact]
            public void DoesNotExecuteCommand()
            {
                Command.DidNotReceiveWithAnyArgs().Execute(default);
            }
        }

        public sealed class NullCommand
        {
            private readonly Exception? _exception;

            public NullCommand()
            {
                ICommand? command = null;

                _exception = Record.Exception(() => command.SafeExecute());
            }

            [Fact]
            public void DoesNotThrow()
            {
                _exception.Should().BeNull();
            }
        }
    }
}
