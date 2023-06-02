using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGeneral : MonoBehaviour
{
    public static PlayerGeneral Instance;
    public bool hasTouched = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "MoveTrigger" && !hasTouched)
        {
            StartCoroutine(GameManager.Instance.MovePlayer());
            hasTouched = true;
            StartCoroutine(DelayBoolReset());
        }
    }

    private IEnumerator DelayBoolReset()
    {
        yield return new WaitForSecondsRealtime(3f);
        hasTouched = false;
    }
}
