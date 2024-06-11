using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(PrefixButtonFormCell))]
public class PrefixButtonFormCell : ButtonFormCell<PrefixButtonFormItem>
{
    public PrefixButtonFormCell()
    {
        /* Required constructor */
    }

    public PrefixButtonFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected override void OnItemPropertyChanged(string? propertyName)
    {
        base.OnItemPropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(Item.Prefix):
                UpdateLabel();
                break;
        }
    }

    protected override void UpdateLabel()
    {
        base.UpdateLabel();

        Label.Text = Item.Prefix + Label.Text;
    }
}
