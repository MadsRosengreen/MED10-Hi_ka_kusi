using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedStaticGestures : MonoBehaviour
{
    public static SavedStaticGestures Instance;
    public List<HandPose> LeftHandSavedHandPoses;
    public List<HandPose> RightHandSavedHandPoses;
    void Awake() => Instance = this;
}
