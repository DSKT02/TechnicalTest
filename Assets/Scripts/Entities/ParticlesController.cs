using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesController : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem shootPS, deadPS;

    [SerializeField]
    private EntityAction<bool> shootAction;

    [SerializeField]
    private Health playerHealth;

    private void Start()
    {
        shootAction.ActionEvent += (_) => { if (_ && !shootPS.isPlaying) shootPS.Play(); };
        playerHealth.OnHealthReachZero += deadPS.Play;
    }
}
