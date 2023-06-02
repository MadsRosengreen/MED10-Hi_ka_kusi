using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class RecognizeHandPose : HandPoseRecorder
{
    [SerializeField] private float detectionThreshold = .035f;

    HandPose HandPoseRecognizer(OVRSkeleton handSkeleton, List<HandPose> savedHandPoses)
    {
        HandPose currentHandPose = new HandPose();
        float currentMin = Mathf.Infinity;

        foreach (var handPose in savedHandPoses)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < handSkeleton.BindPoses.Count; i++)
            {
                Vector3 currentData = handSkeleton.transform.InverseTransformPoint(handSkeleton.Bones[i].Transform.position);
                float distance = Vector3.Distance(currentData, handPose.fingerData[i]);
                if (distance > detectionThreshold)
                {
                    isDiscarded = true;
                    break;
                }
                sumDistance += distance;
            }

            if (!isDiscarded && sumDistance < currentMin)
            {
                currentMin = sumDistance;
                currentHandPose = handPose;
            }
        }
        return currentHandPose;
    }
}
