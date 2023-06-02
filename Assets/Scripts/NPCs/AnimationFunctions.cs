using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class AnimationFunctions : MonoBehaviour
{
    public static int trickIndex = 1;
    public static void ToGrabTransition(Transform ikTarget, Transform npcHand, Rig ikRig)
    {
        // Set IK target's position & rotation = given NPC's hand position & rotation.
        // Set IK Rig weight = 1

        ikTarget.position = npcHand.position;
        ikTarget.rotation = npcHand.rotation;
        ikRig.weight = 1;
        ikRig.GetComponentInChildren<TwistCorrection>().weight = 1;
    }


    public static IEnumerator FromGrabTransition(Rig ikRig, float transitionTime)
    {
        // Transition IK Rig weight towards 0, gradually;
        float startTime = Time.time;
        ikRig.GetComponentInChildren<TwistCorrection>().weight = 0;
        while (Time.time < startTime + transitionTime)
        {
            ikRig.weight = Mathf.Lerp(1, 0, (Time.time - startTime) / transitionTime);
            yield return null;
        }
        ikRig.weight = 0;
    }



    public static void PlayAnimation(GameObject npc, string whichAni, bool aniStage = false, float startTime = 0.0f)
    {
        Animator npcAnim = npc.GetComponent<Animator>();
        if (!aniStage) npcAnim.Play(whichAni, -1, startTime);
        else npcAnim.SetBool(whichAni, !npcAnim.GetBool(whichAni));
    }

    public static void SlowWalkingNPCs()
    {
        for (int i = 0; i < NPCManager.Instance.npcRefs.Length; i++)
        {
            if (NPCManager.Instance.npcRefs[i] != null)
            {
                NPCManager.Instance.npcRefs[i].GetComponent<NpcMovement>().moveSpeed = .3f;
            }
        }
    }

    public static void PlayNPCReady4NewRound()
    {
        if (GameManager.Instance.roundInProgress)
        {
            return;
        }
        for (int i = 0; i < NPCManager.Instance.npcRefs.Length; i++)
        {
            if (NPCManager.Instance.npcRefs[i] != null)
            {
                PlayAnimation(NPCManager.Instance.npcRefs[i], "idleClapping");
            }
        }
    }

    public static void StopAnimationTransition(GameObject npc, string whichAni)
    {
        Animator npcAnim = npc.GetComponent<Animator>();
        npcAnim.SetBool(whichAni, !npcAnim.GetBool(whichAni));
    }

    public static int RandomizeTrickAni(int plusIndex = 0)
    {
        return trickIndex = Random.Range(1 + plusIndex, 3 + plusIndex);
    }

    public static string RandomDual(string one = "", string two = "")
    {
        if (Random.Range(0, 1) == 0) return one;
        else return two;
    }

}
