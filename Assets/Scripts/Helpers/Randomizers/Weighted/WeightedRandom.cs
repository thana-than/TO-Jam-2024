using System;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// WeightedRandom Class.
/// </summary>
[Serializable]
public abstract class WeightedRandom { }

/// <summary>
/// WeightedRandom Class.
/// </summary>
[Serializable]
public class WeightedRandom<T> : WeightedRandom
{
    [Serializable]
    public class Item : WeightedItem<T>
    {
        public Item()
        {
            weight = 1;
        }

        public Item(T item, int weight)
        {
            this.item = item;
            this.weight = weight;
        }

        public Item(Item item)
        {
            this.item = item;
        }
    }

    [System.Serializable]
    public struct WeightTableItem
    {
        public int weight;
        public int cumulativeWeight;
        public int cumulativeWeight_rootIndex;
        public int cumulativeWeight_maxIndex;
        public string name;
        public WeightTableItem(Item item)
        {
            name = item.item.ToString();
            weight = item.weight;
            cumulativeWeight = item.cumulativeWeight;
            cumulativeWeight_rootIndex = 0;
            cumulativeWeight_maxIndex = 0;
        }

        public static void BuildFromItems(Item[] items, ref WeightTableItem[] array)
        {
            int len = items.Length;

            if (array.Length != len)
                array = new WeightTableItem[len];

            for (int i = 0; i < len; i++)
                array[i] = new WeightTableItem(items[i]);
        }
    }

    public WeightedRandom()
    {
        items = new Item[] { new Item() };
    }
    public Item[] items = { new Item() };
    [Min(0)] public int default_avoidPrevious = 1;
    public int[] previousIndexes;
    int totalWeight = 0;

    public void AddToItems(UnityEngine.Object[] objects)
    {
        Skim();

        int startWeight = 1;
        int itemLen = items.Length;
        if (itemLen > 0 && items[itemLen - 1].weight > 0)
            startWeight = items[itemLen - 1].weight;

        List<Item> newItems = new List<Item>();
        foreach (UnityEngine.Object obj in objects)
        {
            if (obj is T newItem)
                newItems.Add(new Item(newItem, startWeight));
        }

        if (newItems.Count > 0)
        {
            newItems.AddRange(items);
            Sort(newItems);
        }
    }

    void Skim()
    {
        if (typeof(T).IsValueType)
            return;

        List<Item> iList = items.ToList();

        for (int i = iList.Count - 1; i >= 0; i--)
        {
            if (iList[i].item.Equals(default(T)))
                iList.Remove(iList[i]);
        }

        items = iList.ToArray();
    }

    public void Sort() => Sort(items.ToList());

    bool isSorted = false;
    void Sort(List<Item> unsortedItems)
    {
        items = unsortedItems.OrderBy(o => o.item.ToString()).OrderByDescending(o => o.weight).ToArray();
        isSorted = true;
    }

    public void Reset()
    {
        ClearPreviousItemCache();
        if (!isSorted)
            Sort();
    }

    public void ClearPreviousItemCache()
    {
        previousIndexes = new int[items.Length];

        for (int i = previousIndexes.Length - 1; i >= 0; i--)
        {
            previousIndexes[i] = -1;
        }
    }

    //*The rollTable is what is actually used in the roll to determine the index
    //*This is separate from our items table as we can dynamically shift the weights of items in case we need to change the current roll
    //*ie. avoiding previous values, or being told that certain items aren't allowed in this roll (via external delegate invoke)
    [SerializeField] WeightTableItem[] rollTable = new WeightTableItem[0];
    int currentRollCount = 0;

    //*Each roll process involves THREE iterations and a binary search. Not very efficient but not sure where to improve performance and keep accuracy
    public T Roll() => Roll(default_avoidPrevious);
    public T Roll(int avoidPreviousItemCount)
    {
        if (!isSorted)
            Sort();
        UpdateRollTable(avoidPreviousItemCount);
        int index = RollForIndex();
        if (index < 0)
            return default;

        return items[index];
    }

    void UpdateRollTable(int avoidPreviousItemCount)
    {
        //*Clean values
        int totalLen = items.Length;
        WeightTableItem.BuildFromItems(items, ref rollTable);
        currentRollCount = 0;
        totalWeight = 0;

        //*Build previous Index cached array if necessary
        if (previousIndexes == null || previousIndexes.Length != totalLen)
            ClearPreviousItemCache();

        //*First filter, initial weight and external filter check
        bool externalFilter = itemRollCheck != null;
        for (int i = 0; i < totalLen; i++)
        {
            //* If we have an external check to filter values, we check here
            if (externalFilter && !itemRollCheck.Invoke(items[i], i))
            {
                //*Important to set both cumulativeWeight and weight to 0, as these are both checked throughout the roll
                rollTable[i].weight = 0;
                rollTable[i].cumulativeWeight = 0;
            }

            //* Accumulate roll count only if our weight is statistically possible to be rolled
            if (rollTable[i].weight > 0)
                currentRollCount++;
        }

        //* Skim out previous entries if we don't want em
        for (int i = 0;

            currentRollCount > 1 && //* Check we don't have too small of a roll array to continue; 
            i < avoidPreviousItemCount && //* Stop at the count specified
            i < totalLen - 1 && //* Stop before the last value
            previousIndexes[i] >= 0; //* Stop if we haven't reached this point before in the previous indexes

            i++)
        {
            int ignoreIndex = previousIndexes[i];
            if (rollTable[ignoreIndex].weight > 0)
            {
                //* Reduce the count here if we are resetting an allowed item
                rollTable[ignoreIndex].weight = 0;
                rollTable[ignoreIndex].cumulativeWeight = 0;
                currentRollCount--;
                //Debug.Log("Ignore: " + ignoreIndex);
            }
        }

        //*Build the actual list for this current roll and accumulate weight values
        for (int i = 0; i < totalLen; i++)
        {
            if (rollTable[i].weight > 0)
            {
                //*If this is a valid item, we accumulate the total weight value and make the roll table index keep track of the cumulativeWeight at this point
                totalWeight += rollTable[i].weight;
                rollTable[i].cumulativeWeight = totalWeight;
                //*The root index indicates the first index of a new cumulative weight value, we use this to speed up our binary search
                rollTable[i].cumulativeWeight_rootIndex = i;

                //*The root value of the previous cumulativeWeight index also holds on to the last index of its own cumulativeWeight value
                //*Reason being, this allows even more speed to the binary search, as we can bump up the min index of the search to the max of the inapplicable value
                if (i > 0)
                {
                    int prevRoot = rollTable[i - 1].cumulativeWeight_rootIndex;
                    rollTable[prevRoot].cumulativeWeight_maxIndex = i - 1;
                }
            }
            else
            {
                //* If this item is finally determined to be sorted out of the list,
                //* We'll just make it equal to the previous item in value and weight
                //* In our actual binary search, we pick the FIRST (root) index of the matching cumulativeWeight value,
                //* as the first index will always be valid
                //* This should have remove the value from the temporary sorted array
                //* Without messing with the final binary search (or ruining sort order)
                if (i > 0)
                {
                    rollTable[i] = rollTable[i - 1];
                    //rollTable[i].cumulativeWeight_rootIndex = -1;
                    //? below not neccessary?
                    //? rollTable[i].cumulativeWeight_maxIndex = rollTable[i - 1].cumulativeWeight_maxIndex;
                }
            }
        }

        //*Probably not needed, but tell the last root index that the length of the array is the max index
        int lastRoot = rollTable[totalLen - 1].cumulativeWeight_rootIndex;
        rollTable[lastRoot].cumulativeWeight_maxIndex = totalLen - 1;
    }


    /// <summary>
    /// Only the root cumulativeWeight index holds the max index of the matching cumulativeWeight value (for efficiencies sake)
    /// </summary>
    int GetMaxIndexOfSameCumulativeWeight(int index)
    {
        int root = rollTable[index].cumulativeWeight_rootIndex;
        return rollTable[root].cumulativeWeight_maxIndex;
    }

    public delegate bool ItemAllowedInRollCheck(T item, int index);
    public ItemAllowedInRollCheck itemRollCheck;

    void AddLatestIndexToPrevious(int index)
    {
        //*Keep track of previous indexes played
        for (int i = previousIndexes.Length - 1; i > 0; i--)
            previousIndexes[i] = previousIndexes[i - 1];
        previousIndexes[0] = index;
    }

    public int RollForIndex()
    {
        if (!isSorted)
            Sort();

        if (currentRollCount == 0)
        {
            //Debug.Log("ROLL COUNT IS 0");
            return -1;
        }


        // if (currentRollCount == 1)
        //     return 0;


        int weightResult = UnityEngine.Random.Range(0, totalWeight);

        int foundIndex = BinaryRollSearch(weightResult);

        //Debug.Log("Index: " + foundIndex + " Result: " + weightResult + " | Total Weight: " + totalWeight);

        AddLatestIndexToPrevious(foundIndex);

        return foundIndex;
    }

    int BinaryRollSearch(int cumulativeWeight)
    {
        int min = GetMaxIndexOfSameCumulativeWeight(0);
        int max = rollTable[rollTable.Length - 1].cumulativeWeight_rootIndex;

        if (min > max)
        {
            int swap = max;
            max = min;
            min = swap;
        }

        //Debug.Log("SEARCH START | min: " + min + " max: " + max + " | table length: " + rollTable.Length);
        while (min <= max)
        {
            int mid = (min + max) / 2;

            if (IsMatch(cumulativeWeight, mid))
                return rollTable[mid].cumulativeWeight_rootIndex;
            else if (cumulativeWeight < rollTable[mid].cumulativeWeight)
                max = rollTable[mid].cumulativeWeight_rootIndex - 1; //*Get the lowest index of this weight as to shorten the binary search
            else
                min = GetMaxIndexOfSameCumulativeWeight(mid) + 1; //*Get the highest index of this weight as to shorten the binary search
        }

        //Debug.Log("SEARCH FAILED | min: " + min + " max: " + max);
        return -1;
    }

    //*The winning index will have the FIRST cumulative weight GREATER than the search weight
    //*To check this in a binary search the previous cumulative weight on the table should also be less or equal to the search weight
    bool IsMatch(int cumulativeWeight, int index)
    {
        int minIndexOfSameValue = rollTable[index].cumulativeWeight_rootIndex;
        if (cumulativeWeight < rollTable[index].cumulativeWeight)
        {
            if (minIndexOfSameValue == 0 || cumulativeWeight >= rollTable[minIndexOfSameValue - 1].cumulativeWeight)
                return true;
        }

        return false;
    }

    public static implicit operator T(WeightedRandom<T> item)
    {
        return item.Roll();
    }
}