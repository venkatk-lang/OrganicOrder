using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class LevelGenerator
{
    public GeneratedLevel Generate(
       ItemDatabaseSO itemDB,
       int positions,
       int ruleCount,
       int maxLockedSeeds)
    {
        var items = itemDB.items.GetRange(0, positions);

        var lockedSeeds = positions > 3
      ? GenerateLockedSeeds(items, positions, maxLockedSeeds)
      : new List<LockedSeed>();

        var rules = GenerateRules(items, ruleCount, positions);

        int solutions = RuleSolver
            .GetSolutions(items, rules, lockedSeeds)
            .Count;

        return new GeneratedLevel
        {
            positions = positions,
            items = items,
            rules = rules,
            lockedSeeds = lockedSeeds,
            isImpossible = solutions == 0
        };
    }

    // ----------------------------------------------------
    // Locked Seeds
    // ----------------------------------------------------

    List<LockedSeed> GenerateLockedSeeds(
        List<ItemData> items,
        int positions,
        int maxLockedSeeds)
    {
        int lockCount =
            Random.Range(0, Mathf.Min(maxLockedSeeds, positions - 1) + 1);

        var chosenItems = items
            .OrderBy(_ => Random.value)
            .Take(lockCount)
            .ToList();

        var chosenPositions = Enumerable
            .Range(0, positions)
            .OrderBy(_ => Random.value)
            .Take(lockCount)
            .ToList();

        var locks = new List<LockedSeed>(lockCount);

        for (int i = 0; i < lockCount; i++)
        {
            locks.Add(new LockedSeed
            {
                item = chosenItems[i],
                position = chosenPositions[i]
            });
        }

        return locks;
    }
    //List<RuntimeRule> GenerateRules(
    // List<ItemData> items,
    // int ruleCount,
    // int positions)
    //{
    //    var rules = new List<RuntimeRule>(ruleCount);
  

    //    bool allowAdvanced = positions > 3;
    //    bool ifThenUsed = false;

    //    for (int i = 0; i < ruleCount; i++)
    //    {
    //        RuntimeRule rule;

    //        if (!allowAdvanced)
    //        {
    //            //  ONLY simple atomic rules

    //            ICondition baseCondition = CreateRandomAtomicCondition(items);

    //            if (CanApplyNot(baseCondition))
    //                baseCondition = MaybeNot(baseCondition);

    //            rule = new RuntimeRule
    //            {
    //               root = baseCondition
    //            };
    //        }
    //        else
    //        {
    //            rule = GenerateSingleRule(items, allowIfThen: !ifThenUsed);

    //            if (rule.root is IfThenCondition)
    //                ifThenUsed = true;
    //        }

    //        rules.Add(rule);
    //    }

    //    return rules;
    //}
    List<RuntimeRule> GenerateRules(
    List<ItemData> items,
    int ruleCount,
    int positions)
    {
        var rules = new List<RuntimeRule>(ruleCount);
        var usedSignatures = new HashSet<string>();

        bool allowAdvanced = positions > 3;
        bool ifThenUsed = false;

        int safety = 0;

        while (rules.Count < ruleCount && safety < 50)
        {
            safety++;

            RuntimeRule rule;

            if (!allowAdvanced)
            {
                ICondition baseCondition = CreateRandomAtomicCondition(items);
                if (CanApplyNot(baseCondition))
                    baseCondition = MaybeNot(baseCondition);

                rule = new RuntimeRule { root = baseCondition };
            }
            else
            {
                rule = GenerateSingleRule(items, allowIfThen: !ifThenUsed);
                if (rule.root is IfThenCondition)
                    ifThenUsed = true;
            }

            string sig = rule.GetSignature();

            if (usedSignatures.Contains(sig))
                continue; // regenerate

            usedSignatures.Add(sig);
            rules.Add(rule);
        }

        return rules;
    }


    RuntimeRule GenerateSingleRule(List<ItemData> items,bool allowIfThen)
    {
        int max = allowIfThen ? 6 : 5;
        int roll = Random.Range(0, max);

        return roll switch
        {
            0 => CreateLeftRule(items), // for left right conditions we dont need NOT.
            1 => CreateRightRule(items), // for left right conditions we dont need NOT.
            2 => WrapMaybeNotRule(CreateAdjacentRule(items)),
            3 => WrapMaybeNotRule(CreateInPositionRule(items)),
            4 => CreateOrRule(items),
            _ => CreateIfThenRule(items)
        };
    }
    ICondition CreateRandomAtomicCondition(List<ItemData> items)
    {
        int roll = Random.Range(0, 4);

        return roll switch
        {
            0 => CreateInPositionCondition(items),
            1 => CreateLeftCondition(items),
            2 => CreateRightCondition(items),
            _ => CreateAdjacentCondition(items)
        };
    }

    ICondition CreateInPositionCondition(List<ItemData> items)
    {
        var item = RandomItem(items);
        int pos = Random.Range(0, items.Count);
        return new InPositionCondition(item, pos);
    }
    RuntimeRule CreateInPositionRule(List<ItemData> items)
    {
        
        return new RuntimeRule
        {
            root = CreateInPositionCondition(items)
        };

    }
    ICondition CreateLeftCondition(List<ItemData> items)
    {
        var a = RandomItem(items);
        var b = RandomDifferentItem(items, a);
        return new LeftOfCondition(a, b);
    }
    RuntimeRule CreateLeftRule(List<ItemData> items)
    {
        return new RuntimeRule
        {
            root = CreateLeftCondition(items),
        };
    }
    ICondition CreateRightCondition(List<ItemData> items)
    {
        var a = RandomItem(items);
        var b = RandomDifferentItem(items, a);
        return new RightOfCondition(a, b);
    }
    RuntimeRule CreateRightRule(List<ItemData> items)
    {
       
        return new RuntimeRule
        {
            root = CreateRightCondition(items)
        };
    }

    ICondition CreateAdjacentCondition(List<ItemData> items)
    {
        var a = RandomItem(items);
        var b = RandomDifferentItem(items, a);
        return new AdjacentCondition(a, b);
    }

    RuntimeRule CreateAdjacentRule(List<ItemData> items)
    {
        return new RuntimeRule
        {
            root = CreateAdjacentCondition(items)
        };
    }



    RuntimeRule WrapMaybeNotRule(RuntimeRule baseRule)  //40% chance to wrap in not
    {
        // ~40% chance to negate
        if (Random.value < 0.4f)
        {
            return new RuntimeRule
            {
                root = new NotCondition(baseRule.root)
            };
        }

        return baseRule;
    }
    RuntimeRule CreateOrAdjacentRule(List<ItemData> items)
    {
        if (items.Count < 3)
            return null;

        ItemData a = RandomItem(items);
        ItemData b = RandomDifferentItem(items, a);
        ItemData c = RandomDifferentItem(items, a, b);

        AdjacentCondition condA = new AdjacentCondition(a, c);
        AdjacentCondition condB = new AdjacentCondition(b, c);

        return new RuntimeRule
        {
            root = new OrCondition(condA, condB)
        };
    }
    RuntimeRule CreateOrLeftRule(List<ItemData> items)
    {
        if (items.Count < 3)
            return null;

        ItemData a = RandomItem(items);
        ItemData b = RandomDifferentItem(items, a);
        ItemData c = RandomDifferentItem(items, a, b);

        LeftOfCondition condA = new LeftOfCondition(a, c);
        LeftOfCondition condB = new LeftOfCondition(b, c);

        return new RuntimeRule
        {
            root = new OrCondition(condA, condB)
        };
    }
    RuntimeRule CreateOrRightRule(List<ItemData> items)
    {
        if (items.Count < 3)
            return null;

        ItemData a = RandomItem(items);
        ItemData b = RandomDifferentItem(items, a);
        ItemData c = RandomDifferentItem(items, a, b);

        RightOfCondition condA = new RightOfCondition(a, c);
        RightOfCondition condB = new RightOfCondition(b, c);

        return new RuntimeRule
        {
            root = new OrCondition(condA, condB)
        };
    }

    RuntimeRule CreateOrInPositionRule(List<ItemData> items)
    {
        if (items.Count < 2)
            return null;

        ItemData a = RandomItem(items);
        ItemData b = RandomDifferentItem(items, a);

        int position = Random.Range(0, items.Count);

        InPositionCondition condA =
            new InPositionCondition(a, position);

        InPositionCondition condB =
            new InPositionCondition(b, position);

        return new RuntimeRule
        {
            root = new OrCondition(condA, condB)
        };
    }
    RuntimeRule CreateOrRule(List<ItemData> items)
    {
        int roll = Random.Range(0, 1); //Using only 2 adjacent and InPoisiton for OR here

        return roll switch
        {
            0 => WrapMaybeNotRule(CreateOrAdjacentRule(items)),
            1 => WrapMaybeNotRule(CreateOrInPositionRule(items)),
            2 => CreateOrRightRule(items),
            _ => CreateOrLeftRule(items)
        };
    }
    ICondition CreateItemOrCondition(
    List<ItemData> items)
    {
        int roll = Random.Range(0, 4);

        ItemData a = RandomItem(items);
        ItemData b = RandomDifferentItem(items, a);
        ItemData c = RandomDifferentItem(items, a,b);

        switch (roll) 
        {
            case 0: return new OrCondition(
                    new AdjacentCondition(a, c),
                    new AdjacentCondition(b, c));
            case 1: return new OrCondition(
                    new LeftOfCondition(a, c),
                    new LeftOfCondition(b, c));
            case 2: return new OrCondition(
                    new RightOfCondition(a, c),
                    new RightOfCondition(b, c));
            default:  int pos = Random.Range(0, items.Count);
                return new OrCondition(
                        new InPositionCondition(a, pos),
                        new InPositionCondition(b, pos));


        }
    }
    ICondition CreateGeneralCondition(List<ItemData> items)
    {
        bool useOr = Random.value < 0.35f;
   

        ICondition baseCondition =
            useOr
                ? CreateItemOrCondition(items)
                : CreateRandomAtomicCondition(items);

        if (CanApplyNot(baseCondition))
            return MaybeNot(baseCondition);

        return baseCondition;
    }
    bool CanApplyNot(ICondition condition)
    {
        return condition is InPositionCondition
            || condition is AdjacentCondition
            || condition is OrCondition;
    }

    ICondition MaybeNot(ICondition condition)
    {
        return Random.value < 0.4f
            ? new NotCondition(condition)
            : condition;
    }

    RuntimeRule CreateIfThenRule(
     List<ItemData> items)
    {
        ICondition ifCondition;
        ICondition thenCondition;

        int safety = 0;

        do
        {
            ifCondition = CreateGeneralCondition(items);
            thenCondition = CreateGeneralCondition(items);
            safety++;
        }
        while (
            AreMeaninglesslyEquivalent(ifCondition, thenCondition)
            && safety < 10
        );

        return new RuntimeRule
        {
            root = new IfThenCondition(ifCondition, thenCondition)
        };
    }
    bool AreMeaninglesslyEquivalent(ICondition a, ICondition b)
    {
        if (a.GetType() != b.GetType())
            return false;

        HashSet<ItemData> setA = new();
        HashSet<ItemData> setB = new();

        a.CollectItems(setA);
        b.CollectItems(setB);

        return setA.SetEquals(setB);
    }


    ItemData RandomItem(List<ItemData> items)
    {
        return items[Random.Range(0, items.Count)];
    }

    ItemData RandomDifferentItem(List<ItemData> items, ItemData exclude1,ItemData exclude2 = null)
    {
        ItemData result;
        do
        {
            result = RandomItem(items);
        }
        while (result == exclude1 || result == exclude2);

        return result;
    }
    ItemData RandomDifferentItem(
    List<ItemData> allItems,
    params ItemData[] exclude)
    {
        if (allItems == null || allItems.Count == 0)
        {
            Debug.LogError("RandomDifferentItem: allItems is empty.");
            return null;
        }

        // Build exclusion set
        HashSet<ItemData> excluded = new HashSet<ItemData>(exclude);

        // Build candidate list
        List<ItemData> candidates = new List<ItemData>();
        foreach (var item in allItems)
        {
            if (!excluded.Contains(item))
                candidates.Add(item);
        }

        if (candidates.Count == 0)
        {
            Debug.LogError("RandomDifferentItem: No valid candidates after exclusion.");
            return null;
        }

        int index = Random.Range(0, candidates.Count);
        return candidates[index];
    }
}
