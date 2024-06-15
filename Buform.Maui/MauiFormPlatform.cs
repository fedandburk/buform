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

    public static void RegisterGroupHeader<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooterView<TGroup>
    {
        GroupRegistry.RegisterGroupHeader<TGroup, TGroupView>();
    }

    public static void RegisterGroupFooter<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormHeaderFooterView<TGroup>
    {
        GroupRegistry.RegisterGroupFooter<TGroup, TGroupView>();
    }

    public static void RegisterItem<TItem, TItemView>()
        where TItem : class, IFormItem
        where TItemView : FormItemView<TItem>
    {
        ItemRegistry.RegisterItem<TItem, TItemView>();
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
