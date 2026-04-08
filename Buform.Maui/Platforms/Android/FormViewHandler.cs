using Microsoft.Maui.Handlers;

namespace Buform;

internal sealed class FormViewHandler : ViewHandler<FormView, FormRecyclerView>
{
    public static readonly PropertyMapper<FormView, FormViewHandler> PropertyMapper =
        new(ViewMapper) { [nameof(FormView.Form)] = MapForm, };

    public FormViewHandler()
        : base(PropertyMapper) { }

    protected override FormRecyclerView CreatePlatformView()
    {
        if (MauiContext is null)
        {
            throw new InvalidOperationException("MauiContext is not available.");
        }

        var recyclerView = new FormRecyclerView(Context);

        recyclerView.SetClipToPadding(false);
        recyclerView.SetPadding(0, Dp(8), 0, Dp(16));

        recyclerView.SetAdapter(new MauiFormRecyclerAdapter(Context, MauiContext));

        return recyclerView;
    }

    protected override void ConnectHandler(FormRecyclerView platformView)
    {
        base.ConnectHandler(platformView);
        UpdateForm();
    }

    protected override void DisconnectHandler(FormRecyclerView platformView)
    {
        if (platformView.GetAdapter() is MauiFormRecyclerAdapter adapter)
        {
            adapter.Form = null;
        }

        base.DisconnectHandler(platformView);
    }

    private static void MapForm(FormViewHandler handler, FormView view)
    {
        handler.UpdateForm();
    }

    private void UpdateForm()
    {
        if (PlatformView?.GetAdapter() is MauiFormRecyclerAdapter adapter)
        {
            adapter.Form = VirtualView.Form;
        }
    }

    private int Dp(int value)
    {
        var density = Context.Resources?.DisplayMetrics?.Density ?? 1f;
        return (int)(value * density);
    }
}
