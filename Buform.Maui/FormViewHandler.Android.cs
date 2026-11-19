using AndroidX.RecyclerView.Widget;
using Microsoft.Maui.Handlers;

namespace Buform;

internal sealed class FormViewHandler : ViewHandler<FormView, RecyclerView>
{
    public static readonly PropertyMapper<FormView, FormViewHandler> PropertyMapper = new(
        ViewMapper
    )
    {
        [nameof(FormView.Form)] = MapForm,
    };

    public FormViewHandler()
        : base(PropertyMapper)
    {
        /* Required constructor */
    }

    protected override RecyclerView CreatePlatformView()
    {
        if (MauiContext == null)
        {
            throw new InvalidOperationException("MauiContext is not available.");
        }

        var recyclerView = new RecyclerView(Context);

        recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
        recyclerView.SetAdapter(new MauiFormRecyclerAdapter(MauiContext));

        FormRowAppearance.ApplyFormStyle(recyclerView);

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
}
