using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


[System.Serializable]
public struct NPC
{
    [SerializeField] public GameObject leftInterceptor, rightInterceptor;
    [SerializeField] public Transform[] ballTargets, wayPoints;
    [SerializeField] public Transform rightNPCHand, leftNPCHand, rightShoulderNPC, leftShoulderNPC;
    [SerializeField] public Rig rightIKRig, leftIKRig;
}

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;

    [SerializeField] public NPC[] npcs;
    [SerializeField] public GameObject[] npcRefs;

    [SerializeField] private GameObject ballManager;
    public GameObject questAni;

    [SerializeField] public AnimationClip[] npcAnimations;

    [SerializeField] public float interceptDistanceThreshold, interceptionTime;

    private Vector3 closestTrajecPosRight, closestTrajecPosLeft;
    public LineRenderer line;
    public int CatcherIndex = 0;
    public bool danceInProgress = true;

    void Awake()
    {
        for (int i = 0; i < npcRefs.Length; i++)
        {
            npcs[i].ballTargets = new Transform[6];
            npcs[i].wayPoints = new Transform[20];

            Rig[] ikRigs = npcRefs[i].GetComponentsInChildren<Rig>();

            for (int j = 0; j < ikRigs.Length; j++)
            {
                if (ikRigs[j].gameObject.name.Contains("Left"))
                {
                    npcs[i].leftInterceptor = ikRigs[j].transform.FindChildRecursive("Target").gameObject;
                    npcs[i].leftIKRig = ikRigs[j];
                    npcs[i].leftNPCHand = npcRefs[i].transform.FindChildRecursive("hand.L");
                    npcs[i].leftShoulderNPC = npcRefs[i].transform.FindChildRecursive("upper_arm.L");
                }
                else if (ikRigs[j].gameObject.name.Contains("Right"))
                {
                    npcs[i].rightInterceptor = ikRigs[j].transform.FindChildRecursive("Target").gameObject;
                    npcs[i].rightIKRig = ikRigs[j];
                    npcs[i].rightNPCHand = npcRefs[i].transform.FindChildRecursive("hand.R");
                    npcs[i].rightShoulderNPC = npcRefs[i].transform.FindChildRecursive("upper_arm.R");
                }
            }

            for (int k = 0; k < npcs[i].ballTargets.Length; k++)
            {
                npcs[i].ballTargets[k] = npcRefs[i].transform.FindChildRecursive("Ball Targets").GetChild(k);
            }
        }
        FindWaypoints();
        line = ballManager.GetComponent<LineRenderer>();
        Instance = this;
    }

    private void FixedUpdate()
    {
        if (SimulateBallTrajectory.SimComplete)
        {
            CatcherIndex = GameManager.Instance.CatcherStage;
            if (CatcherIndex < GameManager.Instance.Players.Length - 1)
            {
                closestTrajecPosRight = InterceptBall.Instance.CalcClosestTrajectoryPos(npcs[CatcherIndex].rightShoulderNPC, line, interceptDistanceThreshold);
                closestTrajecPosLeft = InterceptBall.Instance.CalcClosestTrajectoryPos(npcs[CatcherIndex].leftShoulderNPC, line, interceptDistanceThreshold);


                // Right hand intercept
                if (closestTrajecPosRight != Vector3.zero)
                {
                    AnimationFunctions.ToGrabTransition(npcs[CatcherIndex].rightInterceptor.transform, npcs[CatcherIndex].rightNPCHand, npcs[CatcherIndex].rightIKRig);

                    StartCoroutine(InterceptBall.Instance.Intercept(npcs[CatcherIndex].rightInterceptor.transform, closestTrajecPosRight, interceptionTime, npcs[CatcherIndex].rightNPCHand));

                    SimulateBallTrajectory.SimComplete = false;
                }

                // Left hand intercept
                if (closestTrajecPosLeft != Vector3.zero)
                {
                    AnimationFunctions.ToGrabTransition(npcs[CatcherIndex].leftInterceptor.transform, npcs[CatcherIndex].leftNPCHand, npcs[CatcherIndex].leftIKRig);

                    StartCoroutine(InterceptBall.Instance.Intercept(npcs[CatcherIndex].leftInterceptor.transform, closestTrajecPosLeft, interceptionTime, npcs[CatcherIndex].leftNPCHand));

                    SimulateBallTrajectory.SimComplete = false;
                }

            }

            SimulateBallTrajectory.SimComplete = false;
        }
    }

    private void FindWaypoints()
    {
        for (int i = 0; i < npcs.Length; i++)
        {
            if (npcRefs[i] != null)
            {
                for (int j = 0; j < npcs[i].wayPoints.Length; j++)
                {
                    if (npcRefs[i].GetComponent<NpcMovement>().isOutCircel)
                        npcs[i].wayPoints[j] = GameObject.Find("OuterWaypointParent").GetComponent<Transform>().GetChild(j);
                    else
                        npcs[i].wayPoints[j] = GameObject.Find("InnerWaypointParent").GetComponent<Transform>().GetChild(j);
                }
            }
        }
    }

    public void StopAllDance()
    {
        for (int i = 0; i < npcRefs.Length; i++)
        {
            if (npcRefs[i] != null)
            {
                npcRefs[i].GetComponent<NpcMovement>().StopDance(i);
            }
        }
    }

    public void DanceCircel()
    {
        int tempInt = 0;
        for (int i = 0; i < npcRefs.Length; i++)
        {
            if (npcRefs[i] != null)
            {
                tempInt++;
                npcRefs[i].GetComponent<NpcMovement>().StartDance();
            }
        }
        if (tempInt < 3)
        {
            for (int i = 0; i < npcRefs.Length; i++)
            {
                if (npcRefs[i] != null)
                {
                    if (npcRefs[i].GetComponent<NpcMovement>().isOutCircel)
                    {
                        npcRefs[i].GetComponent<NpcMovement>().isOutCircel = false;
                    }
                }
            }
            FindWaypoints();
        }
    }

    public void NPCEliminated()
    {
        Vector3 tempV3 = GameManager.Instance.Player.transform.position;
        float shortestDist = Mathf.Infinity;
        int index = 0;
        tempV3.y = 0;
        for (int i = 0; i < npcs[0].wayPoints.Length; i++)
        {
            float dist = Vector3.Distance(tempV3, npcs[0].wayPoints[i].transform.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                index = i;
            }
        }
        NpcMovement.Instance.ResetRound(index);
    }
}
