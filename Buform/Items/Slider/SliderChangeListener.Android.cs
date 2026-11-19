using Google.Android.Material.Slider;
using Object = Java.Lang.Object;

namespace Buform;

internal sealed class SliderChangeListener : Object, Slider.IOnChangeListener
{
    private readonly Action<float, bool> _onValueChange;

    public SliderChangeListener(Action<float, bool> onValueChange)
    {
        ArgumentNullException.ThrowIfNull(onValueChange);

        _onValueChange = onValueChange;
    }

    public void OnValueChange(Slider slider, float value, bool fromUser)
    {
        _onValueChange(value, fromUser);
    }
}
