using System;
using PrimeTween;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private CameraSetting _mapCamera;
    [SerializeField] private CameraSetting _gameplayCamera;
    [SerializeField] private Camera _camera;

    [SerializeField] private bool _isOrthographic;
    [SerializeField] private Transform _gameplayParent;
    private float _interpolationFactor;
    private Tween _tween;
    private Tween _positionTween;

    private void Awake()
    {
        ChangeState();
    }

    [ContextMenu("Change")]
    private void ChangeState()
    {
        _isOrthographic = !_isOrthographic;
        SetState();
        AnimateCameraProjection();
    }
    
    private void SetState()
    {
        var setting = _isOrthographic ? _mapCamera : _gameplayCamera;
        _camera.transform.SetParent(_isOrthographic ? null : _gameplayParent);
        // _camera.orthographic = setting.IsOrtographic;
        _camera.orthographicSize = setting.Size;
        _camera.fieldOfView = setting.Size;
    }
    

    public void AnimateCameraProjection() {
        _tween.Stop();
        _positionTween.Stop();
        var setting = _isOrthographic ? _mapCamera : _gameplayCamera;
        _positionTween = Tween.LocalPosition(_camera.transform, setting.Position, 1f);
        _tween = Tween.Custom(this, _isOrthographic ? 0 : 1, _isOrthographic ? 1 : 0, 1f, ease: Ease.InOutSine, onValueChange: (target, t) => {
                // target.InterpolateProjectionMatrix(t);
            })
            .OnComplete(this, target => {
                // target._camera.orthographic = target._isOrthographic;
                target._camera.ResetProjectionMatrix();
            });
    }

    private void InterpolateProjectionMatrix(float _interpolationFactor) {
        this._interpolationFactor = _interpolationFactor;
        float aspect = (float) Screen.width / Screen.height;
        var orthographicSize = _camera.orthographicSize;
        var perspectiveMatrix = Matrix4x4.Perspective(_camera.fieldOfView, aspect, _camera.nearClipPlane, _camera.farClipPlane);
        var orthoMatrix = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, _camera.nearClipPlane, _camera.farClipPlane);
        Matrix4x4 projectionMatrix = default;
        for (int i = 0; i < 16; i++) {
            projectionMatrix[i] = Mathf.Lerp(perspectiveMatrix[i], orthoMatrix[i], _interpolationFactor);
        }
        _camera.projectionMatrix = projectionMatrix;
    }

    public bool IsAnimating => _tween.isAlive;
}

[Serializable]
public class CameraSetting
{
    public Vector3 Position;
    public bool IsOrtographic;
    public float Size;
}