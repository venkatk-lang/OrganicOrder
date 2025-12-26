using UnityEngine;

[CreateAssetMenu(menuName = "OrganicOrder/Item")]
public class ItemData : ScriptableObject
{
    public Item itemType;
    public string itemName;
    public Sprite icon;
}
public enum Item
{
    Pizza,
    Donut,
    Cake,
    Chicken,
    Sandwich,
    Muffin
}