using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField]
    private Image[] healthbars, healthbarsBackgrounds;

    [SerializeField]
    private Health playerHealth;

    [SerializeField]
    private float backgroundhealthSpeed = 0.5f;

    private float currentHealtvalue;

    private void Start()
    {
        playerHealth.OnHealthChange += (_) =>
        {
            currentHealtvalue = _;
            foreach (var item in healthbars)
            {
                item.fillAmount = currentHealtvalue;
            }
        };
    }

    private void Update()
    {
        foreach (var item in healthbarsBackgrounds)
        {
            item.fillAmount = Mathf.MoveTowards(item.fillAmount, currentHealtvalue, backgroundhealthSpeed * Time.deltaTime);
        }
    }
}
