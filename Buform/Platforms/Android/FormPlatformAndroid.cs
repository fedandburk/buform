namespace Buform;

public static class FormPlatform
{
    private static readonly FormItemRegistry ItemRegistry;

    static FormPlatform()
    {
        ItemRegistry = new FormItemRegistry();

        FormComponentRegistry.Register();
    }

    public static void RegisterItem<TItem, TViewHolder>(int resourceId)
        where TItem : class, IFormItem
        where TViewHolder : FormViewHolder
    {
        ItemRegistry.RegisterItem<TItem, TViewHolder>(resourceId);
    }

    public static bool TryGetViewType(Type itemType, out int? viewType)
    {
        return ItemRegistry.TryGetViewType(itemType, out viewType);
    }

    public static bool TryGetViewHolderAndResourceId(
        int viewType,
        out Type? viewHolderType,
        out int? resourceId
    )
    {
        return ItemRegistry.TryGetViewHolderAndResourceId(
            viewType,
            out viewHolderType,
            out resourceId
        );
    }
}
