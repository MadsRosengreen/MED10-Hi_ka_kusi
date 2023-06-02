using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnGroundHit : MonoBehaviour
{
    private bool canHitGround = true;
    public AudioSource ballAud;
    private void OnCollisionEnter(Collision collision)
    {
        // If the ball touches the ground and the player has not touched the collider to move from start- to play area
        if (collision.collider.CompareTag("Grabbable") && canHitGround && !CanGrab.Instance.isGrabbing && !PlayerGeneral.Instance.hasTouched)
        {
            ballAud.Play();
            StartCoroutine(ReEnableOnGroundHitAfterDelay(4));
            if (GameManager.Instance.playerIsSpectating)
            {
                StartCoroutine(GameManager.Instance.Fade2NewScene());
            }

            if (GameManager.Instance.CatcherStage < NPCManager.Instance.npcRefs.Length)
            {
                if (IsWithinReachOnGroundHit(collision.transform, NPCManager.Instance.npcRefs[GameManager.Instance.CatcherStage].transform, GameManager.Instance.FailedCatchDistanceThreshold))
                {
                    StartCoroutine(GameManager.Instance.EliminateNPC(GameManager.Instance.CatcherStage)); // Catcher is out
                    Debug.Log("NPC number " + GameManager.Instance.CatcherStage + " is out");
                }
                else
                {
                    StartCoroutine(GameManager.Instance.EliminateNPC(GameManager.Instance.ThrowStage)); // Thrower is out
                    Debug.Log("NPC number " + GameManager.Instance.ThrowStage + " is out");
                }
            }
            else
            {
                if (IsWithinReachOnGroundHit(collision.transform, GameManager.Instance.Player.transform, GameManager.Instance.FailedCatchDistanceThreshold))
                {
                    StartCoroutine(GameManager.Instance.EliminateNPC(GameManager.Instance.CatcherStage)); // Player is out
                    Debug.Log("The player is out");
                }
                else
                {
                    StartCoroutine(GameManager.Instance.EliminateNPC(GameManager.Instance.ThrowStage)); // Thrower is out
                    Debug.Log("NPC number " + GameManager.Instance.ThrowStage + " is out");
                }
            }

        }
    }

    private IEnumerator ReEnableOnGroundHitAfterDelay(float delay)
    {
        canHitGround = false;
        yield return new WaitForSeconds(delay);
        canHitGround = true;
    }

    private bool IsWithinReachOnGroundHit(Transform touchDown, Transform participant, float distanceThreshold)
    {
        bool catcherFailed;
        Vector3 playerTrans = participant.position;
        playerTrans.y = 0;

        float dist = Vector3.Distance(playerTrans, touchDown.position);
        if (dist < distanceThreshold || GoodThrowCheck.Instance.GoodThrow == true)
        {
            return catcherFailed = true;
            GoodThrowCheck.Instance.GoodThrow = false;
        }
        else return catcherFailed = false;
    }
}
