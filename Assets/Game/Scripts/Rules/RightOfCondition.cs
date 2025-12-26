using System.Collections.Generic;

public class RightOfCondition : ICondition, IItemBasedCondition
{
    ItemData right, left;

    public ItemData PrimaryItem => right;

    public RightOfCondition(ItemData right, ItemData left)
    {
        this.right = right;
        this.left = left;
    }

    public bool Evaluate(Dictionary<ItemData, int> pos)
        => pos[right] > pos[left];


    public string ToRuleText()
    => $"{right.itemName} must be to the right of {left.itemName}";

    public string ToIfText()
        => $"{right.itemName} is to the right of {left.itemName}";

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
            ? $"is to the right of {left.itemName}"
            : $"must be to the right of {left.itemName}";
    }
    public string GetSignature()
    {
        return $"RIGHT:{right.itemType}-{left.itemType}";
    }
   

}
