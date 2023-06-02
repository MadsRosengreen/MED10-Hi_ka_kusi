using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Throw;

public class NPCThrowBall : MonoBehaviour
{
    public static NPCThrowBall Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void NPCThrow(GameObject ball, Transform[] ballTargets, float throwAngle, LineRenderer line, Vector3[] physicsIterationPositions)
    {
        Vector3 linearVelocity = CalculateThrowVelocity(ball.transform.position, ballTargets, throwAngle);
        ball.GetComponent<Rigidbody>().isKinematic = false;
        ball.transform.SetParent(null);
        SimulateBallTrajectory.SimulateAssistedTrajectory(ball, linearVelocity, line, physicsIterationPositions);
        ball.GetComponent<PhysicsGrabbable>().enabled = false;
        ball.GetComponent<Rigidbody>().AddForce(linearVelocity, ForceMode.VelocityChange);
        ball.GetComponent<PhysicsGrabbable>().enabled = true;
        StartCoroutine(ball.GetComponent<IsBallGrabbed>().SetGrabbedFalse());
    }

    Vector3 CalculateThrowVelocity(Vector3 source, Transform[] ballTargets, float angle)
    {
        Vector3 newTarget = RandomizeThrowTarget(ballTargets);

        Vector3 direction = newTarget - source;

        float h = direction.y;
        //direction.y = 0;
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
        // Random target selection - last index (6) is out of reach, will eliminate the catcher
        Random.InitState(System.DateTime.Now.Millisecond);
        int randIndex;
        if (GameManager.Instance.playerIsSpectating) randIndex = 3;
        else randIndex = Random.Range(0, 6);
        Debug.Log("Random target is: " + randIndex);


        return ballTargets[randIndex].position;
    }

}
