using System.Runtime.Versioning;
using ObjCRuntime;

namespace Buform;

public abstract class PopUpPickerFormCell<TItem> : PresentedPickerFormCell<TItem>
    where TItem : class, IPickerFormItem
{
    private const int PopUpIndicatorMinimumVersion = 16;

    [SupportedOSPlatformGuard("ios16.0")]
    [SupportedOSPlatformGuard("maccatalyst16.0")]
    private static bool IsPopUpIndicatorSupported =>
        OperatingSystem.IsIOSVersionAtLeast(PopUpIndicatorMinimumVersion)
        || OperatingSystem.IsMacCatalystVersionAtLeast(PopUpIndicatorMinimumVersion);

    protected virtual UIButton? MenuButton { get; set; }

    protected virtual bool IsPopUp => Item?.InputType == PickerInputType.PopUp;

    public override bool IsSelectable => !IsPopUp && base.IsSelectable;

    protected PopUpPickerFormCell()
    {
        /* Required constructor */
    }

    protected PopUpPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }

    protected override void Initialize()
    {
        base.Initialize();

        var configuration = UIButtonConfiguration.PlainButtonConfiguration;

        if (IsPopUpIndicatorSupported)
        {
            configuration.Indicator = UIButtonConfigurationIndicator.Popup;
        }

        configuration.BaseForegroundColor = UIColor.SecondaryLabel;
        configuration.ContentInsets = NSDirectionalEdgeInsets.Zero;

        MenuButton = new UIButton
        {
            TranslatesAutoresizingMaskIntoConstraints = false,
            Configuration = configuration,
            ShowsMenuAsPrimaryAction = true,
            HorizontalAlignment = UIControlContentHorizontalAlignment.Trailing,
            Hidden = true,
        };

        MenuButton.Menu = UIMenu.Create([
            UIDeferredMenuElement.CreateUncached(completion => completion(CreateMenuElements())),
        ]);

        ContentView.AddSubview(MenuButton);

        ContentView.AddConstraints(
            new[]
            {
                MenuButton.TopAnchor.ConstraintEqualTo(ContentView.TopAnchor),
                MenuButton.BottomAnchor.ConstraintEqualTo(ContentView.BottomAnchor),
                MenuButton.LeadingAnchor.ConstraintEqualTo(ContentView.LeadingAnchor),
                MenuButton.TrailingAnchor.ConstraintEqualTo(
                    ContentView.LayoutMarginsGuide.TrailingAnchor
                ),
            }
        );
    }

    private UIMenuElement[] CreateMenuElements()
    {
        if (Item == null)
        {
            return [];
        }

        return CreateMenuElements(Item);
    }

    protected virtual UIMenuElement[] CreateMenuElements(TItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        var optionActions = item
            .Options.Select(option => (UIMenuElement)CreateOptionAction(item, option))
            .ToArray();

        var options = UIMenu.Create(
            item.Message ?? string.Empty,
            null,
            UIMenuIdentifier.None,
            UIMenuOptions.DisplayInline,
            optionActions
        );

        if (!item.CanBeCleared)
        {
            return [options];
        }

        var clearAction = UIAction.Create(FormTexts.Clear, null, null, _ => item.Pick(default));
        clearAction.Attributes = UIMenuElementAttributes.Destructive;

        var clear = UIMenu.Create(
            string.Empty,
            null,
            UIMenuIdentifier.None,
            UIMenuOptions.DisplayInline,
            [clearAction]
        );

        return [options, clear];
    }

    protected virtual UIAction CreateOptionAction(TItem item, IPickerOptionFormItem option)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(option);

        var action = UIAction.Create(option.FormattedValue ?? string.Empty, null, null, _ =>
            item.Pick(option)
        );

        action.State = item.IsPicked(option) ? UIMenuElementState.On : UIMenuElementState.Off;

        return action;
    }

    protected override void UpdateInputType()
    {
        if (!IsPopUp)
        {
            base.UpdateInputType();
        }
        else
        {
            PickerPresenter?.Dispose();
            PickerPresenter = null;
        }

        UpdatePopUpState();
    }

    protected virtual void UpdatePopUpState()
    {
        var isPopUp = IsPopUp;

        Accessory = isPopUp
            ? UITableViewCellAccessory.None
            : UITableViewCellAccessory.DisclosureIndicator;

        if (ValueLabel != null)
        {
            ValueLabel.Hidden = isPopUp;
        }

        if (MenuButton != null)
        {
            MenuButton.Hidden = !isPopUp;
        }
    }

    protected override void UpdateReadOnlyState()
    {
        base.UpdateReadOnlyState();

        if (MenuButton == null)
        {
            return;
        }

        MenuButton.Enabled = !Item?.IsReadOnly ?? false;
    }

    protected override void UpdateLabel(string? text)
    {
        base.UpdateLabel(text);

        if (MenuButton == null)
        {
            return;
        }

        MenuButton.AccessibilityLabel = text;
    }

    protected override void UpdateValue(string? text)
    {
        base.UpdateValue(text);

        if (MenuButton?.Configuration == null)
        {
            return;
        }

        var configuration = MenuButton.Configuration;
        configuration.Title = text;

        MenuButton.Configuration = configuration;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            MenuButton?.Dispose();
            MenuButton = null;
        }

        base.Dispose(disposing);
    }
}

[Register(nameof(PopUpPickerFormCell))]
public sealed class PopUpPickerFormCell : PopUpPickerFormCell<IPickerFormItem>
{
    public PopUpPickerFormCell()
    {
        /* Required constructor */
    }

    public PopUpPickerFormCell(NativeHandle handle)
        : base(handle)
    {
        /* Required constructor */
    }
}
