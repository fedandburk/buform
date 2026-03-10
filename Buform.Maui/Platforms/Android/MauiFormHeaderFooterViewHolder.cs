using Android.Views;
using Android.Widget;
using Microsoft.Maui.Platform;
using AndroidX.RecyclerView.Widget;
using AView = Android.Views.View;

namespace Buform;

internal sealed class MauiFormHeaderFooterViewHolder : RecyclerView.ViewHolder
{
    private readonly FrameLayout _container;
    private FormHeaderFooterView? _formHeaderFooterView;
    private AView? _platformView;

    public MauiFormHeaderFooterViewHolder(FrameLayout container)
        : base(container)
    {
        _container = container;
    }

    public void Bind(object section, Type viewType)
    {
        _container.RemoveAllViews();

        _formHeaderFooterView = (Activator.CreateInstance(viewType) as FormHeaderFooterView)!;
        _formHeaderFooterView.BindingContext = section;

        var mauiContext = Application.Current?.Handler?.MauiContext;
        if (mauiContext == null)
        {
            return;
        }

        _platformView = _formHeaderFooterView.ToPlatform(mauiContext);

        _container.AddView(
            _platformView,
            new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent));
    }
}