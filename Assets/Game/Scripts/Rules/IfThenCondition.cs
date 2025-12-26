using System.Collections.Generic;
using System.Diagnostics;

public class IfThenCondition : ICondition
{
    ICondition ifCond;
    public ICondition IfCond => ifCond;
    ICondition thenCond;
    public ICondition ThenCond => thenCond;

    public IfThenCondition(ICondition ifCond, ICondition thenCond)
    {
        this.ifCond = ifCond;
        this.thenCond = thenCond;
    }

    public bool Evaluate(Dictionary<ItemData, int> pos)
        => !ifCond.Evaluate(pos) || thenCond.Evaluate(pos);

    public string ToRuleText()
    {
       
        return $"If {ifCond.ToIfText()}, then {thenCond.ToThenText()}";
    }
      

    public string ToIfText() => ToRuleText();  // not used
    public string ToThenText() => ToRuleText();  // not used

    public void CollectItems(HashSet<ItemData> items)
    {
        ifCond.CollectItems(items);
        thenCond.CollectItems(items);
    }

    public string GetSignature()
    {
        return $"IF({ifCond.GetSignature()})THEN({thenCond.GetSignature()})";
    }

}
