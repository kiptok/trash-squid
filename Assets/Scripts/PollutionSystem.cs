using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PollutionSystem : MonoBehaviour
{
    public static event Action<PollutionSystem, int> OnPollutionSet;

    public Material water;
    public Material sky;

    public int _pollution = 0;
    private int _maxPollution = 50;
    private int _pollutionID;
    private int _skyColorAID;
    private int _skyColorBID;
    private int _cleanColorAID;
    private int _cleanColorBID;
    private int _dirtyColorAID;
    private int _dirtyColorBID;
    private int _lightColorAID;
    private int _lightColorBID;
    private float _lerpValuePerSecond = 0.5f;

    public int Pollution
    {
        get => _pollution;
        private set
        {
            //_pollution = Mathf.Clamp(value, 0, _maxPollution);
            _pollution = value;
            OnPollutionSet?.Invoke(this, _pollution);
            //if (_pollution == _maxPollution) SceneManager.LoadScene("LossScene");
        }
    }

    private void Awake()
    {
        TrashSpawner.OnTrashSpawned += OnTrashSpawned;
        BoatTop.OnTrashHit += OnTrashHitBoatTop;
    }

    private void Start()
    {
        Pollution = 0;
        //water.SetFloat("Pollution", _pollution / 100.0f);
        _pollutionID = Shader.PropertyToID("_Pollution");
        _skyColorAID = Shader.PropertyToID("Sky_Color_A");
        _skyColorBID = Shader.PropertyToID("Sky_Color_B");
        _cleanColorAID = Shader.PropertyToID("Clean_Color_A");
        _cleanColorBID = Shader.PropertyToID("Clean_Color_B");
        _dirtyColorAID = Shader.PropertyToID("Dirty_Color_A");
        _dirtyColorBID = Shader.PropertyToID("Dirty_Color_B");
        _lightColorAID = Shader.PropertyToID("Light_Color_A");
        _lightColorBID = Shader.PropertyToID("Light_Color_B");
        // set colors for sky/water
        SetColors();
    }

    private void Update()
    {
        Pollute(water);
        Pollute(sky);
        UpdateColors();
    }

    private void SetColors()
    {
        // semi-random sky color A and derived sky color B
        Color skyColorA = Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f);
        Color.RGBToHSV(skyColorA, out float Ah, out float As, out float Av);
        float minh, maxh, mins, maxs, minv, maxv;
        minh = (Ah - 0.1667f) % 1f;
        maxh = (Ah + 0.1667f) % 1f;
        mins = (As + 0.3333f) % 1f;
        maxs = (As + 0.6667f) % 1f;
        minv = 0.8f;
        maxv = 1f;
        Color skyColorB = Random.ColorHSV(minh, maxh, mins, maxs, minv, maxv);
        sky.SetColor(_skyColorAID, skyColorA);
        sky.SetColor(_skyColorBID, skyColorB);

        Color lightColorA = skyColorA;
        Color lightColorB = skyColorB;

        // light color in water comes from these
        water.SetColor(_lightColorAID, lightColorA);
        water.SetColor(_lightColorBID, lightColorB);
    }

    private void UpdateColors() {
        // if
    }

    private void Pollute(Material material)
    {
        float currentPollution = material.GetFloat(_pollutionID);
        float targetPollution = _pollution / (float)_maxPollution;
        float lerpValue = _lerpValuePerSecond * Time.deltaTime;
        float newPollution = Mathf.Lerp(currentPollution, targetPollution, lerpValue);

        material.SetFloat(_pollutionID, newPollution);
    }

    private void OnDestroy()
    {
        TrashSpawner.OnTrashSpawned -= OnTrashSpawned;
        BoatTop.OnTrashHit -= OnTrashHitBoatTop;
    }

    private void OnTrashHitBoatTop(BoatTop boatTop, Trash trash)
    {
        Pollution -= 1;
        // play sound
    }

    private void OnTrashSpawned(TrashSpawner spawner, Trash trash)
    {
        Pollution += 1;
        //  play sound
    }
}
