using System.Diagnostics.CodeAnalysis;

namespace Buform;

public static class MauiFormPlatform
{
    private static readonly MauiFormGroupRegistry GroupRegistry;
    private static readonly MauiFormItemRegistry ItemRegistry;

    static MauiFormPlatform()
    {
        GroupRegistry = new MauiFormGroupRegistry();
        ItemRegistry = new MauiFormItemRegistry();
    }

    public static void Initialize(MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<FormView, FormViewHandler>();
        });
    }

    public static void RegisterGroupHeader<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            TGroupView
    >()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooterView<TGroup>
    {
        GroupRegistry.RegisterGroupHeader<TGroup, TGroupView>();
    }

    public static void RegisterGroupFooter<
        TGroup,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            TGroupView
    >()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooterView<TGroup>
    {
        GroupRegistry.RegisterGroupFooter<TGroup, TGroupView>();
    }

    public static void RegisterItem<
        TItem,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            TItemView
    >()
        where TItem : class, IFormItem
        where TItemView : FormItemView<TItem>
    {
        ItemRegistry.RegisterItem<TItem, TItemView>();
    }

    public static bool TryGetHeaderViewType(
        Type groupType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            out Type? viewType
    )
    {
        return GroupRegistry.TryGetHeaderViewType(groupType, out viewType);
    }

    public static bool TryGetFooterViewType(
        Type groupType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            out Type? viewType
    )
    {
        return GroupRegistry.TryGetFooterViewType(groupType, out viewType);
    }

    public static bool TryGetCellViewType(
        Type itemType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
            out Type? viewType
    )
    {
        return ItemRegistry.TryGetCellViewType(itemType, out viewType);
    }
}
