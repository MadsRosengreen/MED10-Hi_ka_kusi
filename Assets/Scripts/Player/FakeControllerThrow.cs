using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeControllerThrow : MonoBehaviour
{
    [SerializeField] private ThrowBall throwBall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Grabbable")
        {
            throwBall.canFakeThrow = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Grabbable")
        {
            throwBall.canFakeThrow = false;
        }
    }
}
