using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "OrganicOrder/Item Database")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemData> items;
}
