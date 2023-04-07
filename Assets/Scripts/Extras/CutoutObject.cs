using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private LayerMask wallMask;

    [SerializeField]
    private Vector3 offset;

    [SerializeField]
    private float cutoutSize = 0.1f, falloffSize = 0.05f;

    [SerializeField]
    private float detectionRadius = 1f, playerRadius = 0.2f;

    [SerializeField]
    private float fadeSpeed = 1f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position) + offset;

        //cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 _offset = targetObject.position - transform.position;

        List<RaycastHit> hitObjects = new List<RaycastHit>(Physics.SphereCastAll(transform.position, playerRadius, _offset, _offset.magnitude, wallMask));
        List<RaycastHit> hitObjectsOutside = new List<RaycastHit>(Physics.SphereCastAll(transform.position, detectionRadius, _offset, _offset.magnitude, wallMask));

        foreach (var item in hitObjects)
        {
            if (!item.transform.TryGetComponent<Renderer>(out var render)) return;
            Material[] materials = render.materials;

            foreach (var material in materials)
            {
                material.SetVector("_CutoutPos", cutoutPos);
                material.SetFloat("_CutoutSize", Mathf.MoveTowards(material.GetFloat("_CutoutSize"), cutoutSize, fadeSpeed * Time.fixedDeltaTime));
                material.SetFloat("_FalloffSize", falloffSize);
            }

            hitObjectsOutside.RemoveAll(_ => _.collider == item.collider);
        }

        foreach (var item in hitObjectsOutside)
        {
            if (!item.transform.TryGetComponent<Renderer>(out var render)) return;
            Material[] materials = render.materials;

            foreach (var material in materials)
            {
                material.SetVector("_CutoutPos", cutoutPos);
                material.SetFloat("_CutoutSize", material.GetFloat("_CutoutSize") > 0 ? Mathf.MoveTowards(material.GetFloat("_CutoutSize"), 0, fadeSpeed * Time.fixedDeltaTime) : 0);
                material.SetFloat("_FalloffSize", falloffSize);
            }
        }
    }
}
