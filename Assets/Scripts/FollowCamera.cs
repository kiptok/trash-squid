using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour {
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _posLerpValuePerSecond = 0.1f;
    [SerializeField] private float _zoomLerpValuePerSecond = 0.1f;
    [SerializeField] private float _minOrthographicSize = 11.13f;
    [SerializeField] private float _maxOrthographicSize = 31.25f;

    private bool _inWater;
    private Camera _camera;

    private void Awake() {
        if (_target == null)
            Debug.LogWarning("No _target. FollowCamera will remain inactive.", this);
        _camera = GetComponent<Camera>();
        //SquidController.OnSurface += OnSquidSurface;
        //SquidController.OnDive += OnSquidDive;
    }

    private void Update() {
        if (_target != null) {
            Zoom();
            Follow();
        }
    }

    private void Follow() {
        var distanceToTarget = _target.position - transform.position;
        var directionToTarget = distanceToTarget.normalized;
        var lerpValue = _posLerpValuePerSecond * Time.deltaTime;
        var movementMagnitudeThisFrame = Mathf.Lerp(0.0f, distanceToTarget.magnitude, lerpValue);
        var movementThisFrame = directionToTarget * movementMagnitudeThisFrame;
        float minXPos = -50f + _camera.aspect * _camera.orthographicSize;
        float maxXPos = 50f - _camera.aspect * _camera.orthographicSize;
        float minYPos = -100f + _camera.orthographicSize;
        float maxYPos = 100f - _camera.orthographicSize;
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        float zPos = transform.position.z;
        movementThisFrame.z = 0;
        transform.position += movementThisFrame;
        xPos = Mathf.Clamp(xPos + movementThisFrame.x, minXPos, maxXPos);
        yPos = Mathf.Clamp(yPos + movementThisFrame.y, minYPos, maxYPos);
        transform.position = new Vector3(xPos, yPos, zPos);
    }

    private void Zoom() {
        var targetOrthographicSize = Mathf.Lerp(_minOrthographicSize, _maxOrthographicSize, transform.position.y / _maxOrthographicSize);
        //targetOrthographicSize = Mathf.Clamp(transform.position.y, _minOrthographicSize, _maxOrthographicSize);
        var lerpValue = _zoomLerpValuePerSecond * Time.deltaTime;
        var resizeThisFrame = Mathf.Lerp(0.0f, targetOrthographicSize - _camera.orthographicSize, lerpValue);

        _camera.orthographicSize += resizeThisFrame;
    }
}
