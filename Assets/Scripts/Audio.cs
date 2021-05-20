using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour {

    public SquidController squid;

    //EventRef presents the designer with UI for selecting events
    [FMODUnity.EventRef]
    public string GameMusicEvent = "";

    //EventInstance allows us to manage an event over its lifetime
    FMOD.Studio.EventInstance gameMusic;

    private int squidY;
    FMOD.Studio.PARAMETER_ID squidYParameterId;

    private bool _inWater;

    private Transform squidTransform;
    private Rigidbody2D squidRigidBody;

    private void Awake() {
        SquidController.OnDive += OnSquidDive;
        SquidController.OnSurface += OnSquidSurface;
    }

    void Start() {
        squidRigidBody = squid.GetComponent<Rigidbody2D>();
        squidTransform = squid.GetComponent<Transform>();

        _inWater = true;

        //Create an instance of an Event and manually start it
        gameMusic = FMODUnity.RuntimeManager.CreateInstance(GameMusicEvent);
        gameMusic.start();

        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(gameMusic, squid.GetComponent<Transform>(), squid.GetComponent<Rigidbody2D>())

        // Cache a handle to the "squidY" parameter for usage in Update()
        FMOD.Studio.EventDescription squidYEventDescription;
        gameMusic.getDescription(out squidYEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION squidYParameterDescription;
        squidYEventDescription.getParameterDescriptionByName("SquidY", out squidYParameterDescription);
        squidYParameterId = squidYParameterDescription.id;
    }

    private void OnDestroy() {
        gameMusic.release();
        SquidController.OnDive -= OnSquidDive;
        SquidController.OnSurface -= OnSquidSurface;
    }

    // Update is called once per frame
    void Update() {
        if (_inWater) {
            gameMusic.setParameterByID(squidYParameterId, squid.transform.position.y);
            Debug.Log(squid.transform.position.y);
        }
    }

    private void OnSquidDive(SquidController sq) {
        _inWater = true;
    }

    private void OnSquidSurface(SquidController sq) {
        _inWater = false;
    }
}
