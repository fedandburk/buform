using Google.Android.Material.Button;
using Object = Java.Lang.Object;

namespace Buform;

internal sealed class SegmentsButtonCheckedListener
    : Object,
        MaterialButtonToggleGroup.IOnButtonCheckedListener
{
    private readonly Action<int, bool> _onButtonChecked;

    public SegmentsButtonCheckedListener(Action<int, bool> onButtonChecked)
    {
        ArgumentNullException.ThrowIfNull(onButtonChecked);

        _onButtonChecked = onButtonChecked;
    }

    public void OnButtonChecked(MaterialButtonToggleGroup? group, int checkedId, bool isChecked)
    {
        _onButtonChecked(checkedId, isChecked);
    }
}
