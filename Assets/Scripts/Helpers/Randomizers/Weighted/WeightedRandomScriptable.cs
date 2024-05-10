public abstract class WeightedRandomScriptable<T> : UnityEngine.ScriptableObject
{
    [System.Serializable] public class RandomGroup : WeightedRandom<T> { }
    public RandomGroup weightedRandomGroup = new RandomGroup();

    public virtual void OnEnable()
    {
        weightedRandomGroup.Reset();
        weightedRandomGroup.itemRollCheck = ItemAllowedInRoll;
    }

    public T Roll() => weightedRandomGroup.Roll();
    public T Roll(int avoidPreviousItemCount) => weightedRandomGroup.Roll(avoidPreviousItemCount);
    public void ClearPreviousItemCache() => weightedRandomGroup.ClearPreviousItemCache();
    public int RollForIndex() => weightedRandomGroup.RollForIndex();
    protected virtual bool ItemAllowedInRoll(T item, int index) => true;
    public static implicit operator T(WeightedRandomScriptable<T> item)
    {
        return item.Roll();
    }
}