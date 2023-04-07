using UnityEngine;

public class Move : EntityAction<Vector2>
{
    [SerializeField]
    private Rigidbody targetRigidbody;

    [SerializeField]
    private float targetSpeed;

    [SerializeField]
    private float accelerationRate, decelerationRate;

    private float currentSpeed;

    public override void SetInput(Vector2 _input)
    {
        base.SetInput(_input);
        Execute();
    }

    protected override void Action()
    {
        if (targetRigidbody == null) return;

        if (CurrentInput != Vector2.zero)
        {
            currentSpeed += (accelerationRate * Time.deltaTime * DeltaInput.magnitude);
        }
        else
        {
            currentSpeed -= decelerationRate * Time.deltaTime;
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0, targetSpeed);

        targetRigidbody.velocity =
        new Vector3(CurrentInput.normalized.x * currentSpeed, targetRigidbody.velocity.y, CurrentInput.normalized.y * currentSpeed);
    }
}
