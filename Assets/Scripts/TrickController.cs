using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickController : MonoBehaviour
{
    //public SquidController squid;
    //private TrickCombo combo;
    private Transform tfm;
    private Rigidbody2D rb;
    private float currentSpin; // spin maybe for separate script
    private float currentRotation;
    private float highestAir;
    private float spinThreshold = 180.0f;
    private float spinTolerance = 25.0f;
    private bool _inAir;
    private bool _jumped;

    public static event Action<SquidController> TrickCompleted;

    private void Awake() {
        tfm = this.transform;
        rb = this.GetComponent<Rigidbody2D>();
        SquidController.OnSurface += OnSquidSurface;
        SquidController.OnDive += OnSquidDive;
        SquidController.OnJump += OnSquidJump;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSpin = 0f;
        currentRotation = rb.rotation;
        highestAir = 0f;
        _inAir = false;
        _jumped = false;
    }

    private void OnDestroy() {
        SquidController.OnSurface -= OnSquidSurface;
        SquidController.OnDive -= OnSquidDive;
        SquidController.OnJump -= OnSquidJump;
    }

    // Update is called once per frame
    void Update()
    {
        if (_inAir) {
            if (_jumped) {

            }
            AddSpin(rb.rotation - currentRotation);
            currentRotation = rb.rotation;
            highestAir = Mathf.Max(highestAir, tfm.position.y);
        }
    }

    public void AddSpin(float degrees) {
        if (Mathf.Sign(degrees) != Mathf.Sign(currentSpin)) {
            currentSpin = 0f;
        }
        currentSpin += degrees;
        CheckSpin();
    }

    private void CheckSpin() {
        if (Mathf.Abs(currentSpin) > spinThreshold) {
            IterateSpin(Mathf.Sign(currentSpin));
            currentSpin = Mathf.Sign(currentSpin) * (Mathf.Abs(currentSpin) - spinThreshold);
            // OnSpin?.Invoke(this);
        }
    }

    private void IterateSpin(float direction) {
        //Debug.Log(direction);
    }

    void OnSquidSurface(SquidController squid) {
        _inAir = true;
        currentRotation = rb.rotation;
    }

    void OnSquidDive(SquidController squid) {
        if (spinThreshold - Mathf.Abs(currentSpin) <= spinTolerance) {
            IterateSpin(Mathf.Sign(currentSpin));
        }
        _inAir = false;
        _jumped = false;
        currentSpin = 0f;
    }

    void OnSquidJump(SquidController squid) {
        _jumped = true;
    }
}
