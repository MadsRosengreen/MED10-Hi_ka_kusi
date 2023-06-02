using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using System;
using UnityEngine.Animations.Rigging;

public class InterceptBall : MonoBehaviour
{
    public static InterceptBall Instance;
    [SerializeField]
    private float rotationSpeed = 180f;
    private Vector3 palmDirection;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator Intercept(Transform source, Vector3 target, float interceptTime, Transform npcHand)
    {
        float startTime = Time.time;
        while (Time.time < startTime + interceptTime)
        {
            source.position = Vector3.Lerp(source.position, target, (Time.time - startTime) / interceptTime);

            //TODO: Rotate source towards the incoming ball
            Vector3 targetDirection = palmDirection - source.up; //This need altering to find the vector going from where the ball is coming from
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

            npcHand.rotation = Quaternion.RotateTowards(npcHand.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            //Vector3 newDirection = Vector3.RotateTowards(source.right, -ballDirection, (Time.time - startTime) / interceptTime, 10.0f);
            //transform.rotation = Quaternion.LookRotation(newDirection);
            yield return null;
        }
        source.position = npcHand.position;
        source.rotation = npcHand.rotation;
    }

    public Vector3 CalcClosestTrajectoryPos(Transform interceptor, LineRenderer trajectory, float distanceThreshold)
    {
        float shortestDist = Mathf.Infinity;
        int trajectoryIndex = 0;
        for (int i = 5; i < trajectory.positionCount; i++)
        {
            float newDist = Vector3.Distance(interceptor.position, trajectory.GetPosition(i));
            if (newDist > distanceThreshold)
            {
                continue;
            }
            else if (newDist < shortestDist)
            {
                shortestDist = newDist;
                trajectoryIndex = i;
            }
        }
        if (shortestDist < Mathf.Infinity)
        {
            palmDirection = CalcPalmDirection(trajectory.GetPosition(trajectoryIndex), trajectory.GetPosition(trajectoryIndex - 1));
            return trajectory.GetPosition(trajectoryIndex);
        }
        else
        {
            return Vector3.zero;
        }
    }

    private Vector3 CalcPalmDirection(Vector3 interceptPoint, Vector3 previousPoint)
    {
        Vector3 palmDirection = interceptPoint - previousPoint;
        return palmDirection;
    }

    public float GetSmallestInterceptorDistance(Transform interceptor, int trajectoryIndex, LineRenderer trajectory, float currentInterceptDistance)
    {
        float dist = Vector3.Distance(interceptor.position, trajectory.GetPosition(trajectoryIndex));
        if (currentInterceptDistance == Mathf.Infinity || currentInterceptDistance > dist)
        {
            return dist;
        }
        else
        {
            return currentInterceptDistance;
        }
    }
}
