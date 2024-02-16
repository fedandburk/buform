using System;
using System.Collections.Generic;
using System.Linq;

namespace Buform;

internal class FormItemRegistry
{
    private sealed class Holder
    {
        public Type CellType { get; }
        public Type? ExpandedCellType { get; }

        public Holder(Type cellType, Type? expandedCellType)
        {
            CellType = cellType ?? throw new ArgumentNullException(nameof(cellType));
            ExpandedCellType = expandedCellType;
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

    public void RegisterItemClass<TItem, TItemView>()
        where TItem : class, IFormItem
        where TItemView : FormItemView<TItem>
    {
        _holders[typeof(TItem)] = new Holder(
            typeof(TItemView),
            null
        );
    }

    public void RegisterItemClass<TItem, TItemView, TExpandedItemView>()
        where TItem : class, IFormItem
        where TItemView : FormItemView<TItem>
        where TExpandedItemView : FormItemView<TItem>
    {
        _holders[typeof(TItem)] = new Holder(
            typeof(TItemView),
            typeof(TExpandedItemView)
        );
    }

    public bool TryGetCellViewType(Type itemType, out Type? viewType)
    {
        if (itemType == null)
        {
            throw new ArgumentNullException(nameof(itemType));
        }

        var result = TryGetHolder(itemType, out var holder);

        if (result)
        {
            viewType = holder!.CellType;

            return true;
        }

        viewType = null;

        return false;
    }

    public bool TryGetExpandedCellViewType(Type itemType, out Type? viewType)
    {
        if (itemType == null)
        {
            throw new ArgumentNullException(nameof(itemType));
        }

        var result = TryGetHolder(itemType, out var holder);

        if (result)
        {
            viewType = holder!.ExpandedCellType;

            return viewType != null;
        }

        viewType = null;

        return false;
    }
}