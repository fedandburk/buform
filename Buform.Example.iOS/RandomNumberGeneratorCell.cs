// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

using ObjCRuntime;

namespace Buform;

[Register(nameof(RandomNumberGeneratorCell))]
public sealed class RandomNumberGeneratorCell : TextFormCell<RandomNumberGeneratorItem>
{
    private UIButton? _button;

    public RandomNumberGeneratorCell()
    {
        /* Required constructor */
    }

    public RandomNumberGeneratorCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    private void OnTouchUpInside(object? _, EventArgs __)
    {
        Item?.GenerateCommand.Execute(null);
    }

    protected override void Initialize()
    {
        base.Initialize();

        SelectionStyle = UITableViewCellSelectionStyle.None;

        _button = UIButton.FromType(UIButtonType.System);
        _button.TouchUpInside += OnTouchUpInside;
        _button.SetTitle("Generate random number", UIControlState.Normal);
        _button.SizeToFit();

        AccessoryView = _button;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        var button = _button;
        if (button != null)
        {
            button.TouchUpInside -= OnTouchUpInside;
        }
    }
}
