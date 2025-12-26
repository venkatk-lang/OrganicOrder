using System.Collections.Generic;

public class LeftOfCondition : ICondition, IItemBasedCondition
{
    ItemData left, right;

    public ItemData PrimaryItem => left;

    public LeftOfCondition(ItemData left, ItemData right)
    {
        this.left = left;
        this.right = right;
    }

    public bool Evaluate(Dictionary<ItemData, int> pos)
        => pos[left] < pos[right];

    public string ToRuleText()
       => $"{left.itemName} must be to the left of {right.itemName}";

    public string ToIfText()
        => $"{left.itemName} is to the left of {right.itemName}";

    public string ToThenText()
        => ToRuleText();

    public void CollectItems(HashSet<ItemData> items)
    {
        items.Add(left);
        items.Add(right);
    }

    public string GetTextWithoutItem(ConditionGrammar grammar)
    {
        return grammar == ConditionGrammar.If
            ? $"is to the left of {right.itemName}"
            : $"must be to the left of {right.itemName}";
    }
    public string GetSignature()
    {
        return $"LEFT:{left.itemType}-{right.itemType}";
    }

}
