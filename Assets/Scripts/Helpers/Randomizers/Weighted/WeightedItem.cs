using UnityEngine;

[System.Serializable]
public abstract class WeightedItem { }

[System.Serializable]
public class WeightedItem<T> : WeightedItem
{
    public T item;

    public WeightedItem() { weight = 1; }

    public WeightedItem(T item) { this.item = item; weight = 1; }

    //[SerializeField] public int m_weight = 1;
    public int weight;


    [HideInInspector]
    public int cumulativeWeight;

    public static implicit operator T(WeightedItem<T> WeightedItem)
    {
        return WeightedItem.item;
    }
}