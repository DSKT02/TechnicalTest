using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectivesUI : MonoBehaviour
{
    [SerializeField]
    private GameFlowManager manager;

    [SerializeField]
    private Inventory playerInventory;

    [SerializeField]
    private TextMeshProUGUI[] killObjectivePropts;

    [SerializeField]
    private GameObject[] keyImages;


    private void Start()
    {
        foreach (var item in keyImages)
        {
            item.SetActive(false);
        }
        foreach (var item in killObjectivePropts)
        {
            item.text = $"{manager.EnemiesToBeat}/{manager.TotalEnemiesToBeat}";
        }

        manager.OnScoreChange += (_) =>
        {
            foreach (var item in killObjectivePropts)
            {
                item.text = _;
            }
        };

        playerInventory.OnKeyChange += (_) =>
        {
            foreach (var item in keyImages)
            {
                item.SetActive(_);
            }
        };
    }
}
