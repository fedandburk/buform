using Microsoft.Maui.Handlers;
using AndroidX.RecyclerView.Widget;

namespace Buform;

internal sealed class FormViewHandler : ViewHandler<FormView, RecyclerView>
{
    public static readonly PropertyMapper<FormView, FormViewHandler> PropertyMapper =
        new(ViewMapper)
        {
            [nameof(FormView.Form)] = MapForm
        };

    public FormViewHandler() : base(PropertyMapper)
    {
    }

    protected override RecyclerView CreatePlatformView()
    {
        var recyclerView = new RecyclerView(Context);

        recyclerView.SetLayoutManager(new LinearLayoutManager(Context));

        var adapter = new MauiFormRecyclerAdapter(Context);
        recyclerView.SetAdapter(adapter);

        return recyclerView;
    }

    protected override void ConnectHandler(RecyclerView platformView)
    {
        base.ConnectHandler(platformView);
        UpdateForm();
    }

    protected override void DisconnectHandler(RecyclerView platformView)
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
        if (PlatformView?.GetAdapter() is not MauiFormRecyclerAdapter adapter)
        {
            return;
        }

        adapter.SetForm(VirtualView?.Form);
    }
}

