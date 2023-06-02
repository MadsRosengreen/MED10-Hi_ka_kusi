using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchAssist : MonoBehaviour
{
    [SerializeField] Transform gripPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable"))
        {
            Debug.Log("Catch assist hit the target!");
            other.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.position = gripPoint.position;
            other.transform.SetParent(gripPoint.transform);
        }
    }
}
