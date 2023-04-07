using System;
using UnityEngine;

public abstract class EntityAction<T> : MonoBehaviour
{
    public bool AllowAction { get; set; } = true;
    public bool AllowActionEvent { get; set; } = true;
    protected T CurrentInput { get; set; }
    protected T DeltaInput { get; set; }

    public Action<T> ActionEvent { get; set; }

    public virtual void SetInput(T _input)
    {
        DeltaInput = CurrentInput;
        CurrentInput = _input;
    }

    public virtual void Execute()
    {
        if (!AllowAction) return;

        Action();

        if (AllowActionEvent)
            ActionEvent?.Invoke(CurrentInput);
    }

    protected abstract void Action();

}