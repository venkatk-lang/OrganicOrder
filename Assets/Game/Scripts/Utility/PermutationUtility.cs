using System.Collections.Generic;

public static class PermutationUtility
{
    public static IEnumerable<List<T>> Permute<T>(List<T> list)
    {
        if (list.Count == 1)
        {
            yield return new List<T>(list);
            yield break;
        }

        for (int i = 0; i < list.Count; i++)
        {
            var current = list[i];
            var remaining = new List<T>(list);
            remaining.RemoveAt(i);

            foreach (var perm in Permute(remaining))
            {
                perm.Insert(0, current);
                yield return perm;
            }
        }
    }
}
