using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using UnityEngine.Animations.Rigging;
using Oculus.Interaction.Throw;

public class ThrowBall : MonoBehaviour
{
    [SerializeField] private GameObject ball, catchAssistR, catchAssistL;
    [SerializeField] private Vector3 startPos;
    [SerializeField] public LineRenderer line;

    private PhysicsGrabbable physicsGrabbable;

    public bool tryThrowPlayer, tryReset;
    public bool tryTestThrowPlayer;
    public bool isUsingController = true;
    public bool canFakeThrow = false;
    private bool throwInProgress = false;
    private bool ballHandIsMissing;
    [SerializeField] private float controllerReleasePressure = 0.1f;

    [Range(30, 70)]
    [SerializeField] private float angle = 40f;
    [SerializeField] private float timeHandNotTracked = .3f;
    private float time = 0;

    private Vector3 prevControllerPos = Vector3.zero;
    [SerializeField] Vector3[] catchAssistOffset;
    public Vector3 linearVelocity = Vector3.zero;
    public Vector3[] physicsIterationPositions;

    // Start is called before the first frame update
    void Start()
    {
        physicsIterationPositions = new Vector3[80];
        startPos = ball.transform.position;
        physicsGrabbable = ball.GetComponent<PhysicsGrabbable>();
        Random.InitState(System.DateTime.Now.Millisecond);
        isUsingController = true;
    }


    private void Update()
    {
        // Check which and wether the hand disappears while holding the ball
        if (CanGrab.Instance.isGrabbing && isUsingController && !tryThrowPlayer) // If we are grabbing
        {
            throwInProgress = false;
            if (CanGrab.Instance.Hand.CompareTag("RightHand")) // Using the right hand
            {
                if (canFakeThrow) // We check if the right hand disappears
                {
                    ballHandIsMissing = true;
                    if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < controllerReleasePressure || (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) < controllerReleasePressure && OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) < controllerReleasePressure))
                    {
                        tryThrowPlayer = true; // And then try to throw the ball
                        throwInProgress = true;
                    }
                }
                else
                {
                    ballHandIsMissing = false;
                }
            }
            else if (CanGrab.Instance.Hand.CompareTag("LeftHand")) // Using the left hand
            {
                if (canFakeThrow) // We check if the left hand disappears
                {
                    ballHandIsMissing = true;
                    if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) < controllerReleasePressure || (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) < controllerReleasePressure && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) < controllerReleasePressure))
                    {
                        tryThrowPlayer = true; // And then try to throw the ball
                        throwInProgress = true;
                    }
                    else
                    {
                        ballHandIsMissing = false;
                    }
                }
            }
        }



        // Check which and wether the hand disappears while holding the ball
        if (CanGrab.Instance.isGrabbing && !isUsingController && !tryThrowPlayer) // If we are grabbing
        {
            throwInProgress = false;
            if (CanGrab.Instance.Hand.CompareTag("RightHand")) // Using the right hand
            {
                if (!HandsReference.Instance.RightHand.IsTracked) // We check if the right hand disappears
                {
                    ballHandIsMissing = true;
                }
                else
                {
                    ballHandIsMissing = false;
                    time = 0;
                }
            }
            else if (CanGrab.Instance.Hand.CompareTag("LeftHand")) // Using the left hand
            {
                if (!HandsReference.Instance.LeftHand.IsTracked) // We check if the left hand disappears
                {
                    ballHandIsMissing = true;
                }
                else
                {
                    ballHandIsMissing = false;
                    time = 0;
                }
            }
        }

        // Check if the hand holding the ball has been missing for more than time threshold
        if (ballHandIsMissing && !throwInProgress && !isUsingController)
        {
            if (time < timeHandNotTracked) // For x seconds
            {
                time += Time.deltaTime;
            }
            else
            {
                tryThrowPlayer = true; // And then try to throw the ball
                throwInProgress = true;
            }
        }
    }

    void FixedUpdate()
    {
        // Player throw with hand or controller when they are in sight of headset.
        if (physicsGrabbable.HasPendingForce && !CanGrab.Instance.transitionHand && !ballHandIsMissing && !tryThrowPlayer)
        {
            DisableVelocityCalculator();
            physicsGrabbable.LinearVelocity = Vector3.zero;
            physicsGrabbable.AngularVelocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().isKinematic = false;
            linearVelocity = CalculateThrowVelocity(ball.transform.position, NPCManager.Instance.npcs[GameManager.Instance.CatcherStage].ballTargets, angle); // MODIFY: angle depending on hand velocity
            SimulateBallTrajectory.SimulateAssistedTrajectory(ball, linearVelocity, line, physicsIterationPositions);
            CanGrab.Instance.Hand.GetComponentInChildren<IInteractor>().Unselect();
            ball.GetComponent<Rigidbody>().isKinematic = false;
            ball.GetComponent<Rigidbody>().AddForce(linearVelocity, ForceMode.VelocityChange);
            tryThrowPlayer = false;
            GameManager.Instance.WhoThrows();
            EnableVelocityCalculator();
            if (!NPCManager.Instance.danceInProgress) NPCManager.Instance.DanceCircel();
            if (!GameManager.Instance.roundInProgress) GameManager.Instance.roundInProgress = true;
            CatchAssistOn(true);
            CanGrab.Instance.Hand = null;
        }

        // Old normal throw for when the ball is released within view
        //if (physicsGrabbable.HasPendingForce && !CanGrab.Instance.transitionHand && !ballHandIsMissing && !tryThrowPlayer)
        //{
        //    ball.GetComponent<Rigidbody>().isKinematic = false;
        //    SimulateBallTrajectory.SimulatePlayerTrajectory(ball, physicsGrabbable, line, physicsIterationPositions);
        //    GameManager.Instance.WhoThrows();
        //    if (!NPCManager.Instance.danceInProgress) NPCManager.Instance.DanceCircel();
        //    if (!GameManager.Instance.roundInProgress) GameManager.Instance.roundInProgress = true;
        //    CatchAssistOn(true);
        //    CanGrab.Instance.Hand = null;
        //}

        // Player is assisted to throw with hand or controller when they are out of sight of headset.
        else if (tryThrowPlayer) // MODIFY: condition to be based on hand velocity magnitude(?)
        {
            DisableVelocityCalculator();
            linearVelocity = CalculateThrowVelocity(ball.transform.position, NPCManager.Instance.npcs[GameManager.Instance.CatcherStage].ballTargets, angle); // MODIFY: angle depending on hand velocity
            ball.GetComponent<Rigidbody>().isKinematic = false;
            SimulateBallTrajectory.SimulateAssistedTrajectory(ball, linearVelocity, line, physicsIterationPositions);
            CanGrab.Instance.Hand.GetComponentInChildren<IInteractor>().Unselect();
            ball.GetComponent<Rigidbody>().isKinematic = false;
            ball.GetComponent<Rigidbody>().AddForce(linearVelocity, ForceMode.VelocityChange);
            tryThrowPlayer = false;
            GameManager.Instance.WhoThrows();
            EnableVelocityCalculator();
            if (!NPCManager.Instance.danceInProgress) NPCManager.Instance.DanceCircel();
            if (!GameManager.Instance.roundInProgress) GameManager.Instance.roundInProgress = true;
            CatchAssistOn(true);
            CanGrab.Instance.Hand = null;
        }
        else if (tryTestThrowPlayer) // For testing purposes
        {
            linearVelocity = CalculateThrowVelocity(ball.transform.position, NPCManager.Instance.npcs[GameManager.Instance.CatcherStage].ballTargets, 62f); // MODIFY: angle depending on hand velocity
            ball.GetComponent<Rigidbody>().isKinematic = false;
            SimulateBallTrajectory.SimulateAssistedTrajectory(ball, linearVelocity, line, physicsIterationPositions);
            ball.GetComponent<Rigidbody>().isKinematic = false;
            //GameManager.Instance.GameStageStep();
            ball.GetComponent<Rigidbody>().AddForce(linearVelocity, ForceMode.VelocityChange);
            StartCoroutine(AudioManager.Instance.playHiKaKusi(1f));
            tryTestThrowPlayer = false;
        }

        // Reset ball position for testing purposes.
        if (tryReset)
        {
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.transform.SetParent(null);
            ReturnBallToStartPosition();
            ball.GetComponent<Rigidbody>().isKinematic = false;
            StartCoroutine(AnimationFunctions.FromGrabTransition(NPCManager.Instance.npcs[GameManager.Instance.ThrowStage].rightIKRig, .5f));
            StartCoroutine(AnimationFunctions.FromGrabTransition(NPCManager.Instance.npcs[GameManager.Instance.ThrowStage].leftIKRig, .5f));
            tryReset = false;
        }
    }


    Vector3 CalculateThrowVelocity(Vector3 source, Transform[] ballTargets, float angle)
    {
        Vector3 newTarget = RandomizeThrowTarget(ballTargets);
        Vector3 direction = newTarget - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }

    private Vector3 RandomizeThrowTarget(Transform[] ballTargets)
    {
        // Random target selection, including target to eliminate npc
        int randIndex;
        if (GameManager.Instance.playerIsSpectating) randIndex = 3;
        else randIndex = Random.Range(1, 6);
        Debug.Log("Random target is: " + randIndex);

        return ballTargets[randIndex].position;
    }

    public void CatchAssistOn(bool turnOn)
    {
        StartCoroutine(ToggleCatchAssist(turnOn));
    }

    public IEnumerator ToggleCatchAssist(bool turnOn)
    {
        if (turnOn)
        {
            yield return new WaitForSecondsRealtime(2f);
            catchAssistL.SetActive(true);
            catchAssistR.SetActive(true);
        }
        else
        {
            ball.transform.SetParent(null);
            yield return new WaitForEndOfFrame();
            catchAssistL.SetActive(false);
            catchAssistR.SetActive(false);
        }
    }


    private void DisableVelocityCalculator()
    {
        CanGrab.Instance.Hand.GetComponentInChildren<StandardVelocityCalculator>().InstantVelocityInfluence = 0;
        CanGrab.Instance.Hand.GetComponentInChildren<StandardVelocityCalculator>().TangentialVelocityInfluence = 0;
        CanGrab.Instance.Hand.GetComponentInChildren<StandardVelocityCalculator>().TrendVelocityInfluence = 0;
        CanGrab.Instance.Hand.GetComponentInChildren<StandardVelocityCalculator>().ExternalVelocityInfluence = 0;
    }

    private void EnableVelocityCalculator()
    {
        CanGrab.Instance.Hand.GetComponentInChildren<StandardVelocityCalculator>().InstantVelocityInfluence = 1;
        CanGrab.Instance.Hand.GetComponentInChildren<StandardVelocityCalculator>().TangentialVelocityInfluence = 1;
        CanGrab.Instance.Hand.GetComponentInChildren<StandardVelocityCalculator>().TrendVelocityInfluence = 1;
        CanGrab.Instance.Hand.GetComponentInChildren<StandardVelocityCalculator>().ExternalVelocityInfluence = 1;
    }

    void ReturnBallToStartPosition()
    {
        ball.transform.position = startPos;
    }
}
