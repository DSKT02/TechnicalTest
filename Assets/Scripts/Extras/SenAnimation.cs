using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SenAnimation : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector3 amplitude, frecuency, offset;

    private Vector3 initialPos;

    private float t;
    private void Start()
    {
        initialPos = transform.position;
    }
    private void OnEnable()
    {
        t = 0;
    }

    private void Update()
    {
        Vector3 evaluatePos = new Vector3(Evaluate(amplitude.x, frecuency.x, offset.x, t), Evaluate(amplitude.y, frecuency.y, offset.y, t), Evaluate(amplitude.z, frecuency.z, offset.z, t)) + initialPos;
        transform.position = evaluatePos;
        t += Time.deltaTime;
    }

    private float Evaluate(float _amplitude, float _frecuency, float _offset, float _t)
    {
        return (Mathf.Sin(_t * _frecuency) * _amplitude) + _offset;
    }
}
