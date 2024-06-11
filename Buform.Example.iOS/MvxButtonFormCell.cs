using Fedandburk.Common.Extensions;
using ObjCRuntime;

namespace Buform;

[Preserve(AllMembers = true)]
[Register(nameof(MvxButtonFormCell))]
public sealed class MvxButtonFormCell : MvxFormCell<MvxButtonFormItem>
{
    private UILabel? _label;

    public override bool IsSelectable => !Item?.IsReadOnly ?? false;

    public MvxButtonFormCell()
    {
        /* Required constructor */
    }

    public MvxButtonFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected override void Initialize()
    {
        _label = new UILabel
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Font = UIFont.PreferredBody,
            TextColor = UIColor.Label
        };

        ContentView.AddSubviews(_label);

        ContentView.AddConstraints(
            [
                _label.TopAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.TopAnchor),
                _label.BottomAnchor.ConstraintEqualTo(ContentView.LayoutMarginsGuide.BottomAnchor),
                _label.LeadingAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.LeadingAnchor
                ),
                _label.TrailingAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.TrailingAnchor
                )
            ]
        );
    }

    protected override void InitializeBindings()
    {
        var set = CreateBindingSet();
        set.Bind(_label).For(v => v.Text).To(vm => vm.Label);
        set.Apply();
    }

    public override void OnSelected()
    {
        base.OnSelected();

        Item?.Value.SafeExecute();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _label?.Dispose();
            _label = null;
        }

        base.Dispose(disposing);
    }
}
