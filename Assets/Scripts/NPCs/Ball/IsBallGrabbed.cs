using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsBallGrabbed : MonoBehaviour
{
    public static IsBallGrabbed Instance;
    public bool isGrabbed;

    private void Awake()
    {
        isGrabbed = false;
        Instance = this;
    }
    public IEnumerator SetGrabbedFalse()
    {
        yield return new WaitForSeconds(.25f);
        isGrabbed = false;
    }
}
