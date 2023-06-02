using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct HandPose
{
    public string name;
    public List<Vector3> fingerData;
    public Vector3 orientation;
}