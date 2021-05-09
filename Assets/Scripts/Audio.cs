using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour {

    public SquidController squid;
    private AudioSource music;
    private AudioLowPassFilter lpf;

    public float minCutoff;
    public float maxCutoff;

    private bool filtering;

    private void Awake() {
        music = GetComponent<AudioSource>();
        lpf = GetComponent<AudioLowPassFilter>();
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
        if (filtering) {
//            lpf.cutoffFrequency = maxCutoff * (minCutoff / maxCutoff) * Mathf.Pow()
            lpf.cutoffFrequency = Mathf.Lerp(maxCutoff, minCutoff, squid.transform.position.y / -100f);
        }
    }

    private void OnSquidDive(SquidController sq) {
        filtering = true;
    }

    private void OnSquidSurface(SquidController sq) {
        filtering = false;
    }
}
