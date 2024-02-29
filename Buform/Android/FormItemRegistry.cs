using Android.Runtime;

namespace Buform;

[Preserve(AllMembers = true)]
public sealed class FormItemRegistry
{
    private sealed class Holder
    {
        public Type ViewHolderType { get; }
        public int ResourceId { get; }

        public Holder(Type viewHolderType, int resourceId)
        {
            ViewHolderType = viewHolderType ?? throw new ArgumentNullException(nameof(viewHolderType));
            ResourceId = resourceId;
        }
    }

    private readonly IDictionary<Type, Holder> _holders;

    public FormItemRegistry()
    {
        _holders = new Dictionary<Type, Holder>();
    }

    private bool TryGetHolder(Type itemType, out Holder? holder)
    {
        if (_holders.TryGetValue(itemType, out holder))
        {
            return true;
        }

        var interfaceTypes = itemType.GetInterfaces().Except(
            itemType.GetInterfaces().SelectMany(item => item.GetInterfaces())
        );

        foreach (var interfaceType in interfaceTypes)
        {
            if (_holders.TryGetValue(interfaceType, out holder))
            {
                return true;
            }
        }

        return false;
    }

    public void RegisterItem<TItem, TViewHolder>(int resourceId)
        where TItem : class, IFormItem
        where TViewHolder : FormViewHolder
    {
        _holders[typeof(TItem)] = new Holder(
            typeof(TViewHolder),
            resourceId
        );
    }

    public bool TryGetViewHolderAndResourceId(Type itemType, out Type? viewHolderType, out int? resourceId)
    {
        if (itemType == null)
        {
            throw new ArgumentNullException(nameof(itemType));
        }

        var result = TryGetHolder(itemType, out var holder);

        if (result)
        {
            viewHolderType = holder!.ViewHolderType;
            resourceId = holder.ResourceId;

            return true;
        }

        viewHolderType = null;
        resourceId = null;

        return false;
    }
}