using AwesomeAssertions;
using Xunit;

namespace Buform.Tests;

public static class AsyncPickerFormItemSpecs
{
    public static class LoadOptionsAsync
    {
        public sealed class SourceLoaded : Context, IAsyncLifetime
        {
            public async ValueTask InitializeAsync()
            {
                Sut.SourceFactory = _ => Task.FromResult<IEnumerable<string?>>([First, Second]);

                await Sut.LoadOptionsAsync(CancellationToken.None);
            }

            public ValueTask DisposeAsync()
            {
                return ValueTask.CompletedTask;
            }

            [Fact]
            public void SetsLoadedState()
            {
                Sut.State.Should().Be(AsyncPickerLoadingState.Loaded);
            }

            [Fact]
            public void ExposesSourceAsOptions()
            {
                Sut.Options.Select(option => option.Value).Should().Equal(First, Second);
            }
        }

        public sealed class SourceFailed : Context, IAsyncLifetime
        {
            private readonly InvalidOperationException _exception;

            public SourceFailed()
            {
                _exception = new InvalidOperationException();
            }

            public async ValueTask InitializeAsync()
            {
                Sut.SourceFactory = _ => Task.FromException<IEnumerable<string?>>(_exception);

                await Sut.LoadOptionsAsync(CancellationToken.None);
            }

            public ValueTask DisposeAsync()
            {
                return ValueTask.CompletedTask;
            }

            [Fact]
            public void SetsFailedState()
            {
                Sut.State.Should().Be(AsyncPickerLoadingState.Failed);
            }

            [Fact]
            public void KeepsLoadException()
            {
                Sut.LoadException.Should().BeSameAs(_exception);
            }
        }

        public sealed class LoadingCancelled : Context, IAsyncLifetime
        {
            private Exception? _exception;

            public async ValueTask InitializeAsync()
            {
                using var cancellationTokenSource = new CancellationTokenSource();

                Sut.SourceFactory = async cancellationToken =>
                {
                    await cancellationTokenSource.CancelAsync();

                    cancellationToken.ThrowIfCancellationRequested();

                    return [First];
                };

                try
                {
                    await Sut.LoadOptionsAsync(cancellationTokenSource.Token);
                }
                catch (Exception exception)
                {
                    _exception = exception;
                }
            }

            public ValueTask DisposeAsync()
            {
                return ValueTask.CompletedTask;
            }

            [Fact]
            public void ThrowsOperationCanceledException()
            {
                _exception.Should().BeAssignableTo<OperationCanceledException>();
            }

            [Fact]
            public void ResetsState()
            {
                Sut.State.Should().Be(AsyncPickerLoadingState.None);
            }
        }

        public sealed class LoadingCancelledThenRepeated : Context, IAsyncLifetime
        {
            public async ValueTask InitializeAsync()
            {
                using var cancellationTokenSource = new CancellationTokenSource();

                await cancellationTokenSource.CancelAsync();

                Sut.SourceFactory = cancellationToken =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    return Task.FromResult<IEnumerable<string?>>([First]);
                };

                try
                {
                    await Sut.LoadOptionsAsync(cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    /* Expected cancellation */
                }

                await Sut.LoadOptionsAsync(CancellationToken.None);
            }

            public ValueTask DisposeAsync()
            {
                return ValueTask.CompletedTask;
            }

            [Fact]
            public void SetsLoadedState()
            {
                Sut.State.Should().Be(AsyncPickerLoadingState.Loaded);
            }
        }
    }

    public abstract class Context
    {
        protected const string First = "first";
        protected const string Second = "second";

        protected Context()
        {
            Target = new Model();

            Sut = new AsyncPickerFormItem<string?>(() => Target.Name);

            _ = new Form(Target) { new LabeledFormGroup { Sut } };
        }

        protected Model Target { get; }

        protected AsyncPickerFormItem<string?> Sut { get; }
    }
}
