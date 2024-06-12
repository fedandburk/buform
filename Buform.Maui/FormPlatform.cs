using Microsoft.Maui.Controls.Internals;

namespace Buform;

[Preserve(AllMembers = true)]
public static class MauiFormPlatform
{
    private static readonly FormGroupRegistry GroupRegistry;
    private static readonly FormItemRegistry ItemRegistry;

    static MauiFormPlatform()
    {
        GroupRegistry = new FormGroupRegistry();
        ItemRegistry = new FormItemRegistry();
    }

    public static void Initialize(MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<FormView, FormViewHandler>();
        });
    }

    public static void RegisterGroupHeaderClass<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooterView<TGroup>
    {
        GroupRegistry.RegisterGroupHeaderClass<TGroup, TGroupView>();
    }

    public static void RegisterGroupFooterClass<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooterView<TGroup>
    {
        GroupRegistry.RegisterGroupFooterClass<TGroup, TGroupView>();
    }

    public static void RegisterItemClass<TItem, TItemView>()
        where TItem : class, IFormItem
        where TItemView : FormItemView<TItem>
    {
        ItemRegistry.RegisterItemClass<TItem, TItemView>();
    }

    public static bool TryGetHeaderViewType(Type groupType, out Type? viewType)
    {
        return GroupRegistry.TryGetHeaderViewType(groupType, out viewType);
    }

    public static bool TryGetFooterViewType(Type groupType, out Type? viewType)
    {
        return GroupRegistry.TryGetFooterViewType(groupType, out viewType);
    }

    public static bool TryGetCellViewType(Type itemType, out Type? viewType)
    {
        return ItemRegistry.TryGetCellViewType(itemType, out viewType);
    }
}
