using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
public static class FormPlatform
{
    private static readonly FormRegistry Registry;

    static FormPlatform()
    {
        Registry = new FormRegistry();

        FormComponentRegistry.Register();
    }

    public static void RegisterItem<TItem, TViewHolder>(int resourceId)
        where TItem : class, IFormItem
        where TViewHolder : FormViewHolder
    {
        Registry.RegisterItem<TItem, TViewHolder>(resourceId);
    }

    public static void RegisterGroupHeader<TGroup, TViewHolder>(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormViewHolder
    {
        Registry.RegisterGroupHeader<TGroup, TViewHolder>(resourceId);
    }

    public static void RegisterGroupFooter<TGroup, TViewHolder>(int resourceId)
        where TGroup : class, IFormGroup
        where TViewHolder : FormViewHolder
    {
        Registry.RegisterGroupFooter<TGroup, TViewHolder>(resourceId);
    }

    public static bool TryGetItemViewType(Type itemType, out int? viewType)
    {
        return Registry.TryGetItemViewType(itemType, out viewType);
    }

    public static bool TryGetGroupHeaderViewType(Type itemType, out int? viewType)
    {
        return Registry.TryGetGroupHeaderViewType(itemType, out viewType);
    }

    public static bool TryGetGroupFooterViewType(Type itemType, out int? viewType)
    {
        return Registry.TryGetGroupFooterViewType(itemType, out viewType);
    }

    public static bool TryGetViewHolderAndResourceId(
        int viewType,
        out Type? viewHolderType,
        out int? resourceId
    )
    {
        return Registry.TryGetViewHolderAndResourceId(viewType, out viewHolderType, out resourceId);
    }
}
