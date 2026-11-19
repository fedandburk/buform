using System.Diagnostics.CodeAnalysis;

namespace Buform;

public static class FormPlatform
{
    private static readonly FormGroupRegistry GroupRegistry;
    private static readonly FormItemRegistry ItemRegistry;

    static FormPlatform()
    {
        GroupRegistry = new FormGroupRegistry();
        ItemRegistry = new FormItemRegistry();

        RegisterGroupHeader<ILabeledFormGroup, LabeledFormGroupHeader>();
        RegisterGroupFooter<ILabeledFormGroup, LabeledFormGroupFooter>();
        RegisterGroupHandler<IListFormGroup, ListFormGroupHandler>();

        RegisterItem<ButtonFormItem, ButtonFormCell>();
        RegisterItem<DateTimeFormItem, DateTimeFormCell>();
        RegisterItem<IAsyncPickerFormItem, AsyncPickerFormCell>();
        RegisterItem<ICallbackPickerFormItem, CallbackPickerFormCell>();
        RegisterItem<IMultiValuePickerFormItem, MultiValuePickerFormCell>();
        RegisterItem<IPickerFormItem, PopUpPickerFormCell>();
        RegisterItem<ISegmentsFormItem, SegmentsFormCell>();
        RegisterItem<SliderFormItem, SliderFormCell>();
        RegisterItem<StepperFormItem, StepperFormCell>();
        RegisterItem<SwitchFormItem, SwitchFormCell>();
        RegisterItem<IMultilineTextInputFormItem, MultilineTextInputFormCell>();
        RegisterItem<ITextInputFormItem, TextInputFormCell>();
        RegisterItem<ITextFormItem, TextFormCell>();
    }

    public static void RegisterGroupHeader<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooter<TGroup>
    {
        GroupRegistry.RegisterGroupHeader<TGroup, TGroupView>();
    }

    public static void RegisterGroupFooter<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooter<TGroup>
    {
        GroupRegistry.RegisterGroupFooter<TGroup, TGroupView>();
    }

    public static void RegisterGroupHeaderNib<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooter<TGroup>
    {
        GroupRegistry.RegisterGroupHeaderNib<TGroup, TGroupView>();
    }

    public static void RegisterGroupFooterNib<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooter<TGroup>
    {
        GroupRegistry.RegisterGroupFooterNib<TGroup, TGroupView>();
    }

    public static void RegisterItem<TItem, TItemView>()
        where TItem : class, IFormItem
        where TItemView : FormCell<TItem>
    {
        ItemRegistry.RegisterItem<TItem, TItemView>();
    }

    public static void RegisterItemNib<TItem, TItemView>()
        where TItem : class, IFormItem
        where TItemView : FormCell<TItem>
    {
        ItemRegistry.RegisterItemNib<TItem, TItemView>();
    }

    internal static void Register(UITableView tableView)
    {
        GroupRegistry.Register(tableView);
        ItemRegistry.Register(tableView);
    }

    public static void RegisterGroupHandler<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            THandler
    >()
        where TGroup : class, IFormGroup
        where THandler : FormGroupHandler<TGroup>
    {
        GroupRegistry.RegisterGroupHandler<TGroup, THandler>();
    }

    internal static bool TryGetHeaderReuseIdentifier(Type groupType, out string? reuseIdentifier)
    {
        return GroupRegistry.TryGetHeaderReuseIdentifier(groupType, out reuseIdentifier);
    }

    internal static bool TryGetFooterReuseIdentifier(Type groupType, out string? reuseIdentifier)
    {
        return GroupRegistry.TryGetFooterReuseIdentifier(groupType, out reuseIdentifier);
    }

    internal static bool TryGetReuseIdentifier(Type itemType, out string? reuseIdentifier)
    {
        return ItemRegistry.TryGetReuseIdentifier(itemType, out reuseIdentifier);
    }

    internal static IFormGroupHandler GetGroupHandler(IFormGroup group)
    {
        return GroupRegistry.GetGroupHandler(group);
    }
}
