using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrickSystem : MonoBehaviour
{
    public SquidController squid;

    public static event Action<SquidController> TrickCompleted;

    private void Awake() {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnDestroy() {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnSquidSurface() {

    }
}
