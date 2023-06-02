using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabBall : MonoBehaviour
{
    [SerializeField] private Transform gripPoint;
    public int npcIndex;
    private bool rightHand;

    private void Start()
    {
        for (int i = 0; i < NPCManager.Instance.npcs.Length; i++)
        {
            if (this.transform.parent == NPCManager.Instance.npcs[i].leftNPCHand)
            {
                npcIndex = i;
                rightHand = false;
                return;
            }
            if (this.transform.parent == NPCManager.Instance.npcs[i].rightNPCHand)
            {
                npcIndex = i;
                rightHand = true;
                return;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabbable") && GameManager.Instance.ThrowStage != npcIndex)
        {
            if (!other.GetComponentInParent<IsBallGrabbed>().isGrabbed)
            {
                other.GetComponent<Rigidbody>().isKinematic = true;
                other.transform.SetParent(this.transform);
                other.transform.position = this.transform.position;
                other.GetComponentInParent<IsBallGrabbed>().isGrabbed = true;

                StartCoroutine(AnimationFunctions.FromGrabTransition(NPCManager.Instance.npcs[npcIndex].leftIKRig, 0.3f));
                StartCoroutine(AnimationFunctions.FromGrabTransition(NPCManager.Instance.npcs[npcIndex].rightIKRig, 0.3f));

                AnimationFunctions.PlayAnimation(NPCManager.Instance.npcRefs[npcIndex], NPCManager.Instance.npcAnimations[0].name);
                GoodThrowCheck.Instance.GoodThrow = false;
            }

            if (rightHand && other.GetComponentInParent<IsBallGrabbed>().isGrabbed)
            {
                other.GetComponent<Rigidbody>().isKinematic = true;
                other.transform.SetParent(this.transform);
                other.transform.position = this.transform.position;

                StartCoroutine(AnimationFunctions.FromGrabTransition(NPCManager.Instance.npcs[npcIndex].leftIKRig, 0));
                StartCoroutine(AnimationFunctions.FromGrabTransition(NPCManager.Instance.npcs[npcIndex].rightIKRig, 0));

                GameManager.Instance.GameStageStep();
                AnimationFunctions.PlayAnimation(NPCManager.Instance.npcRefs[npcIndex], NPCManager.Instance.npcAnimations[AnimationFunctions.RandomizeTrickAni()].name);
                AnimationFunctions.SlowWalkingNPCs();
                GoodThrowCheck.Instance.GoodThrow = false;
            }
        }
        
     }
}
