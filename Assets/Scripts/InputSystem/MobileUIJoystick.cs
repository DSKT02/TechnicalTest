using UnityEngine;
using UnityEngine.UI;
using System;


[RequireComponent(typeof(RectTransform), typeof(Image))]
public class MobileUIJoystick : MonoBehaviour
{
    public Action<Vector2> OnJoystickMove;

    [SerializeField] private RectTransform _pointer;

    [SerializeField, Range(0, 2)] float _maxPointerRadius = 1;
    [SerializeField, Range(0, 1)] private float _relativeSize = 1f / 6f;
    [SerializeField] private float _reCenterSpeed = 500;
    [SerializeField] private float _toleranceRadius = 0;
    [SerializeField] private float _minAlpha = .1f;
    [SerializeField] private float _maxAlpha = .5f;

    private RectTransform _mRectTransform;
    private Image _mImage;
    private Image _pointerImage;

    private int _currentTouches = 0;
    private int _selectedFinger;
    private int _selectedFingerIndex;

    private bool _dragging = false;

    private float _radius;
    private Vector2 _center;
    private Vector2 _originalCenter;

    private Vector2 _joystickValue;

    private bool _allowInput = true;

    public Vector2 JoystickValue
    {
        get => _joystickValue;
        set => _joystickValue = new Vector2(Mathf.Clamp(value.x, -1, 1), Mathf.Clamp(value.y, -1, 1));
    }

    #region Unity Methods
    private void Start()
    {
        float tempScreenX = Screen.width;
        float tempScreenY = Screen.height;

        _mRectTransform = GetComponent<RectTransform>();
        _mImage = GetComponent<Image>();

        _mRectTransform.sizeDelta = new Vector2(tempScreenX * _relativeSize, tempScreenY * _relativeSize * (tempScreenX / tempScreenY));

        _radius = (_mRectTransform.sizeDelta.x / 2f) + (tempScreenX * (_mRectTransform.anchorMax.x - _mRectTransform.anchorMin.x) / 2f);
        _radius *= _maxPointerRadius;
        _originalCenter =
            new Vector2(
                _mRectTransform.position.x, _mRectTransform.position.y
            //tempScreenX * (_mRectTransform.anchorMin.x + _mRectTransform.anchorMax.x) / 2f,
            //tempScreenY * (_mRectTransform.anchorMin.y + _mRectTransform.anchorMax.y) / 2f
            );
        _center = _originalCenter;

        if (_pointer == null)
        {
            RectTransform[] tempSons = GetComponentsInChildren<RectTransform>();
            if (tempSons.Length > 1)
            {
                _pointer = tempSons[1];
                _pointer.transform.localPosition = Vector3.zero;
            }
            Debug.Assert(_pointer != null, "Needs Pointer", gameObject);
        }
        else
        {
            _pointer.transform.parent = transform;
            _pointer.transform.position = _center;
        }
        _pointerImage = _pointer.GetComponent<Image>();
    }

    private void LateUpdate()
    {
        //OnJoystickMove?.Invoke(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));//temporal
        if (_currentTouches != Input.touchCount)
        {
            _currentTouches = Input.touchCount;
            OnTouchUp();
            OnTouchDown();
            ResetFingerIndex();
        }

        OnTouchDrag();
        DrawJoystick();
    }
    #endregion

    public Type GetEventType()
    {
        return typeof(Action<Vector2>);
    }

    public void ToggleInput(bool state)
    {
        _allowInput = state;
    }

    private Vector2 GetTouchPos(int fingerIndex)
    {
        if (Input.touchCount <= fingerIndex) return Vector2.zero;

        return Input.touches[fingerIndex].position;
    }

    private void ResetFingerIndex()
    {
        if (Input.touchCount == 0) return;

        for (int i = 0; i < _currentTouches; i++)
        {
            if (Input.touches[i].fingerId == _selectedFinger)
            {
                _selectedFingerIndex = i;
                return;
            }
        }
    }

    private void OnTouchDown()
    {
        if (_dragging) return;

        foreach (var item in Input.touches)
        {
            if (Vector2.Distance(GetTouchPos(item.fingerId), _center) < _radius + _toleranceRadius)
            {
                _selectedFinger = item.fingerId;
                if (Vector2.Distance(GetTouchPos(_selectedFinger), _center) > _radius)
                    _center = GetTouchPos(_selectedFinger);

                _dragging = true;
                return;
            }
        }
    }

    private void OnTouchDrag()
    {
        if (!_dragging) return;

        Vector2 tempRelativePos = GetTouchPos(_selectedFingerIndex) - _center;
        float tempDist = Mathf.Clamp(Vector2.Distance(Vector2.zero, tempRelativePos), 0, _radius);
        JoystickValue = (tempRelativePos.normalized * tempDist) / _radius;

        if (_allowInput)
            OnJoystickMove?.Invoke(JoystickValue);
    }

    private void OnTouchUp()
    {
        if (!_dragging) return;

        foreach (var item in Input.touches)
        {
            if (item.fingerId == _selectedFinger)
                return;
        }

        _center = _originalCenter;
        JoystickValue = Vector2.zero;

        if (_allowInput)
            OnJoystickMove?.Invoke(JoystickValue);

        _dragging = false;
    }

    private void DrawJoystick()
    {
        if (_pointer == null) return;

        Vector3 nextPos = JoystickValue * _radius;

        if (_dragging)
        {
            transform.position = new Vector3(_center.x, _center.y, 0);
            _pointer.transform.position = nextPos + new Vector3(_center.x, _center.y, 0);
            _mImage.color = new Color(_mImage.color.r, _mImage.color.g, _mImage.color.b, _maxAlpha);
            _pointerImage.color = new Color(_pointerImage.color.r, _pointerImage.color.g, _pointerImage.color.b, _maxAlpha);
        }
        else
        {
            _mImage.color =
                Vector4.MoveTowards(
                    _mImage.color,
                    new Color(_mImage.color.r, _mImage.color.g, _mImage.color.b, _minAlpha),
                    Time.deltaTime
                    );
            _pointerImage.color =
                Vector4.MoveTowards(
                    _pointerImage.color,
                    new Color(_pointerImage.color.r, _pointerImage.color.g, _pointerImage.color.b, _minAlpha),
                    Time.deltaTime
                    );
            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    new Vector3(_originalCenter.x, _originalCenter.y, 0),
                    _reCenterSpeed * Time.deltaTime
                    );
            _pointer.transform.position =
                Vector2.MoveTowards(
                    _pointer.transform.position,
                    new Vector3(transform.position.x, transform.position.y, 0),
                    _reCenterSpeed * Time.deltaTime
                    );
        }
    }
}

