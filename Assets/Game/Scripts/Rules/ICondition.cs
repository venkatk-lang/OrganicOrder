using System.Collections.Generic;

public interface ICondition
{
    bool Evaluate(Dictionary<ItemData, int> positions);

    string ToRuleText();     // standalone rule
    string ToIfText();       // used after "If"
    string ToThenText();     // used after "then"
    string GetSignature();

    void CollectItems(HashSet<ItemData> items);
}
