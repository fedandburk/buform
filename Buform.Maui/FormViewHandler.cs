using CoreGraphics;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Buform;

public class FormViewHandler : ViewHandler<FormView, UITableView>
{
    private static readonly PropertyMapper<FormView, FormViewHandler> PropertyMapper =
        new(ViewMapper) { [nameof(FormView.Form)] = MapForm };

    public FormViewHandler()
        : base(PropertyMapper)
    {
        /* Required constructor */
    }

    private static void MapForm(FormViewHandler handler, FormView view)
    {
        if (handler.PlatformView.Source is MauiFormTableViewSource source)
        {
            source.Form = view.Form;
        }
    }

    protected override void ConnectHandler(UITableView platformView)
    {
        base.ConnectHandler(platformView);

        if (PlatformView.Source is MauiFormTableViewSource source)
        {
            source.Form = VirtualView.Form;
        }
    }

    protected override UITableView CreatePlatformView()
    {
        var tableView = new UITableView(CGRect.Empty, UITableViewStyle.InsetGrouped);

        var source = new MauiFormTableViewSource(tableView);

        tableView.Source = source;
        tableView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;

        return tableView;
    }
}
