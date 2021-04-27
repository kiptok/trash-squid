using System;
using UnityEngine;

public class BoatTop : MonoBehaviour
{
    public static event Action<BoatTop, Trash> OnTrashHit;

    public AudioSource trashGetSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTrashCollision(collision);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    HandleTrashCollision(collision);
    //}

    private void HandleTrashCollision(Collider2D collision)
    {
        var trash = collision.gameObject.GetComponent<Trash>();
        if (trash == null || !trash.GetComponent<Rigidbody2D>().simulated) return;
        trashGetSound.Play();
        OnTrashHit?.Invoke(this, trash);
    }
}
