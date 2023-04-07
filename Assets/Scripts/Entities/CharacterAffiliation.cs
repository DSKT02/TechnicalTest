using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAffiliation : MonoBehaviour
{
    [field: SerializeField] public int TeamID { get; set; }

    public Guid CharacterID { get; set; }

    private void Awake()
    {
        CharacterID = Guid.NewGuid();
    }
}
