using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcMovement : MonoBehaviour
{
    public static NpcMovement Instance;
    [SerializeField] private int NPCNo;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private int wayPoint = 0;
    public bool isOutCircel = true;
    [SerializeField] private bool stopMoving = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartDance();
    }

    public void StartDance()
    {
        stopMoving = false;
        moveSpeed = .3f;
        AnimationFunctions.PlayAnimation(this.gameObject, NPCManager.Instance.npcAnimations[0].name);
        StartCoroutine(DanceDance());
        NPCManager.Instance.danceInProgress = true;
    }

    public void StopDance(int index)
    {
        NPCManager.Instance.npcRefs[index].GetComponent<NpcMovement>().stopMoving = true;
        NPCManager.Instance.danceInProgress = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WayPoint" && isOutCircel)
        {
            wayPoint += 1;
            if (wayPoint == 20)
            {
                wayPoint = 0;
            }
        }
        else if (other.tag == "InnerWayPoint" && !isOutCircel)
        {
            wayPoint += 1;
            if (wayPoint == 20)
            {
                wayPoint = 0;
            }
        }
    }

    public void ResetPos(int NPCNo)
    {
        if (NPCNo == 5)
        {
            // Reset ball position to be at intended player start position
            GameManager.Instance.Ball.transform.SetParent(null);
            GameManager.Instance.Ball.GetComponent<Rigidbody>().isKinematic = true;
            Vector3 newPos = NPCManager.Instance.npcs[0].wayPoints[19 - (3 * NPCNo)].transform.position;
            newPos.y = 0.1f;
            GameManager.Instance.Ball.transform.position = newPos;
        }
        else
        {
            wayPoint = 19 - (3 * NPCNo);
            // Set position to calculated waypoint;
            NPCManager.Instance.npcRefs[NPCNo].transform.position = NPCManager.Instance.npcs[0].wayPoints[wayPoint].transform.position;
            // Set rotation of NPCs to be towards the next waypoint
            NPCManager.Instance.npcRefs[NPCNo].transform.forward = NPCManager.Instance.npcs[0].wayPoints[wayPoint + 1 > 19 ? 0 : wayPoint + 1].transform.position - NPCManager.Instance.npcRefs[NPCNo].transform.position;
            NPCManager.Instance.npcRefs[NPCNo].GetComponent<NpcMovement>().wayPoint = wayPoint;
            //NPCManager.Instance.npcRefs[NPCNo].GetComponent<NpcMovement>().moveSpeed = 0.7f;

        }
        NPCManager.Instance.danceInProgress = false;
    }

    public void ResetRound(int playerIndex)
    {
        int amountOfPlayers = 0;
        for (int i = 0; i < GameManager.Instance.Players.Length; i++)
        {
            if (GameManager.Instance.Players[i] != null)
            {
                amountOfPlayers++;
            }
        }
        int j = 0;
        for (int i = 0; i < GameManager.Instance.Players.Length; i++)
        {
            if (GameManager.Instance.Players[i] != null)
            {
                ++j;
                // Calculate the interval of NPC positions depending on NPCs left
                int tempPos = playerIndex - (j * (NPCManager.Instance.npcs[0].wayPoints.Length / amountOfPlayers));

                if (i < GameManager.Instance.Players.Length - 1)
                {
                    if (tempPos < 0)
                    {
                        tempPos += NPCManager.Instance.npcs[0].wayPoints.Length;
                    }
                    // Set position of NPCs to calculated waypoints
                    NPCManager.Instance.npcs[i].leftIKRig.weight = 0;
                    NPCManager.Instance.npcs[i].rightIKRig.weight = 0;
                    NPCManager.Instance.npcRefs[i].transform.position = NPCManager.Instance.npcs[0].wayPoints[tempPos].transform.position;
                    NPCManager.Instance.npcRefs[i].GetComponent<NpcMovement>().wayPoint = tempPos;
                    // Add rotation towards ball/player waypoint
                    NPCManager.Instance.npcRefs[i].transform.forward = NPCManager.Instance.npcs[0].wayPoints[tempPos + 1 > 19 ? 0 : tempPos + 1].transform.position - NPCManager.Instance.npcRefs[i].transform.position;
                }
                else
                {
                    // Add ball to appear at wayPoint[playerIndex]
                    Vector3 newPlayerStartPos = NPCManager.Instance.npcs[0].wayPoints[playerIndex].transform.position;
                    newPlayerStartPos.y = 0.085f;
                    GameManager.Instance.Ball.GetComponent<Rigidbody>().isKinematic = true;
                    GameManager.Instance.Ball.transform.position = newPlayerStartPos;
                }
            }
        }
        NPCManager.Instance.danceInProgress = false;
    }

    private IEnumerator DanceDance()
    {
        while (!stopMoving)
        {
            Vector3 targetPos = NPCManager.Instance.npcs[NPCNo].wayPoints[wayPoint].transform.position - this.transform.position;
            Vector3 newDir = Vector3.RotateTowards(this.transform.forward, targetPos, rotateSpeed * Time.deltaTime, 0.0f);
            this.transform.rotation = Quaternion.LookRotation(newDir);
            this.transform.position = Vector3.MoveTowards(this.transform.position, NPCManager.Instance.npcs[NPCNo].wayPoints[wayPoint].transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
