using UnityEngine;

public class HandsReference : MonoBehaviour
{
    public static HandsReference Instance;

    [SerializeField] private GameObject OVRPrefabRightHand;
    [SerializeField] private GameObject OVRPrefabLeftHand;

    public OVRSkeleton RightSkeleton { get; private set; }
    public OVRSkeleton LeftSkeleton { get; private set; }
    public OVRHand RightHand { get; private set; }
    public OVRHand LeftHand { get; private set; }

    private void Awake()
    {
        RightSkeleton = OVRPrefabRightHand.GetComponent<OVRSkeleton>();
        LeftSkeleton = OVRPrefabLeftHand.GetComponent<OVRSkeleton>();
        RightHand = OVRPrefabRightHand.GetComponent<OVRHand>();
        LeftHand = OVRPrefabLeftHand.GetComponent<OVRHand>();
        Instance = this;
    }
}
