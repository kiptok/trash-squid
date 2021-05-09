using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour {

    private AudioSource music;
    private AudioLowPassFilter lpf;

    private bool filtering;

    private void Awake() {
        SquidController.OnDive += OnSquidDive;
        SquidController.OnSurface += OnSquidSurface;
    }

    private void OnDestroy() {
        SquidController.OnDive -= OnSquidDive;
        SquidController.OnSurface -= OnSquidSurface;
    }

    void Start() {
        filtering = true;
    }

    // Update is called once per frame
    void Update() {
        /*if (filtering) {

        }*/
    }

    private void OnSquidDive(SquidController squid) {
        filtering = true;
    }

    private void OnSquidSurface(SquidController squid) {
        filtering = false;
    }
}
