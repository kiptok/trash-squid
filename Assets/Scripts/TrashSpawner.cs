using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrashSpawner : MonoBehaviour
{
    public static event Action<TrashSpawner, Trash> OnTrashSpawned;
    public List<GameObject> _trashPrefabs = new List<GameObject>();

    public float _minTimeBetweenSpawn = 2.0f;
    public float _maxTimeBetweenSpawn = 8.0f;
    public AudioSource trashSpawnSound;

    //[SerializeField] private GameObject _trashPrefab = null;
    private float _timeBetweenSpawn;

    private void Awake()
    {
        StartCoroutine(SpawnTrashAtRegularInterval());
    }

    private IEnumerator SpawnTrashAtRegularInterval()
    {
        while (true)
        {
            SpawnTrash();
            _timeBetweenSpawn = Random.Range(_minTimeBetweenSpawn, _maxTimeBetweenSpawn);
            yield return new WaitForSeconds(_timeBetweenSpawn);
        }
    }

    private void SpawnTrash()
    {
        GameObject trashPrefab = _trashPrefabs[Random.Range(0, _trashPrefabs.Count)];
        var go = Instantiate(trashPrefab, transform);
        go.GetComponent<Rigidbody2D>().velocity = new Vector3(Random.Range(-21f, 21f), Random.Range(0f, 18f), 0f);
        go.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1f, 1f));
        //go.transform.position = new Vector3(Random.Range(-50f, 50f), 0);
        var trash = go.GetComponent<Trash>();
        OnTrashSpawned?.Invoke(this, trash);
        // play trash sound
    }
}
