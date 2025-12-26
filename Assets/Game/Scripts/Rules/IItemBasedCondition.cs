public interface IItemBasedCondition
{
    ItemData PrimaryItem { get; }
    string GetTextWithoutItem(ConditionGrammar grammar);
}
public enum ConditionGrammar
{
    Rule,   // standalone or THEN
    If      // IF clause
}
