using Microsoft.Maui.Handlers;
using View = Android.Views.View;

namespace Buform;

internal sealed class FormViewHandler : ViewHandler<FormView, View>
{
    public static readonly PropertyMapper<FormView, FormViewHandler> PropertyMapper =
        new(ViewMapper)
        {
            [nameof(FormView.Form)] = MapForm
        };

    public FormViewHandler() : base(PropertyMapper)
    {
    }

    private static void MapForm(FormViewHandler handler, FormView view)
    {
    }

    protected override View CreatePlatformView()
    {
        return new View(Context);
    }
}