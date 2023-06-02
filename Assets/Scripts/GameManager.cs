using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] Players;
    public GameObject Ball, Rig;
    public GameObject Player;
    public TextMeshProUGUI textField;
    [SerializeField] private ThrowBall throwBall;
    [SerializeField] private Transform[] NPCCrowdPos;

    int step = 1;

    [SerializeField] private Image img2Fade;


    [SerializeField] private Vector3[] physicsIterations;

    public int ThrowStage = 4;
    public int CatcherStage = 5;
    public float FailedCatchDistanceThreshold = 0.3f;
    public bool playerIsSpectating = true;
    public bool roundInProgress = false;

    private void Start()
    {
        Players = new GameObject[NPCManager.Instance.npcRefs.Length + 1];
        for (int i = 0; i < Players.Length; i++)
        {
            if (i < NPCManager.Instance.npcRefs.Length)
            {
                Players[i] = NPCManager.Instance.npcRefs[i];
            }
            else
            {
                Players[i] = Player;
            }
        }
        physicsIterations = new Vector3[80];
        Instance = this;

        // Throw the ball to npc 0
        StartCoroutine(ThrowBallAtStart());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameStageStep();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            throwBall.tryTestThrowPlayer = true;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(Fader(0));
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(Fader(1));
        }

        if (!Ball.activeInHierarchy)
        {
            StartCoroutine(Fade2NewScene());
        }
    }

    public void GameStageStep(bool isPlayer = false)
    {
        if (isPlayer)
        {
            ThrowStage = 5;
            for (int i = 0; i < Players.Length; i++)
            {
                // Check the index of the first NPC that is still in the game
                if (Players[i] != null)
                {
                    CatcherStage = i;
                    break;
                }
            }
        }
        else
        {
            ThrowStage = CatcherStage;
            if (playerIsSpectating)
            {
                CatcherStage += step;
                // Reset index when out of bounds
                if (CatcherStage > Players.Length - 2)
                {
                    CatcherStage -= Players.Length - 1;
                }
            }
            else
            {
                // Reset index when out of bounds
                for (int i = ThrowStage + 1; i < Players.Length; i++)
                {
                    // Check the index of the first NPC that is still in the game
                    if (Players[i] != null)
                    {
                        CatcherStage = i;
                        break;
                    }
                }
            }

        }
    }

    public void WhoThrows()
    {
        for (int i = 0; i < NPCManager.Instance.npcRefs.Length; i++)
        {
            if (NPCManager.Instance.npcRefs[i] != null)
            {
                NPCManager.Instance.npcRefs[i].GetComponent<NpcMovement>().moveSpeed = 0.7f;
            }
        }

        if (CatcherStage == Players.Length - 1 && !playerIsSpectating)
        {
            // NPC-2-Player
            //Make Targets[throwStage] throw 'Ball'
            //Targets[throwStage].GetComponent<Animator>().Play("Throw");
            //Throw to Player.transform.position;
            NPCThrowBall.Instance.NPCThrow(Ball, Players[CatcherStage].transform.FindChildRecursive("Ball Targets").GetComponentsInChildren<Transform>(), 62f, NPCManager.Instance.line, physicsIterations);
            //AnimationFunctions.StopAnimationTransition(NPCManager.Instance.npcRefs[ThrowStage], NPCManager.Instance.npcAnimations[AnimationFunctions.trickIndex].name);
            //Make 'Ball' detach
        }

        if (ThrowStage == Players.Length - 1 && !playerIsSpectating)
        {
            //Player throws ball
            //Throw to Targets[catcherStage].transform.position;
            //NPCManager.Instance.CatcherIndex = catcherStage;
            //Targets[catchStage].GetComponent<Animator>().Play("Catch")
            //Make 'Ball' detach
        }
        else if (CatcherStage != Players.Length - 1)
        {
            // NPC-2-NPC
            //Make Targets[throwStage] throw 'Ball'
            //Throw to Targets[catcherStage].transform.position
            NPCThrowBall.Instance.NPCThrow(Ball, NPCManager.Instance.npcs[CatcherStage].ballTargets, 62f, NPCManager.Instance.line, physicsIterations);
            //AnimationFunctions.StopAnimationTransition(NPCManager.Instance.npcRefs[ThrowStage], NPCManager.Instance.npcAnimations[AnimationFunctions.trickIndex].name);
        }
    }

    public IEnumerator EliminateNPC(int playersIndex)
    {
        if (playersIndex == Players.Length - 1)
        {
            // Game over (Player is out)
            // Display "You are out" in the UI
            setText("Unfortunately you have been eliminated  \n Feel free to try again!");
            yield return new WaitForSeconds(6f);
            StartCoroutine(Fader(0));
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(0);
            yield break;
        }
        else
        {
            // Stop all NPCs from dancing and remaining NPCs play happy animations
            int a = 0;
            for (int i = 0; i < NPCManager.Instance.npcRefs.Length; i++)
            {
                if (NPCManager.Instance.npcRefs[i] == null) a++;
                else
                {

                    // Other NPCs still in the game does a little thing and goes to idle
                    NpcMovement.Instance.StopDance(i);
                    NPCManager.Instance.npcs[i].leftIKRig.weight = 0;
                    NPCManager.Instance.npcs[i].rightIKRig.weight = 0;
                    if (i == playersIndex)
                    {
                        // Loser NPC plays a sad animation
                        AnimationFunctions.PlayAnimation(NPCManager.Instance.npcRefs[playersIndex], NPCManager.Instance.npcAnimations[3].name);
                    }   // Other NPCs play happy animation
                    else AnimationFunctions.PlayAnimation(Players[i], NPCManager.Instance.npcAnimations[AnimationFunctions.RandomizeTrickAni(3)].name);
                }
            }

            // If player is the last woman standing, they win!
            if (a == 4)
            {
                //Game over (Player wins)
                StartCoroutine(TransitionToNextRound(playersIndex));
                yield return new WaitForSecondsRealtime(2f);
                setText("You are the last person standing \n Congratulation!");
                yield return new WaitForSecondsRealtime(10f);
                Fade2NewScene();
            }
            else
            {
                // We wait for animations to finish and start a new round;
                yield return new WaitForSeconds(2f);
                StartCoroutine(TransitionToNextRound(playersIndex));
            }
        }
    }

    private IEnumerator ThrowBallAtStart()
    {
        yield return new WaitForSecondsRealtime(.3f);
        throwBall.tryTestThrowPlayer = true;
    }

    public IEnumerator TransitionToNextRound(int playersIndex)
    {
        // Fade to black
        StartCoroutine(Fader(0));
        yield return new WaitForSeconds(2f);

        // Move eliminated NPC to crowd and start cheering animation
        // Also removes eliminated NPC from Players[] AND NPCManager.npcRefs[]
        OnElimRemoveNPC(playersIndex);


        // Reposition competing NPCs so their spacing is even - if possible make the opening spot nearest the player - and start their idle animation.
        // Postion ball is in player's spot.
        NPCManager.Instance.NPCEliminated();
        throwBall.CatchAssistOn(false);

        // Fade from black and await the player starting the game by picking up the ball - start npcs' dancing animation with some delay - and throwing the ball.
        StartCoroutine(Fader(1));
    }

    private IEnumerator Fader(int startAlpha)
    {
        if (startAlpha == 0)
        {
            // Fade out
            float black = 0f;
            while (img2Fade.color.a <= 1)
            {
                black += 1f * Time.deltaTime;
                img2Fade.color = new Vector4(0, 0, 0, black);
                yield return null;
            }
        }
        else
        {
            // Fade in
            float black = 1f;
            while (img2Fade.color.a >= 0)
            {
                black -= 1f * Time.deltaTime;
                img2Fade.color = new Vector4(0, 0, 0, black);
                yield return null;
            }
        }
    }

    public void OnElimRemoveNPC(int npcIndex)
    {
        GameObject elimNPC = NPCManager.Instance.npcRefs[npcIndex];
        elimNPC.transform.position = NPCCrowdPos[npcIndex].position;
        elimNPC.transform.rotation = NPCCrowdPos[npcIndex].rotation;
        AnimationFunctions.PlayAnimation(elimNPC, "idleClapping");
        //AnimationFunctions.StopAnimationTransition(elimNPC, "gameLostSadge");
        Players[npcIndex] = null;
        NPCManager.Instance.npcRefs[npcIndex] = null;
    }

    public IEnumerator Fade2NewScene()
    {
        StartCoroutine(Fader(0));
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
        yield break;
    }
    public void setText(string text = "")
    {
        textField.text = text;
    }

    public IEnumerator MovePlayer()
    {
        StartCoroutine(Fader(0));
        yield return new WaitForSeconds(2f);
        StopCoroutine("ManageAudio");
        AudioManager.Instance.ResetSub();
        Rig.transform.position += new Vector3(2.7f, 0f, -4.2f);

        NPCManager.Instance.StopAllDance();

        // Reset NPC positions and set their animations to one of two idles
        for (int i = 0; i < Players.Length; i++)
        {
            NpcMovement.Instance.ResetPos(i);
            if (i != 5)
            {
                NPCManager.Instance.npcs[i].leftIKRig.weight = 0;
                NPCManager.Instance.npcs[i].rightIKRig.weight = 0;
                AnimationFunctions.PlayAnimation(NPCManager.Instance.npcRefs[i], NPCManager.Instance.npcAnimations[AnimationFunctions.RandomizeTrickAni(4)].name);
            }
        }
        yield return new WaitForSeconds(.5f);
        playerIsSpectating = false;
        StartCoroutine(Fader(1));
    }
}