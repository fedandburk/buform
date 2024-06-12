using Microsoft.Maui.Controls.Internals;

namespace Buform;

[Preserve(AllMembers = true)]
public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseBuform(this MauiAppBuilder builder)
    {
        MauiFormPlatform.Initialize(builder);

        return builder;
    }
}
