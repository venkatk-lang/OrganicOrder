using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class TargetSlot : BaseSlot
{
    [Header("Visuals")]
    public SpriteRenderer backgroundRenderer;
    public Sprite normalSprite;
    public Sprite hoverSprite;

    [Header("Index Text")]
    public TextMeshPro indexText;
    public Action OnItemUpdated;

    protected override void Awake()
    {
        base.Awake();
        SetHover(false);
    }

    public override void SetIndex(int _index)
    {
        base.SetIndex(_index);
        if (indexText != null)
            indexText.text = (_index + 1).ToString();
    }

    public void SetHover(bool hovered)
    {
        if (backgroundRenderer == null) return;

        backgroundRenderer.sprite = hovered ? hoverSprite : normalSprite;
    }
  
    public override void PlaceItem(ItemDraggable item)
    {
        base.PlaceItem(item);
        OnItemUpdated?.Invoke();
    }
    public override void Clear()
    {
        base.Clear();
        OnItemUpdated?.Invoke();
    }
    private void OnDestroy()
    {
        OnItemUpdated = null;

    }
}
