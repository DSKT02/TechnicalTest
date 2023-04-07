using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public bool AllowInputs { get; set; }

    private List<I_Input> inputLinks = new List<I_Input>();

    [SerializeField]
    private List<BuildInInputLink> buildInInputLinks = new List<BuildInInputLink>();

    [SerializeField]
    private List<NativeInputLink> nativeInputLinks = new List<NativeInputLink>();

    [SerializeField]
    private List<BuildInInputAxesRawLink> buildInInputAxesRawLink = new List<BuildInInputAxesRawLink>();

    [SerializeField]
    private List<CustomInputLink<Vector2>> customVector2InputLinks = new List<CustomInputLink<Vector2>>();

    [SerializeField]
    private List<CustomInputLink<bool>> customBoolInputLinks = new List<CustomInputLink<bool>>();

    [SerializeField]
    private List<CustomInputLink<int>> customIntInputLinks = new List<CustomInputLink<int>>();

    [SerializeField]
    private List<NativeIntBoundInputLink> nativeIntBoundInputLink = new List<NativeIntBoundInputLink>();

    [SerializeField]
    private List<ButtonInputLink> buttonInputLink = new List<ButtonInputLink>();

    [SerializeField]
    private List<ButtonIntBoundInputLink> buttonIntBoundInputLink = new List<ButtonIntBoundInputLink>();

    private void Start()
    {
        AddInputLinkListToMain(buildInInputLinks);
        AddInputLinkListToMain(nativeInputLinks);
        AddInputLinkListToMain(buildInInputAxesRawLink);
        AddInputLinkListToMain(customVector2InputLinks);
        AddInputLinkListToMain(customBoolInputLinks);
        AddInputLinkListToMain(customIntInputLinks);
        AddInputLinkListToMain(nativeIntBoundInputLink);
        AddInputLinkListToMain(buttonInputLink);
        AddInputLinkListToMain(buttonIntBoundInputLink);

        InitializeInputLinkLists();
    }

    private void Update()
    {
        UpdateInputLinkLists();
    }

    private void AddInputLinkListToMain<T>(List<T> list) where T : I_Input
    {
        foreach (var item in list)
        {
            inputLinks.Add(item);
        }
        inputLinks.Sort((a, b) => a.Priority.CompareTo(b.Priority));
    }

    private void InitializeInputLinkLists()
    {
        foreach (var item in inputLinks)
        {
            item.Initialize();
        }
    }

    private void UpdateInputLinkLists()
    {
        if (!AllowInputs) return;
        foreach (var item in inputLinks)
        {
            item.CallInputEvent();
        }
    }
}

[System.Serializable]
public class BuildInInputLink : InputLink<bool>
{
    public string buildInInputName;

    public override void CallInputEvent()
    {
        if (Input.GetButtonDown(buildInInputName))
        {
            inputEvent?.Invoke(true);
        }
        if (Input.GetButton(buildInInputName))
        {
            inputEvent?.Invoke(true);
        }
        if (Input.GetButtonUp(buildInInputName))
        {
            inputEvent?.Invoke(false);
        }
    }
}

[System.Serializable]
public class NativeInputLink : InputLink<bool>
{
    public KeyCode nativeInputKeyCode;

    public override void CallInputEvent()
    {
        if (Input.GetKeyDown(nativeInputKeyCode))
        {
            inputEvent?.Invoke(true);
        }
        if (Input.GetKey(nativeInputKeyCode))
        {
            inputEvent?.Invoke(true);
        }
        if (Input.GetKeyUp(nativeInputKeyCode))
        {
            inputEvent?.Invoke(false);
        }

    }
}

[System.Serializable]
public class NativeIntBoundInputLink : InputLink<int>
{
    public KeyCode nativeInputKeyCode;

    public int boundedInt;

    public override void CallInputEvent()
    {
        if (Input.GetKeyDown(nativeInputKeyCode))
        {
            inputEvent?.Invoke(boundedInt);
        }
        if (Input.GetKey(nativeInputKeyCode))
        {
            inputEvent?.Invoke(boundedInt);
        }
        if (Input.GetKeyUp(nativeInputKeyCode))
        {
            inputEvent?.Invoke(boundedInt);
        }

    }
}

[System.Serializable]
public class BuildInInputAxesRawLink : InputLink<Vector2>
{
    public string buildInXAxisName;
    public string buildInYAxisName;

    public override void CallInputEvent()
    {
        inputEvent?.Invoke(new Vector2(Input.GetAxisRaw(buildInXAxisName), Input.GetAxisRaw(buildInYAxisName)));
    }
}

[System.Serializable]
public class CustomInputLink<T> : InputLink<T>
{
    [SerializeField]
    private UnityEngine.Object customInputCallerObject;

    private I_CustomizableInput<T> customInputCaller;

    public override void Initialize()
    {
        base.Initialize();
        customInputCaller = customInputCallerObject as I_CustomizableInput<T>;

        if (customInputCaller == null)
        {
            Dispose();
        }
        Debug.AssertFormat(customInputCaller != null, $"{nameof(customInputCallerObject)} is null or invalid");
    }

    public override void CallInputEvent()
    {
        if (customInputCaller.CustomInputEnabled())
        {
            inputEvent?.Invoke(customInputCaller.CustomInput);
        }
    }
}

[System.Serializable]
public class ButtonInputLink : InputLink<bool>
{
    [SerializeField]
    private Button buttonInputCaller;

    private bool dirty;

    public override void Initialize()
    {
        base.Initialize();
        buttonInputCaller.onClick.AddListener(() => { inputEvent?.Invoke(true); dirty = true; });
    }

    public override void CallInputEvent()
    {
        if (dirty)
        {
            inputEvent?.Invoke(false);
            dirty = false;
        }
    }
}

[System.Serializable]
public class ButtonIntBoundInputLink : InputLink<int>
{
    [SerializeField]
    private Button buttonInputCaller;

    [SerializeField]
    private int boundedInt;

    private bool dirty;

    public override void Initialize()
    {
        base.Initialize();
        buttonInputCaller.onClick.AddListener(() => { inputEvent?.Invoke(boundedInt); dirty = true; });
    }

    public override void CallInputEvent()
    {
        if (dirty)
        {
            dirty = false;
        }
    }
}

public abstract class InputLink<T> : I_Input
{
    public int priority;

    int I_Input.Priority { get => priority; set => priority = value; }
    [field: SerializeField] public bool StandAloneOnly { get; set; }

    public EntityAction<T> entityAction;

    protected Action<T> inputEvent;


    public virtual void Initialize()
    {
#if UNITY_STANDALONE
            inputEvent += entityAction.SetInput;
#else
        if (!StandAloneOnly) inputEvent += entityAction.SetInput;
#endif
    }

    public virtual void Dispose()
    {
        inputEvent = null;
    }

    public abstract void CallInputEvent();
}

public interface I_Input
{
    public int Priority { get; set; }
    public bool StandAloneOnly { get; set; }

    public void Initialize();
    public void Dispose();
    public void CallInputEvent();
}