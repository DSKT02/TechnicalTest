using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolController : MonoBehaviour
{
    private static List<object> pools = new List<object>();

    private void Awake()
    {
        pools.Clear();
    }

    public static void ForceIntoPool<T>(T reference) where T : MonoBehaviour, I_Pooleable<T>
    {
        Pool<T> selectedPool = null;

        foreach (var item in pools)
        {
            var poolAsType = item as Pool<T>;
            if (poolAsType == null) continue;
            selectedPool = poolAsType;
            break;
        }

        if (selectedPool == null)
        {
            selectedPool = new Pool<T>();
            pools.Add(selectedPool);
        }
        selectedPool.ForceIntoPool(reference);
    }

    public static T Get<T>(T reference, Vector3 position = default, Quaternion orientation = default, Transform parent = null) where T : MonoBehaviour, I_Pooleable<T>
    {
        Pool<T> selectedPool = null;

        foreach (var item in pools)
        {
            var poolAsType = item as Pool<T>;
            if (poolAsType == null) continue;
            selectedPool = poolAsType;
            break;
        }

        if (selectedPool == null)
        {
            selectedPool = new Pool<T>();
            pools.Add(selectedPool);
        }
        return selectedPool.Instantiate(reference, position, orientation, parent);
    }

}

public class Pool<T> where T : MonoBehaviour, I_Pooleable<T>
{
    public List<I_Pooleable<T>> items = new List<I_Pooleable<T>>();

    public List<I_Pooleable<T>> DisableItems { get { return items.FindAll((_) => !_.Active); } }
    public List<I_Pooleable<T>> ActiveItems { get { return items.FindAll((_) => _.Active); } }

    public void ForceIntoPool(T reference)
    {
        items.Add(reference);
    }

    public T Instantiate(T reference, Vector3 position = default, Quaternion orientation = default, Transform parent = null)
    {
        var tempItemRaw = DisableItems.Find
        (
            (_) => _.GetType() == reference.GetType() &&
            _.GetFromPoolCondition() == reference.GetFromPoolCondition()
        );

        if (tempItemRaw == null)
        {
            var tempItem = GameObject.Instantiate(reference, position, orientation);
            tempItem.transform.parent = parent;
            items.Add(tempItem as I_Pooleable<T>);
            items[items.Count - 1].Active = true;
            tempItem.gameObject.SetActive(true);
            return tempItem;
        }
        else
        {
            var tempItem = tempItemRaw as T;
            tempItem.transform.position = position;
            tempItem.transform.rotation = orientation;
            tempItem.transform.parent = parent;
            tempItemRaw.Active = true;
            tempItem.gameObject.SetActive(true);
            return tempItem;
        }
    }
}