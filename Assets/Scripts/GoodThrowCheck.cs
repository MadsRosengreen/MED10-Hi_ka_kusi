using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodThrowCheck : MonoBehaviour
{
    public static GoodThrowCheck Instance;
    public bool GoodThrow = false;

    private void Start()
    {
        Instance = this;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AcceptThrow") GoodThrow = true;
    }
}
