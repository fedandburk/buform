using System;
using Microsoft.Maui.Hosting;

namespace Buform;

public static class BuformMaui
{
    private static readonly FormGroupRegistry GroupRegistry;
    private static readonly FormItemRegistry ItemRegistry;

    static BuformMaui()
    {
        GroupRegistry = new FormGroupRegistry();
        ItemRegistry = new FormItemRegistry();

        FormComponentRegistry.Register();
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
        where TGroupView : FormsHeaderFooterView<TGroup>
    {
        GroupRegistry.RegisterGroupHeaderClass<TGroup, TGroupView>();
    }

    public static void RegisterGroupFooterClass<TGroup, TGroupView>()
        where TGroup : class, IFormGroup
        where TGroupView : FormsHeaderFooterView<TGroup>
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
