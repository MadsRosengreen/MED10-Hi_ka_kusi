using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentBall : MonoBehaviour
{
    public void UnParent(GameObject obj)
    {
        obj.transform.SetParent(null);
    }
}
