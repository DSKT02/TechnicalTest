using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosInput : MonoBehaviour, I_CustomizableInput<Vector2>
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private LayerMask mouseDetectionLayer;

    Vector2 I_CustomizableInput<Vector2>.CustomInput { get; set; }

    bool I_CustomizableInput<Vector2>.CustomInputEnabled()
    {
#if UNITY_STANDALONE
        return true;
#else
        return false;
#endif
    }

    void I_CustomizableInput<Vector2>.CustomInputUpdate()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, mouseDetectionLayer))
        {
            (this as I_CustomizableInput<Vector2>).CustomInput = new Vector2(hit.point.x, hit.point.z);
        }
    }

    private void FixedUpdate()
    {
        (this as I_CustomizableInput<Vector2>).CustomInputUpdate();
    }
}
