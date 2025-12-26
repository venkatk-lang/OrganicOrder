using System.Collections.Generic;

public class InPositionCondition : ICondition, IItemBasedCondition
{
    ItemData item;
    int position;

    public ItemData PrimaryItem => item;

    public InPositionCondition(ItemData item, int position)
    {
        this.item = item;
        this.position = position;
    }

    public bool Evaluate(Dictionary<ItemData, int> pos)
        => pos[item] == position;

    public string ToRuleText()
      => $"{item.itemName} must be in position {position + 1}";

    public string ToIfText()
        => $"{item.itemName} is in position {position + 1}";

    public string ToThenText()
        => ToRuleText();

    public void CollectItems(HashSet<ItemData> items)
        => items.Add(item);

    public string GetTextWithoutItem(ConditionGrammar grammar)
    {
        return grammar == ConditionGrammar.If
            ? $"is in position {position + 1}"
            : $"must be in position {position + 1}";
    }
    public string GetSignature()
    {
        return $"POS:{item.itemType}@{position}";
    }
  
}
