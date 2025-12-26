using System.Collections.Generic;
using UnityEngine;

public class OrCondition : ICondition
{
    ICondition a, b;
    public ICondition A => a;
    public ICondition B => b;

    public OrCondition(ICondition a, ICondition b)
    {
        this.a = a;
        this.b = b;
    }

    public bool Evaluate(Dictionary<ItemData, int> pos)
        => a.Evaluate(pos) || b.Evaluate(pos);



    public string ToRuleText()
        => BuildText(ConditionGrammar.Rule);

    public string ToIfText()
        => BuildText(ConditionGrammar.If);

    public string ToThenText()
        => BuildText(ConditionGrammar.Rule);

    string BuildText(ConditionGrammar grammar)
    {
        if (TryBuildItemOrText(grammar, out string combined))
            return combined;

        // fallback generic OR
        string left = grammar == ConditionGrammar.If
            ? a.ToIfText()
            : a.ToRuleText();

        string right = grammar == ConditionGrammar.If
            ? b.ToIfText()
            : b.ToRuleText();

        return $"{left} OR {right}";
    }

    bool TryBuildItemOrText(
        ConditionGrammar grammar,
        out string text)
    {
        text = null;

        if (a.GetType() != b.GetType())
            return false;

        if (a is not IItemBasedCondition ca ||
            b is not IItemBasedCondition cb)
            return false;

        if (ca.PrimaryItem == cb.PrimaryItem)
            return false;

        string condA = ca.GetTextWithoutItem(grammar);
        string condB = cb.GetTextWithoutItem(grammar);

        if (condA != condB)
            return false;

        text =
            $"{ca.PrimaryItem.itemName} or {cb.PrimaryItem.itemName} {condA}";

        return true;
    }

    public void CollectItems(HashSet<ItemData> items)
    {
        a.CollectItems(items);
        b.CollectItems(items);
    }
    public string GetSignature()
    {
        // order-independent
        var sigA = a.GetSignature();
        var sigB = b.GetSignature();

        return sigA.CompareTo(sigB) < 0
            ? $"OR({sigA}|{sigB})"
            : $"OR({sigB}|{sigA})";
    }
   
}
