using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPoseRecorder: MonoBehaviour
{
    [SerializeField] private E_WhichHand whichHand;

    //######### Move update to another script - probably ############
    void Update()
    {
        if (!HandsReference.Instance.LeftSkeleton.IsInitialized || !HandsReference.Instance.RightSkeleton.IsInitialized)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RecordHandPose(((int)whichHand));
        }
    }


    public void RecordHandPose(int hand)
    {

        // Creates new static gesture
        HandPose newGesture = new HandPose();

        // Sets the name of the new gesture
        newGesture.name = "New Gesture";

        // Creates a list of 3D vectors we use to save the individual bones' positions
        List<Vector3> newFingerData = new List<Vector3>();

        // Left hand
        if (hand == 1 || hand == 3)
        {
            foreach (var bone in HandsReference.Instance.LeftSkeleton.Bones)
            {
                // Adds each bone's position from the InverseTransformPoint(wrist) to the Vector3 list we just created
                newFingerData.Add(HandsReference.Instance.LeftSkeleton.transform.InverseTransformPoint(bone.Transform.position));

            }

            // Sets the new gesture's finger data equal to the fingerdata we just set above
            newGesture.fingerData = newFingerData;

            // Adds the new gesture to our list of saved gestures
            SavedStaticGestures.Instance.LeftHandSavedHandPoses.Add(newGesture);
        }

        // Right hand
        if (hand == 2 || hand == 3)
        {
            foreach (var bone in HandsReference.Instance.RightSkeleton.Bones)
            {
                // Adds each bone's position from the InverseTransformPoint(wrist) to the Vector3 list we just created
                newFingerData.Add(HandsReference.Instance.RightSkeleton.transform.InverseTransformPoint(bone.Transform.position));

            }

            // Sets the new gesture's finger data equal to the fingerdata we just set above
            newGesture.fingerData = newFingerData;

            // Adds the new gesture to our list of saved gestures
            SavedStaticGestures.Instance.RightHandSavedHandPoses.Add(newGesture);
        }
    }
}
