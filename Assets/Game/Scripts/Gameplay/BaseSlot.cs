using System;
using UnityEngine;

public abstract class BaseSlot : MonoBehaviour
{
    public int index;
    public ItemDraggable CurrentItem { get; private set; }

    public bool IsOccupied => CurrentItem != null;
    protected virtual void Awake() { }
    public virtual void SetIndex(int _index)
    {
        index = _index;
    }
   
    public virtual bool CanAcceptItem(ItemDraggable incoming)
    {
        if (CurrentItem == null)
            return true;
        if (CurrentItem.IsLocked)
        {
            CurrentItem.ShowWarning();
            return false;
        }
        return true;
    }

    public virtual void PlaceItem(ItemDraggable item)
    {
        if (!CanAcceptItem(item))
        {
            item.ReturnToOrigin();
            return;
        }

        BaseSlot originSlot = item.OriginSlot;

        if (CurrentItem != null)
        {
            ItemDraggable oldItem = CurrentItem;
            Clear();
            oldItem.PlaceToSlot(originSlot);
            originSlot.CurrentItem = oldItem;
        }

        CurrentItem = item;
        item.PlaceToSlot(this);
    }

    public virtual void Clear()
    {
        CurrentItem = null;
    }
}
