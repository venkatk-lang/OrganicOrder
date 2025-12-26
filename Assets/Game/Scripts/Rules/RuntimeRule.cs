using System.Collections.Generic;

public class RuntimeRule
{
    public ICondition root;

    public bool IsSatisfied(Dictionary<ItemData, int> pos)
        => root.Evaluate(pos);

    public string GetText()
        => root.ToRuleText();

    public void CollectItems(HashSet<ItemData> items)
        => root.CollectItems(items);

    public string GetSignature()
    => root.GetSignature();
    
}
