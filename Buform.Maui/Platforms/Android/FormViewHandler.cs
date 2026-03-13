using AndroidX.RecyclerView.Widget;
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
        var recyclerView = new FormRecyclerView(Context);

        recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
        recyclerView.SetClipToPadding(false);

        var density = Context.Resources?.DisplayMetrics?.Density ?? 1f;
        int Dp(int value) => (int)(value * density);

        recyclerView.SetPadding(0, Dp(8), 0, Dp(16));

        var adapter = new MauiFormRecyclerAdapter(Context, MauiContext!);
        recyclerView.SetAdapter(adapter);

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
            adapter.SetForm(null);
        }

        platformView.SetAdapter(null);

        base.DisconnectHandler(platformView);
    }

    private static void MapForm(FormViewHandler handler, FormView view)
    {
        handler.UpdateForm();
    }

    private void UpdateForm()
    {
        if (PlatformView.GetAdapter() is not MauiFormRecyclerAdapter adapter)
        {
            return;
        }

        adapter.SetForm(VirtualView.Form);
    }
}
