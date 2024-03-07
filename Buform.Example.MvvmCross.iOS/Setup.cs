using Buform.Example.Core;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.IoC;
using MvvmCross.Platforms.Ios.Core;
using Serilog;
using Serilog.Extensions.Logging;

namespace Buform.Example.MvvmCross.iOS;

[Preserve(AllMembers = true)]
public sealed class Setup : MvxIosSetup<Application>
{
    protected override ILoggerProvider CreateLogProvider()
    {
        return new SerilogLoggerProvider();
    }

    protected override ILoggerFactory CreateLogFactory()
    {
        Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.NSLog().CreateLogger();

        return new SerilogLoggerFactory();
    }

    protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
    {
        base.InitializeFirstChance(iocProvider);

        iocProvider.LazyConstructAndRegisterSingleton<IMvxCommandHelper, MvxStrongCommandHelper>();
    }

    protected override void InitializeLastChance(IMvxIoCProvider iocProvider)
    {
        base.InitializeLastChance(iocProvider);

        FormPlatform.RegisterGroupHeaderNib<HeaderFormGroup, HeaderFormGroupHeader>();

        FormPlatform.RegisterItemClass<RandomNumberGeneratorItem, RandomNumberGeneratorCell>();
    }
}
