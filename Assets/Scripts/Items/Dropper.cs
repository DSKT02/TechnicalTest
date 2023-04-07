using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper : MonoBehaviour
{
    [SerializeField]
    private Health health;

    [SerializeField]
    private List<Drop> drops = new List<Drop>();

    private MonoBehaviour selectedDrop;

    public void Start()
    {
        health.OnDead += Drop;
        Dictionary<float, MonoBehaviour> probabilityTable = new Dictionary<float, MonoBehaviour>();
        List<float> probabilityIdexes = new List<float>();

        float probabilityCounter = 0;
        foreach (var item in drops)
        {
            probabilityCounter += item.probability;
            probabilityIdexes.Add(probabilityCounter);
            probabilityTable.Add(probabilityCounter, item.itemPrefab);
        }

        float value = Random.Range(0, probabilityCounter);

        foreach (var item in probabilityIdexes)
        {
            if (value > item) continue;
            selectedDrop = probabilityTable[item];
            break;
        }
    }

    private void Drop()
    {
        Instantiate(selectedDrop, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.identity);
    }
}

[System.Serializable]
public class Drop
{
    public float probability = 1;
    public MonoBehaviour itemPrefab;
}