using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wrapper : MonoBehaviour
{
    // https://www.youtube.com/watch?v=F8xA4BLE1VI&ab_channel=_napppin__napppin_

    public enum WrapType {
        TOP,
        BOTTOM,
        LEFT,
        RIGHT
    }

    [Header("Wrap specs")]
    public WrapType wrapType;
    public Transform targetTrm;
    public Transform currentTrm;

    private void OnTriggerExit2D(Collider2D collision) {
        if (wrapType == WrapType.LEFT && collision.transform.position.x < this.transform.position.x) Wrap(collision.transform);
        else if (wrapType == WrapType.RIGHT && collision.transform.position.x > this.transform.position.x) Wrap(collision.transform);
        //else if (wrapType == WrapType.TOP)
    }

    private void Wrap(Transform _wrapTrm) {
        if (wrapType == WrapType.LEFT) _wrapTrm.transform.position = new Vector2(targetTrm.position.x - Mathf.Abs(_wrapTrm.transform.position.x - currentTrm.position.x), _wrapTrm.transform.position.y);
        else if (wrapType == WrapType.RIGHT) _wrapTrm.transform.position = new Vector2(targetTrm.position.x + Mathf.Abs(_wrapTrm.transform.position.x - currentTrm.position.x), _wrapTrm.transform.position.y);
        //
    }
}
