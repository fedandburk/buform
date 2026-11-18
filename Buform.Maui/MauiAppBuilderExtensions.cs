namespace Buform;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseBuform(this MauiAppBuilder builder)
    {
        MauiFormPlatform.Initialize(builder);

        return builder;
    }
}
