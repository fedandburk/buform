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
}