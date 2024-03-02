using Android.Runtime;
using Fedandburk.Common.Extensions;

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

    public bool TryGetViewType(Type itemType, out int? viewType)
    {
        var result = TryGetHolder(itemType, out var holder);

        if (!result)
        {
            viewType = default;

            return false;
        }

        var index = _holders.Values.IndexOf(holder);

        if (index < 0)
        {
            viewType = default;

            return false;
        }

        viewType = index;

        return true;
    }

    public bool TryGetViewHolderAndResourceId(int viewType, out Type? viewHolderType, out int? resourceId)
    {
        var holder = _holders.Values.ElementAtOrDefault(viewType);

        if (holder == null)
        {
            viewHolderType = null;
            resourceId = null;

            return false;
        }

        viewHolderType = holder.ViewHolderType;
        resourceId = holder.ResourceId;

        return true;
    }
}