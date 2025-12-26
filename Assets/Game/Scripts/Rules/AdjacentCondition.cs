using System.Collections.Generic;
using UnityEngine;

public class AdjacentCondition : ICondition, IItemBasedCondition
{
    ItemData a, b;

    public ItemData PrimaryItem => a;

    public AdjacentCondition(ItemData a, ItemData b)
    {
        this.a = a;
        this.b = b;
    }

    public bool Evaluate(Dictionary<ItemData, int> pos)
        => Mathf.Abs(pos[a] - pos[b]) == 1;


    public string ToRuleText()
  => $"{a.itemName} must be next to {b.itemName}";

    public string ToIfText()
        => $"{a.itemName} is next to {b.itemName}";
    public string GetTextWithoutItem(ConditionGrammar grammar)
    {
        return grammar == ConditionGrammar.If
            ? $"is next to {b.itemName}"
            : $"must be next to {b.itemName}";
    }
    public string ToThenText()
        => ToRuleText();
    public void CollectItems(HashSet<ItemData> items)
    {
        items.Add(a);
        items.Add(b);
    }

    public string GetSignature()
    {
        return $"ADJ:{a.itemType}-{b.itemType}";
    }

}
