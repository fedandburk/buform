using CommunityToolkit.Maui;

namespace Buform;

public static class MauiProgram
{
    private static MauiAppBuilder RegisterServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<INavigationService, AppShell>();

        return builder;
    }

    private static MauiAppBuilder RegisterRoutes(this MauiAppBuilder builder)
    {
        builder.Services.AddSingletonWithShellRoute<MenuPage, MenuViewModel>(nameof(MenuPage));
        builder.Services.AddSingletonWithShellRoute<ComponentsPage, ComponentsViewModel>(
            nameof(ComponentsPage)
        );
        builder.Services.AddSingletonWithShellRoute<
            CreateConnectionPage,
            CreateConnectionViewModel
        >(nameof(CreateConnectionPage));
        builder.Services.AddSingletonWithShellRoute<CreateEventPage, CreateEventViewModel>(
            nameof(CreateEventPage)
        );

        return builder;
    }

    public static MauiApp CreateMauiApp()
    {
        MauiFormPlatform.RegisterGroupHeader<LogoFormGroup, LogoHeaderView>();
        MauiFormPlatform.RegisterItem<RandomNumberGeneratorItem, RandomNumberGeneratorView>();

        return MauiApp
            .CreateBuilder()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseBuform()
            .RegisterServices()
            .RegisterRoutes()
            .Build();
    }
}
