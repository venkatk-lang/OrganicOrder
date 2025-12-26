using System.Collections.Generic;

public class NotCondition : ICondition
{
    ICondition inner;
    public ICondition Inner => inner;

    public NotCondition(ICondition inner)
    {
        this.inner = inner;
    }

    public bool Evaluate(Dictionary<ItemData, int> pos)
        => !inner.Evaluate(pos);

    public string ToRuleText()
        => inner.ToRuleText().Replace("must be", "cannot be");

    public string ToIfText()
        => inner.ToIfText().Replace(" is ", " is not ");

    public string ToThenText()
        => ToRuleText();

    public void CollectItems(HashSet<ItemData> items)
        => inner.CollectItems(items);

    public string GetSignature()
    {
        return $"NOT({inner.GetSignature()})";
    }

}
