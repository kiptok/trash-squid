using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PollutionSystem : MonoBehaviour
{
    public static event Action<PollutionSystem, int> OnPollutionSet;

    public Material water;
    public Material sky;

    public int _pollution = 0;
    private int _maxPollution = 50;
    private int _pollutionID;

    public int Pollution
    {
        get => _pollution;
        private set
        {
            _pollution = Mathf.Clamp(value, 0, _maxPollution);
            water.SetFloat(_pollutionID, _pollution / (float) _maxPollution);
            sky.SetFloat(_pollutionID, _pollution / (float) _maxPollution);
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
        water.SetFloat(_pollutionID, _pollution / (float) _maxPollution);
        sky.SetFloat(_pollutionID, _pollution / (float) _maxPollution);
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
