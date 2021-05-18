using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickController : MonoBehaviour
{
    //public SquidController squid;

    private Transform tfm;
    private float currentSpin; // spin maybe for separate script
    private float spinThreshold = 180.0f;
    private float spinTolerance = 25.0f;
    private bool _inAir;
    private bool _jumped;

    public static event Action<SquidController> TrickCompleted;

    private void Awake() {
        tfm = this.transform;
        SquidController.OnSurface += OnSquidSurface;
        SquidController.OnDive += OnSquidDive;
        SquidController.OnJump += OnSquidJump;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        
    }

    void OnSquidSurface(SquidController squid) {
        _inAir = true;
    }

    void OnSquidDive(SquidController squid) {
        _inAir = false;
        _jumped = false;
    }

    void OnSquidJump(SquidController squid) {
        _jumped = true;
    }
}
