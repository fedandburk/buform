using Android.Runtime;
using Buform.Example.Core;
using Microsoft.Extensions.Logging;
using MvvmCross.IoC;
using MvvmCross.Platforms.Android.Core;
using Serilog;
using Serilog.Extensions.Logging;

namespace Buform.Example.MvvmCross.Droid;

[Preserve(AllMembers = true)]
public sealed class Setup : MvxAndroidSetup<Core.Application>
{
    protected override ILoggerProvider CreateLogProvider()
    {
        return new SerilogLoggerProvider();
    }

    protected override ILoggerFactory CreateLogFactory()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.AndroidLog()
            .CreateLogger();

        return new SerilogLoggerFactory();
    }

    protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
    {
        base.InitializeFirstChance(iocProvider);

        FormPlatform.RegisterGroupHeader<HeaderFormGroup, HeaderFormGroupHeaderViewHolder>(
            Resource.Layout.FormGroupTextHeaderLayout
        );

        FormPlatform.RegisterGroupFooter<HeaderFormGroup, HeaderFormGroupFooterViewHolder>(
            Resource.Layout.FormGroupTextFooterLayout
        );
    }
}
