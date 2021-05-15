using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCamera : MonoBehaviour {
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _lerpValuePerSecond = 0.1f;

    private Camera _camera;

    private void Awake() {
        if (_target == null)
            Debug.LogWarning("No _target. FollowCamera will remain inactive.", this);
        _camera = GetComponent<Camera>();
        SquidController.OnSurface += OnSquidSurface;
    }

    private void Update() {
        if (_target != null)
            Follow();
    }

    private void Follow() {
        var distanceToTarget = _target.position - transform.position;
        var directionToTarget = distanceToTarget.normalized;
        var lerpValue = _lerpValuePerSecond * Time.deltaTime;
        var movementMagnitudeThisFrame = Mathf.Lerp(0.0f, distanceToTarget.magnitude, lerpValue);
        var movementThisFrame = directionToTarget * movementMagnitudeThisFrame;
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        float zPos = transform.position.z;
        movementThisFrame.z = 0;
        transform.position += movementThisFrame;
        xPos = Mathf.Clamp(xPos + movementThisFrame.x, -30.5f, 30.5f);
        yPos = Mathf.Clamp(yPos + movementThisFrame.y, -88.5f, 88.5f);
        transform.position = new Vector3(xPos, yPos, zPos);
    }

    private void OnSquidSurface(SquidController squid) {

    }
}
