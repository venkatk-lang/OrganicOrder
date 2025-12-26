using IACGGames;
using System;
using UnityEngine;
public class RaycastInputManager : Singleton<RaycastInputManager>
{
    public LayerMask itemLayer;
    public LayerMask slotLayer;

    ItemDraggable currentItem;
    Vector3 grabOffset;
    TargetSlot hoveredTargetSlot;

    public bool InputEnabled { get; private set; } = true;
    public Action OnDragStart;
    void Update()
    {
        if (!InputEnabled)
            return;
        UpdateSlotHover();
        if (Input.GetMouseButtonDown(0))
            PointerDown();

        if (Input.GetMouseButton(0))
            PointerDrag();

        if (Input.GetMouseButtonUp(0))
            PointerUp();
    }
    public void EnableInput()
    {
        InputEnabled = true;
    }

    public void DisableInput()
    {
        InputEnabled = false;

        // Safety cleanup
        if (hoveredTargetSlot != null)
        {
            hoveredTargetSlot.SetHover(false);
            hoveredTargetSlot = null;
        }

        if (currentItem != null)
        {
            SpawnSlot fallback = GameManager.Instance.LevelManager.GetLowestFreeSpawnSlot();
            currentItem.ReturnToSpawn(fallback);
            currentItem = null;
    
        }
    }
   
    void UpdateSlotHover()
    {
        if (currentItem == null)
        {
            if (hoveredTargetSlot != null)
                hoveredTargetSlot.SetHover(false);
            return;
        }
      

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero, 0f, slotLayer);

        TargetSlot newHover = hit ? hit.collider.GetComponent<TargetSlot>() : null;

        if (hoveredTargetSlot == newHover)
            return;

        if (hoveredTargetSlot != null)
            hoveredTargetSlot.SetHover(false);

        hoveredTargetSlot = newHover;

        if (hoveredTargetSlot != null)
            hoveredTargetSlot.SetHover(true);
    }

    void PointerDown()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero, 0f, itemLayer);
        if (!hit) return;

        currentItem = hit.collider.GetComponent<ItemDraggable>();
        if (currentItem == null) return;
 
        if (!currentItem.CanStartDrag())
            return;

        grabOffset = currentItem.transform.position - (Vector3)mouseWorld;
        currentItem.StartDrag();
    }
    void PointerDrag()
    {
        if (currentItem == null) return;
        if (!currentItem.CanStartDrag())
            return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        currentItem.DragTo(mouseWorld + grabOffset);
        OnDragStart?.Invoke();
    }
    void PointerUp()
    {
        if (currentItem == null) return;
        if (!currentItem.CanStartDrag())
            return;

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorld, Vector2.zero, 0f, slotLayer);

        if (hit)
        {
            BaseSlot slot = hit.collider.GetComponent<BaseSlot>();
            if (slot != null)
            {
                currentItem.DropToSlot(slot);
                currentItem = null;
                return;
            }
        }

        SpawnSlot fallback = GameManager.Instance.LevelManager.GetLowestFreeSpawnSlot();
        currentItem.ReturnToSpawn(fallback);

        currentItem = null;
    }
  

}
