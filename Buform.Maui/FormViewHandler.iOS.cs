using CoreGraphics;
using Microsoft.Maui.Handlers;
using UIKit;

namespace Buform;

internal sealed class FormViewHandler : ViewHandler<FormView, UITableView>
{
    private static readonly PropertyMapper<FormView, FormViewHandler> PropertyMapper = new(
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

    private MauiFormTableViewSource? _source;

    private static void MapForm(FormViewHandler handler, FormView view)
    {
        if (handler._source != null)
        {
            handler._source.Form = view.Form;
        }
    }

    protected override void ConnectHandler(UITableView platformView)
    {
        base.ConnectHandler(platformView);

        if (_source != null)
        {
            _source.Form = VirtualView.Form;
        }
    }

    protected override void DisconnectHandler(UITableView platformView)
    {
        _source?.Dispose();
        _source = null;

        base.DisconnectHandler(platformView);
    }

    protected override UITableView CreatePlatformView()
    {
        var tableView = new UITableView(CGRect.Empty, UITableViewStyle.InsetGrouped);

        _source = new MauiFormTableViewSource(tableView);

        tableView.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.Interactive;

        return tableView;
    }
}
