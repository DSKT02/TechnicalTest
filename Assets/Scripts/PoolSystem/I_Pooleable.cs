using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_Pooleable<T>
{
    public bool Active { get; set; }

    public void ClearState();

    public string GetFromPoolCondition();
}
