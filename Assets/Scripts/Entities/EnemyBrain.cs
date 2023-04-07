
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain : MonoBehaviour
{
    [SerializeField]
    private Transform targetTransform;

    [SerializeField]
    private EntityAction<Vector2> aimAction;

    [SerializeField]
    private EntityAction<Vector2> moveAction;

    [SerializeField]
    private EntityAction<bool> shootAction;

    [SerializeField]
    private float maxRadiusDetection;

    [SerializeField]
    private float maxRadiusToShoot;

    [SerializeField]
    private float minRadiusGetClose;

    private bool _hasClueOnPlayerPos, _playerInLineOfSight, _playerInZone, _inDistanceToShoot, _canGetCloserToPlayer;

    private Vector3 lasPlayerPosition;

    private Transform currentPlayer;

    private EnemyState mainState = new EnemyState();

    private List<EnemyNeuron> neurons = new List<EnemyNeuron>();

    private void Start()
    {
        SetUpNeurons();
        UpdateMainState();
        foreach (var item in neurons)
        {
            StartCoroutine(C_BrainActivity(item));
        }
    }

    private void SetUpNeurons()
    {
        //playerInZone = 0,
        //playerInLineOfSight = 0,
        //inDistanceToShoot = 0,
        //canGetCloserToPlayer = 0,
        //hasClueOnPlayerPos = 0,
        //
        //delayTime = 0.2f,
        //
        //OnSuccess = () =>
        //{
        //
        //},
        //OnFailure = () =>
        //{
        //
        //},

        neurons = new List<EnemyNeuron>()
        {
            new EnemyNeuron()
            {
                playerInZone = -1,
                playerInLineOfSight = 0,
                inDistanceToShoot = 0,
                canGetCloserToPlayer = 0,
                hasClueOnPlayerPos = 0,

                delayTime = 0.2f,

                OnSuccess = () =>
                {
                    if(currentPlayer == null)
                    {
                        Collider[] hitColliders = Physics.OverlapSphere(targetTransform.position, maxRadiusDetection);
                        foreach (var item in hitColliders)
                        {
                            if (!item.TryGetComponent<CharacterAffiliation>(out var entity)) continue;

                            if (entity.TeamID == 0)
                            {
                                currentPlayer = entity.transform;
                                _playerInZone = true;
                            }
                        }
                    }
                    else
                    {
                        _playerInZone = Vector3.Distance(currentPlayer.position, targetTransform.position) < maxRadiusDetection;
                    }
                },
            },
            new EnemyNeuron()
            {
                playerInZone = 1,
                playerInLineOfSight = 0,
                inDistanceToShoot = 0,
                canGetCloserToPlayer = 0,
                hasClueOnPlayerPos = 0,

                delayTime = 0.1f,

                OnSuccess = () =>
                {
                    float distance = Vector3.Distance(currentPlayer.position, targetTransform.position);
                    if (Physics.SphereCast(targetTransform.position, 0.3f , currentPlayer.position - targetTransform.position, out var hit, distance))
                    {
                        _playerInLineOfSight = hit.transform == currentPlayer;
                        _hasClueOnPlayerPos = _playerInLineOfSight ? true: _hasClueOnPlayerPos;
                        if(_playerInLineOfSight) lasPlayerPosition = currentPlayer.position;
                    }
                    _playerInZone = Vector3.Distance(currentPlayer.position, targetTransform.position) < maxRadiusDetection;
                },
            },
            new EnemyNeuron()
            {
                playerInZone = 1,
                playerInLineOfSight = 1,
                inDistanceToShoot = 0,
                canGetCloserToPlayer = 0,
                hasClueOnPlayerPos = 0,

                delayTime = 0.15f,

                OnSuccess = () =>
                {
                    _inDistanceToShoot =  Vector3.Distance(currentPlayer.position, targetTransform.position) < maxRadiusToShoot;
                    _canGetCloserToPlayer =  Vector3.Distance(currentPlayer.position, targetTransform.position) > minRadiusGetClose;
                },
            },
            new EnemyNeuron()
            {
                playerInZone = 1,
                playerInLineOfSight = 1,
                inDistanceToShoot = 0,
                canGetCloserToPlayer = 1,
                hasClueOnPlayerPos = 0,

                delayTime = 0.01f,

                OnSuccess = () =>
                {
                    var direction = currentPlayer.position - targetTransform.position;
                    var movementInput = new Vector2(direction.x, direction.z).normalized;
                    var aimInput = new Vector2(currentPlayer.position.x, currentPlayer.position.z);
                    moveAction.SetInput(movementInput);
                    aimAction.SetInput(aimInput);
                },
            },
            new EnemyNeuron()
            {
                playerInZone = 0,
                playerInLineOfSight = -1,
                inDistanceToShoot = 0,
                canGetCloserToPlayer = 0,
                hasClueOnPlayerPos = 1,

                delayTime = 0.01f,

                OnSuccess = () =>
                {
                    if(Vector3.Distance(lasPlayerPosition, targetTransform.position) < minRadiusGetClose / 3f) return;

                    var direction = lasPlayerPosition - targetTransform.position;
                    var movementInput = new Vector2(direction.x, direction.z).normalized;
                    var aimInput = new Vector2(currentPlayer.position.x, currentPlayer.position.z);
                    moveAction.SetInput(movementInput);
                    aimAction.SetInput(aimInput);
                },
            },
            new EnemyNeuron()
            {
                playerInZone = 1,
                playerInLineOfSight = 1,
                inDistanceToShoot = 1,
                canGetCloserToPlayer = 0,
                hasClueOnPlayerPos = 0,

                delayTime = 0.1f,

                OnSuccess = () =>
                {
                    var aimInput = new Vector2(currentPlayer.position.x, currentPlayer.position.z);
                    aimAction.SetInput(aimInput);
                    shootAction.SetInput(true);
                },
                OnFailure = () =>
                {
                    shootAction.SetInput(false);
                },
            },
        };
    }

    private void UpdateMainState()
    {
        mainState.playerInZone = _playerInZone ? 1 : -1;

        mainState.playerInLineOfSight = _playerInLineOfSight ? 1 : -1;

        mainState.inDistanceToShoot = _inDistanceToShoot ? 1 : -1;

        mainState.canGetCloserToPlayer = _canGetCloserToPlayer ? 1 : -1;

        mainState.hasClueOnPlayerPos = _hasClueOnPlayerPos ? 1 : -1;
    }

    private IEnumerator C_BrainActivity(EnemyNeuron neuron)
    {
        while (true)
        {
            UpdateMainState();
            if (!neuron.Compare(mainState))
            {
                neuron.OnFailure?.Invoke();
            }
            yield return new WaitUntil(() => neuron.Compare(mainState));
            neuron.OnSuccess?.Invoke();
            yield return new WaitForSeconds(neuron.delayTime);
        }
    }
}

public class EnemyNeuron : EnemyState
{
    public float delayTime;

    public Action OnSuccess;
    public Action OnFailure;

    public bool Compare(EnemyState _state)
    {
        if (playerInZone == 0 ? false : playerInZone != _state.playerInZone) return false;
        if (playerInLineOfSight == 0 ? false : playerInLineOfSight != _state.playerInLineOfSight) return false;
        if (inDistanceToShoot == 0 ? false : inDistanceToShoot != _state.inDistanceToShoot) return false;
        if (canGetCloserToPlayer == 0 ? false : canGetCloserToPlayer != _state.canGetCloserToPlayer) return false;
        if (hasClueOnPlayerPos == 0 ? false : hasClueOnPlayerPos != _state.hasClueOnPlayerPos) return false;

        return true;

    }
}

public class EnemyState
{
    public int playerInZone;
    public int playerInLineOfSight;
    public int inDistanceToShoot;
    public int canGetCloserToPlayer;
    public int hasClueOnPlayerPos;
}
