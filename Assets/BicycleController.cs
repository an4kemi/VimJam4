using System;
using System.Collections.Generic;
using PrimeTween;
using Source.Scripts.Extensions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class BicycleController : MonoBehaviour
{
    public Vector3 rackPosition;
    [SerializeField] private Transform _rackGroup;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Vector3 _positionOffset;
    private HashSet<DeliveryItem> _rackItems;

    [SerializeField] private float _groundCheckDistance;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _jumpMultiplierSpeed;
    [SerializeField] private float _jumpMultiplierMax;
    [SerializeField] private float _speed;
    [SerializeField] private float _backwardsSpeedDivide;
    [SerializeField] private float _turnSpeed;

    [SerializeField] private Transform[] _fork;
    
    [SerializeField] private float _maxHandleRotation;
    [SerializeField] private float _maxLeanAngle;
    [SerializeField] private float _leanSpeed;
    [SerializeField] private float _turnVisualSpeed;
    [SerializeField] private float _viewPositionSpeed;

    private float _jumpMultiplierCurrent;
    
    private bool _isGrounded;
    private float _horizontalInput;
    private float _verticalInput;

    private Action OnGroundedChange;
    
    private Vector3 _direction;
    private Vector3 _jumpDirection;

    private Tween _jumpTween;
    private Tween _shakeTween;
    
    private void Awake()
    {
        OnGroundedChange += () =>
        {
            if (!_jumpTween.isAlive)
            {
                _jumpTween = Tween.Scale(transform, new Vector3(1.15f, 0.9f, 1.15f), 0.2f, Ease.OutSine, 2, CycleMode.Yoyo);
            }
        };
        _rackItems = new HashSet<DeliveryItem>();
    }

    public void PutOnRack(DeliveryItem item)
    {
        var hasItems = _rackItems.Count > 0;
        _rackItems.Add(item);

        var placePosition = rackPosition + transform.position + new Vector3(0, item.collider.size.y * .5f, 0);
        if (hasItems)
        {
            var child = _rackGroup.GetChild(_rackGroup.childCount - 1);
            var collider = child.GetComponent<BoxCollider>();
            placePosition = child.transform.position + new Vector3(0, collider.size.y * .5f + item.collider.size.y * .5f, 0);
        }
        
        item.collider.transform.SetPositionAndRotation(placePosition, transform.rotation);
        item.collider.transform.SetParent(_rackGroup);
    }

    public void TakeFromRackById(string id)
    {
        DeliveryItem targetItem = null;
        foreach (var item in _rackItems)
        {
            if (item.id != id) continue;
            targetItem = item;
            break;
        }

        if (targetItem != null)
        {
            TakeFromRack(targetItem);
        }
    }
    
    public void TakeFromRack(DeliveryItem item)
    {
        if (!_rackItems.Contains(item)) return;
        item.collider.transform.SetParent(null);
        _rackItems.Remove(item);
    }
    
    private void Update()
    {
        _verticalInput = Input.GetAxis("Vertical");
        _horizontalInput = Input.GetAxis("Horizontal");
        _direction = (transform.forward * 1.5f);
        if (!IsGrounded()) return;

        // var viewTransform = transform;

        // transform.position += _direction * (_verticalInput * Time.deltaTime * _speed);
        // _rigidbody.AddForce(_direction * (_verticalInput * Time.deltaTime * _speed), ForceMode.VelocityChange);
        // _rigidbody.AddForce(_direction * (_verticalInput * Time.deltaTime * _speed));
        // _rigidbody.AddTorque(Vector3.up * (_horizontalInput * _turnSpeed), ForceMode.Acceleration);
        var space = Input.GetKeyUp(KeyCode.Space);
        var spaceHeld = Input.GetKey(KeyCode.Space);
        if (spaceHeld)
        {
            _jumpMultiplierCurrent += Time.deltaTime * _jumpMultiplierSpeed;
            if (!_shakeTween.isAlive)
            {
                _shakeTween = Tween.ShakeScale(transform, new Vector3(1.15f, 0.9f, 1.15f), 0.2f, 1);
            }
        }
        
        if (space)
        {
            _jumpDirection = Vector3.up * (_jumpPower * Mathf.Min(_jumpMultiplierMax, Mathf.Abs(_jumpMultiplierCurrent)));
            _jumpMultiplierCurrent = 1;
            // _rigidbody.AddForce(Vector3.up * (Time.deltaTime * _jumpPower), ForceMode.Impulse);
        }
    }
    
    private bool IsGrounded()
    {
        var isGrounded = Physics.Raycast(_rigidbody.transform.position, -Vector3.up, _groundCheckDistance);
        if (_isGrounded != isGrounded) OnGroundedChange.Invoke();
        _isGrounded = isGrounded;
        return _isGrounded;
    }

    private void FixedUpdate()
    {
        if (!IsGrounded()) return;
        _rigidbody.velocity = _jumpDirection + _direction * (_verticalInput * Time.fixedDeltaTime * _speed) / (_verticalInput < 0 ? _backwardsSpeedDivide : 1);
        if (!_jumpDirection.Equals(Vector3.zero)) _jumpDirection = Vector3.zero;
    }

    private void LateUpdate()
    {
        var handleAngle = Mathf.Clamp(_horizontalInput * _maxHandleRotation, -_maxHandleRotation, _maxHandleRotation);
        var targetHandleRotation = Quaternion.Euler(0f, handleAngle, 0f);
        foreach (var fork in _fork)
        {
            fork.localRotation = targetHandleRotation;
        }
        transform.position = Vector3.Lerp(transform.position, _rigidbody.position + _positionOffset, Time.deltaTime * _viewPositionSpeed);
        // transform.position = _rigidbody.position + _positionOffset;
        
        var leanAngle = Mathf.Clamp(_horizontalInput * _maxLeanAngle, -_maxLeanAngle, _maxLeanAngle);
        var forwardMultiplier = _verticalInput == 0 ? 1 : _verticalInput;
        var targetRotation = Quaternion.Euler(0f, transform.eulerAngles.y + _horizontalInput * forwardMultiplier * Time.deltaTime * _turnSpeed, -leanAngle);
        
        var rotateTo = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _leanSpeed);
        transform.rotation = rotateTo;
        _rigidbody.rotation = rotateTo;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawLine(position, position + transform.forward * _verticalInput);
        Gizmos.DrawSphere(rackPosition + position, .25f);
        Handles.DrawWireArc(position, Vector3.up, Vector3.forward, 360, 1.5f);

        var direction = position + _direction;
        Gizmos.DrawSphere(direction, .25f);
        GizmosUtils.DrawArrow(position, direction, 30, .5f, .5f);
    }
#endif
}

[Serializable]
public class DeliveryItem
{
    public BoxCollider collider;
    public string id;
}
