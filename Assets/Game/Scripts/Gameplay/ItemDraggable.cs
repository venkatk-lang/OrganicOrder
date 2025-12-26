using IACGGames;
using UnityEngine;

public class ItemDraggable : MonoBehaviour
{
    public ItemData Data {  get; private set; }

    public BaseSlot CurrentSlot { get; set; }
    public BaseSlot OriginSlot { get; private set; }
    [Header("Locked")]
    public bool IsLocked { get; private set; }
    public bool Interactable { get; private set; }

    [Header("Visuals")]
    SpriteRenderer mainRenderer;
    [SerializeField] GameObject lockObject;
    [SerializeField] GameObject warningObject; 

    void Awake()
    {
        mainRenderer = GetComponent<SpriteRenderer>();
        lockObject.SetActive(false);
        warningObject.SetActive(false);
    }

    public void SetData(ItemData itemData, bool locked)
    {
        Data = itemData;
        IsLocked = locked;
        MakeInteractable(false);
        mainRenderer.sprite = itemData.icon;
        gameObject.name = $"Item_{itemData.itemName}";
        lockObject.SetActive(IsLocked);
        warningObject.SetActive(false);
    }
    public void MakeInteractable(bool value)
    {
        Interactable = value;
    }
    public void PlaceToSlot(BaseSlot slot)
    {
        CurrentSlot = slot;
        transform.position = slot.transform.position;
        mainRenderer.sortingOrder = 0;
 
    }
    public bool CanStartDrag()
    {
        if (!IsLocked && Interactable)
            return true;
        AudioManager.Instance.PlaySFX(SFXAudioID.Lock);
        ShowWarning();
        return false;
    }

    public void StartDrag()
    {
        OriginSlot = CurrentSlot;
        AudioManager.Instance.PlaySFX(SFXAudioID.Pick);
        if (CurrentSlot != null)
        {
            CurrentSlot.Clear();
            CurrentSlot = null;
        }

        mainRenderer.sortingOrder = 10;
    }

    public void DragTo(Vector3 worldPos)
    {
        transform.position = worldPos;

    }
    public void EndDrag()
    {
        AudioManager.Instance.PlaySFX(SFXAudioID.Put);
        HideWarning();
    }

    public void ShowWarning()
    {
        warningObject.SetActive(true);
        CancelInvoke(nameof(HideWarning));
        Invoke(nameof(HideWarning), 0.5f);
    }

    void HideWarning()
    {
        warningObject.SetActive(false);
    }
    public void DropToSlot(BaseSlot slot)
    {
        EndDrag();
        slot.PlaceItem(this);
    }

    public void ReturnToSpawn(SpawnSlot fallbackSlot)
    {
        AudioManager.Instance.PlaySFX(SFXAudioID.Put);
        fallbackSlot.PlaceItem(this);
    }
    public void ReturnToOrigin()
    {
        if (OriginSlot != null)
            OriginSlot.PlaceItem(this);
    }

}
