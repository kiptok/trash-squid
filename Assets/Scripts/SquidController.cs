using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class SquidController : MonoBehaviour {
    public static event Action<SquidController, Trash> OnDropTrash;
    public static event Action<SquidController> OnDive;
    public static event Action<SquidController> OnSurface;
    public static event Action<SquidController> OnJump;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip trashGetSound;
    public AudioClip thrustSound;
    public AudioClip jumpSound;
    public AudioClip splashSound;
    public AudioClip diveSound;
    public AudioClip surfaceSound;

    [SerializeField, TweakableMember(group = "Squid")] private float _thrustForceMagnitude = 1.0f;
    [SerializeField] private float _relativeForceDirectionInDegrees = 0.0f;
    [SerializeField, TweakableMember(minValue = -100, maxValue = 100, group = "Squid")] private float _torqueForceMagnitude = 1f;
    [SerializeField, TweakableMember(minValue = 0, maxValue = 5, group = "Squid")] private float _thrustTime = 0.5f;
    [SerializeField, TweakableMember(group = "Squid")] private float _jumpForceMagnitude = 1.0f;
    [SerializeField, TweakableMember(minValue = 0, maxValue = 5, group = "Squid")] private float _jumpHoldTime = 0.25f;
    [SerializeField, TweakableMember(minValue = 0, maxValue = 5, group = "Squid")] private float _trashThrowDownFactor = 0.5f;
    [SerializeField, TweakableMember(minValue = 0, maxValue = 5, group = "Squid")] private float _trashThrowVelocityFactor = 0.5f;
    [SerializeField, TweakableMember(minValue = 0, maxValue = 5, group = "Squid")] private float _dropTime = 0.5f;
    [SerializeField] private bool _canSpamThrust = false;
    [SerializeField] private bool _canSpamJump = false;
    [Header("Rigidbody parameters")]
    [SerializeField, TweakableMember(minValue = -5, maxValue = 5, group = "Squid")] private float _airGravityScale = 1.0f;
    [SerializeField, TweakableMember(minValue = -5, maxValue = 5, group = "Squid")] private float _waterGravityScale = 0.001f;
    [SerializeField, TweakableMember(minValue = 0, maxValue = 10, group = "Squid")] private float _airLinearDrag = 0.0f;
    [SerializeField, TweakableMember(minValue = 0, maxValue = 10, group = "Squid")] private float _waterLinearDrag = 1.0f;
    [SerializeField, TweakableMember(minValue = 0, maxValue = 25, group = "Squid")] private float _airAngularDrag = 15.0f;
    [SerializeField, TweakableMember(minValue = 0, maxValue = 25, group = "Squid")] private float _waterAngularDrag = 15.0f;

    private Rigidbody2D _rigidBody = null;
    private Collider2D _collider = null;
    private TrashDetector _trashCollider = null;
    private bool _isThrustQueued = false;
    private bool _canThrust = true;
    private bool _isJumpHeld = false;
    private bool _isJumpQueued = false;
    private bool _canDoubleJump = false;
    public float _jumpHoldTimer = 0.0f;
    private float _torque = 0.0f;
    private bool _inWater = true;
    private List<Trash> _pickedUpTrash = new List<Trash>();
    private bool _canPickUp = true;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _trashCollider = GetComponentInChildren<TrashDetector>();
        Trash.OnOnTrashEntersTriggerHandled += OnTrashCollideWithSquipPickUpCollider;
    }

    private void OnDestroy() {
        Trash.OnOnTrashEntersTriggerHandled -= OnTrashCollideWithSquipPickUpCollider;
    }

    private void Update() {
        if (_canSpamThrust && !_isThrustQueued) HandleSpamThrust();
        else if (!_canSpamThrust) HandleHeldThrust();
        if (!_canSpamJump) HandleHeldJump();
        _torque = Input.GetAxis("Horizontal") * _torqueForceMagnitude;
        if (!_inWater && transform.position.y < 0) HandleDive();
        else if (_inWater && transform.position.y > 0) HandleSurface();
        _inWater = transform.position.y < 0;
        if (!_inWater) _isThrustQueued = false;
        _rigidBody.gravityScale = _inWater ? _waterGravityScale : _airGravityScale;
        _rigidBody.drag = _inWater ? _waterLinearDrag : _airLinearDrag;
        _rigidBody.angularDrag = _inWater ? _waterAngularDrag : _airAngularDrag;
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F)) DropAllTrash();
    }

    private void FixedUpdate() {
        if (_isThrustQueued) {
            Thrust();
            animator.SetBool("Thrusting", true);
        } else if (_canThrust && !_isThrustQueued) {
            animator.SetBool("Thrusting", false);
        }
        if (_isJumpQueued) {
            DoubleJump(); // force determined by how long the jump was held
        }

        ApplyTorque();
        if (_inWater) {
            Glide();
        }
    }

    private void OnTrashCollideWithSquipPickUpCollider(Trash trash, TrashDetector collider) {
        if (collider == _trashCollider && _canPickUp) PickUpTrash(trash);
    }

    private void HandleSpamThrust() {
        _isThrustQueued = Input.GetButtonDown("Jump");
        _canThrust = true;
    }

    private void HandleHeldThrust() {
        if (!_canThrust || _isThrustQueued) return;
        _isThrustQueued = _inWater && (Input.GetButton("Jump") || Input.GetAxisRaw("Vertical") == 1);
        if (!_isThrustQueued) return;
        _canThrust = false;
        StartCoroutine(ResetHeldThrust());
    }

    private IEnumerator ResetHeldThrust() {
        yield return new WaitForSeconds(_thrustTime);
        _canThrust = true;
    }

    private void HandleHeldJump() {
        if (!_canDoubleJump || _isJumpQueued) return;
        if (_canDoubleJump && !_isJumpQueued && _isJumpHeld) {
            // check for release of double jump
            if (!_inWater && !(Input.GetButton("Jump") || Input.GetAxisRaw("Vertical") == 1)) {
                animator.SetBool("Holding jump", false);
                // reset if not held long enough
                if (_jumpHoldTimer > _jumpHoldTime) _isJumpQueued = true; // also maybe an indicator (prior) that jump is held long enough
                else _jumpHoldTimer = 0f;
            }
        }
        _isJumpHeld = !_inWater && (Input.GetButton("Jump") || Input.GetAxisRaw("Vertical") == 1);
        if (!_isJumpHeld) return;
        // play some charging effect/sound
        animator.SetBool("Holding jump", true); // maybe make this one-time
        _jumpHoldTimer += Time.deltaTime;
    }

    private void ApplyTorque() {
        _rigidBody.AddTorque(_torque);
    }

    private void Glide() {
        // var newVelocity = transform.rotation.eulerAngles * _rigidBody.velocity.magnitude;
        var newVelocity = new Vector2(
            Mathf.Cos((_relativeForceDirectionInDegrees + _rigidBody.rotation) * Mathf.Deg2Rad),
            Mathf.Sin((_relativeForceDirectionInDegrees + _rigidBody.rotation) * Mathf.Deg2Rad));
        newVelocity *= _rigidBody.velocity.magnitude;
        newVelocity *= Vector2.Dot(newVelocity.normalized, _rigidBody.velocity.normalized);
        _rigidBody.velocity = newVelocity;
        //        Debug.Log("rigidbody velocity " + _rigidBody.velocity);
        //        Debug.Log("new velocity " + newVelocity);
        //        Debug.Log("dot product " + Vector2.Dot(newVelocity.normalized, _rigidBody.velocity.normalized));
        //        Debug.Log(transform.rotation.eulerAngles);
        //        Debug.Log(_rigidBody.velocity.magnitude);
        //Debug.Log(newVelocity);
    }

    private void Thrust() {
        _rigidBody.AddRelativeForce(CalculateThrustForceVector(), ForceMode2D.Impulse);
        _isThrustQueued = false;
        audioSource.PlayOneShot(thrustSound, Random.Range(0.75f, 1f));
    }

    private Vector2 CalculateThrustForceVector() {
        var directionVector = new Vector2(
            Mathf.Cos(_relativeForceDirectionInDegrees * Mathf.Deg2Rad),
            Mathf.Sin(_relativeForceDirectionInDegrees * Mathf.Deg2Rad));
        return directionVector * _thrustForceMagnitude;
    }

    private Vector2 CalculateJumpForceVector() {
        var directionVector = new Vector2(
            Mathf.Cos(_relativeForceDirectionInDegrees * Mathf.Deg2Rad),
            Mathf.Sin(_relativeForceDirectionInDegrees * Mathf.Deg2Rad));
        return directionVector * _jumpForceMagnitude * _jumpHoldTimer;
    }

    private void DoubleJump() {
        // maybe make jump force dependent on trash or something
        _rigidBody.AddRelativeForce(CalculateJumpForceVector(), ForceMode2D.Impulse);
        _jumpHoldTimer = 0f;
        _canDoubleJump = false;
        _isJumpQueued = false;
        OnJump?.Invoke(this);
        // play jump sound
    }

    private void HandleDive() {
        // maybe make dive sound dependent on physics
        audioSource.PlayOneShot(splashSound);
        _canDoubleJump = false;
        OnDive?.Invoke(this);
    }

    private void HandleSurface() {
        // play surfacing sound
        _canDoubleJump = true;
        _jumpHoldTimer = 0f;
        OnSurface?.Invoke(this);
    }

    private void PickUpTrash(Trash trash) {
        trash.transform.SetParent(transform);
        trash.transform.position = _trashCollider.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        trash.GetComponent<Rigidbody2D>().simulated = false;
        _pickedUpTrash.Add(trash);
        audioSource.Play();
    }

    private IEnumerator ResetPickUp() {
        yield return new WaitForSeconds(_dropTime);
        _canPickUp = true;
    }

    private void DropAllTrash() {
        foreach (var trash in _pickedUpTrash.ToList())
            DropTrash(trash);
        _canPickUp = false;
        StartCoroutine(ResetPickUp());
    }

    private void DropTrash(Trash trash) {
        _pickedUpTrash.Remove(trash);
        trash.transform.SetParent(null);
        trash.GetComponent<Rigidbody2D>().simulated = true;
        trash.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        var trashImpulse = _rigidBody.velocity * _trashThrowVelocityFactor + Vector2.down * _trashThrowDownFactor;
        trash.GetComponent<Rigidbody2D>().AddForce(trashImpulse, ForceMode2D.Impulse);
        OnDropTrash?.Invoke(this, trash);
    }
}
