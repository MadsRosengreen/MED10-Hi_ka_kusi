using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Oculus.Interaction;


public class SimulateBallTrajectory : MonoBehaviour
{
    public static SimulateBallTrajectory Instance;
    public static bool SimComplete;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreatePhysicsScene();
    }

    private static Scene _simulationScene;
    private static PhysicsScene _physicsScene;
    [SerializeField] private Transform groundPlane;

    void CreatePhysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        // Instantiate the ground in physics scene to allow for collision with ground in simulation
        var ghostObj = Instantiate(groundPlane.gameObject, groundPlane.position, groundPlane.rotation);
        ghostObj.GetComponent<Renderer>().enabled = false;
        SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);

    }


    public static void SimulatePlayerTrajectory(GameObject ball, PhysicsGrabbable physicsGrabbable, LineRenderer line, Vector3[] positionPhysicsIterations)
    {
        Vector3 startPos = ball.transform.position;
        Quaternion startRot = ball.transform.rotation;
        var ghostBall = Instantiate(ball, startPos, startRot);
        ghostBall.tag = "Untagged";
        SceneManager.MoveGameObjectToScene(ghostBall, _simulationScene);

        ApplySimulationVelocities(ghostBall.GetComponent<Rigidbody>(), physicsGrabbable.LinearVelocity, physicsGrabbable.AngularVelocity);

        line.positionCount = positionPhysicsIterations.Length;

        for (int i = 0; i < positionPhysicsIterations.Length; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            positionPhysicsIterations[i] = ghostBall.transform.position;
        }
        line.SetPositions(positionPhysicsIterations);

        Destroy(ghostBall.gameObject);
        SimComplete = true;
    }

    public static void SimulateAssistedTrajectory(GameObject ball, Vector3 linearVelocity, LineRenderer line, Vector3[] positionPhysicsIterations)
    {
        Vector3 startPos = ball.transform.position;
        Quaternion startRot = ball.transform.rotation;
        GameObject ghostBall = Instantiate(ball, startPos, startRot);
        ghostBall.tag = "Untagged";
        SceneManager.MoveGameObjectToScene(ghostBall, _simulationScene);

        ghostBall.GetComponent<Rigidbody>().AddForce(linearVelocity, ForceMode.VelocityChange);

        line.positionCount = positionPhysicsIterations.Length;

        for (int i = 0; i < positionPhysicsIterations.Length; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            positionPhysicsIterations[i] = ghostBall.transform.position;
        }
        line.SetPositions(positionPhysicsIterations);

        Destroy(ghostBall.gameObject);
        SimComplete = true;
    }

    private static void ApplySimulationVelocities(Rigidbody rigidbody, Vector3 linearVelocity, Vector3 angularVelocity)
    {
        rigidbody.AddForce(linearVelocity, ForceMode.VelocityChange);
        rigidbody.AddTorque(angularVelocity, ForceMode.VelocityChange);
    }

    // Below is a concept for simulating physics over multiple frames.
    // Current edition has some issues with intervals, that causes every 20 index to return 20 0,0,0's.

    //[SerializeField] private LineRenderer _line;
    //[SerializeField] private Vector3[] _positionPhysicsIterations = new Vector3[200];
    //[SerializeField] private int _positionPhysicsIterationStep = 20;


    //public IEnumerator SimulateTrajectory(GameObject ball, Vector3 linearVelocity, Vector3 angularVelocity, Vector3 startPos, Quaternion startRot)
    //{
    //    var ghostBall = Instantiate(ball, startPos, startRot);
    //    SceneManager.MoveGameObjectToScene(ghostBall, _simulationScene);

    //    ApplyVelocities(ghostBall.GetComponent<Rigidbody>(), linearVelocity, angularVelocity);

    //    for (int i = 0; i < _positionPhysicsIterations.Length; i += _positionPhysicsIterationStep)
    //    {
    //        _line.positionCount += _positionPhysicsIterationStep;

    //        for (int s = i; s < i + _positionPhysicsIterationStep; s++)
    //        {
    //            _physicsScene.Simulate(Time.fixedDeltaTime);
    //            _positionPhysicsIterations[s+i] = ghostBall.transform.position;
    //        }
    //        _line.SetPositions(_positionPhysicsIterations);


    //        yield return new WaitForSeconds(Time.fixedDeltaTime);
    //    }
    //    Destroy(ghostBall.gameObject);

    //}
}