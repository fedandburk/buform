using Fedandburk.Common.Extensions;
using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(RandomNumberGeneratorCell))]
// ReSharper disable once ClassNeverInstantiated.Global
public sealed class RandomNumberGeneratorCell : TextFormCell<RandomNumberGeneratorItem>
{
    private UIButton? _button;

    // ReSharper disable once UnusedMember.Global
    public RandomNumberGeneratorCell()
    {
        /* Required constructor */
    }

    // ReSharper disable once UnusedMember.Global
    public RandomNumberGeneratorCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    private void OnTouchUpInside(object? _, EventArgs __)
    {
        Item?.GenerateCommand.SafeExecute();
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
