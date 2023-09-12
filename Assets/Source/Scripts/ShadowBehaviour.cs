using UnityEngine;

public class ShadowBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _shadow;

    [SerializeField] private float _minSize;
    [SerializeField] private float _maxSize;
    [SerializeField] private float _maxY;
    
    private void Update()
    {
        var targetPosition = _target.position;
        var scale = Vector3.one * Mathf.Lerp(_minSize, _maxSize, _maxY / targetPosition.y);
        var position = transform.position;
        position.y = _shadow.position.y;
        _shadow.position = position;
        _shadow.localScale = scale;
    }
}
