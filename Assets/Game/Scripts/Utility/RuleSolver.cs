using System.Collections.Generic;
using System.Linq;

public static class RuleSolver
{
    public static List<SolverSolution> GetSolutions(
        List<ItemData> items,
        List<RuntimeRule> rules,
        List<LockedSeed> lockedSeeds)
    {
        var results = new List<SolverSolution>();

        foreach (var perm in PermutationUtility.Permute(items))
        {
            if (!RespectsLocks(perm, lockedSeeds))
                continue;

            var map = BuildMap(perm);

            if (rules.All(r => r.IsSatisfied(map)))
            {
                results.Add(new SolverSolution
                {
                    positions = new Dictionary<ItemData, int>(map)
                });

            }
        }

        return results;
    }

    static bool RespectsLocks(List<ItemData> perm, List<LockedSeed> locks)
    {
        foreach (var l in locks)
        {
            if (perm[l.position] != l.item)
                return false;
        }
        return true;
    }

    static Dictionary<ItemData, int> BuildMap(List<ItemData> perm)
    {
        var map = new Dictionary<ItemData, int>();
        for (int i = 0; i < perm.Count; i++)
            map[perm[i]] = i;
        return map;
    }
    public static List<RuntimeRule> GetImpossibleRules(
    List<ItemData> items,
    List<RuntimeRule> rules,
    List<LockedSeed> lockedSeeds)
    {
        var impossibleRules = new List<RuntimeRule>();

        // If the full set is already solvable, nothing is impossible
        if (GetSolutions(items, rules, lockedSeeds).Count > 0)
            return impossibleRules;

        foreach (var rule in rules)
        {
            // Test by removing this rule
            var reducedRules = new List<RuntimeRule>(rules);
            reducedRules.Remove(rule);

            int solutions = GetSolutions(
                items,
                reducedRules,
                lockedSeeds).Count;

            // If removing this rule restores solvability,
            // then this rule contributes to impossibility
            if (solutions > 0)
                impossibleRules.Add(rule);
        }

        return impossibleRules;
    }

}

public class SolverSolution
{
    public Dictionary<ItemData, int> positions;

    public string ToReadableString()
    {
        var lines = new List<string>();
        foreach (var kv in positions)
        {
            lines.Add($"{kv.Key.itemName} -> {kv.Value + 1}");
        }
        return string.Join(", ", lines);
    }
}

