using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushSwitcheroo : MonoBehaviour
{
    public ThrowBall throwBall;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Grabbable" && other.tag != "MainCamera")
        {
            throwBall.isUsingController = !throwBall.isUsingController;
            StartCoroutine(AudioManager.Instance.ControllerText(throwBall.isUsingController));//controlTrigger.isUsingController);
        }
    }
}
