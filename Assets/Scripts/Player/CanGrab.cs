using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanGrab : MonoBehaviour
{
    public static CanGrab Instance;
    public bool isGrabbing = false, transitionHand = false;
    public GameObject Hand;
    public GameObject previousHand;

    private void Awake()
    {
        Instance = this;
    }

    public void IsGrabbing()
    {
        isGrabbing = true;
    }

    public void IsNotGrabbing()
    {
        if (!transitionHand && isGrabbing)
        {
            //Hand = null;
            //transitionHand = false;
            isGrabbing = false;
            Debug.Log("The hand is holding nothing right now.");
        }
    }

    public void ReturnHand(GameObject newHand)
    {
        previousHand = Hand;
        if (newHand != previousHand && previousHand != null)
        {
            Hand = newHand;
            transitionHand = true;
            isGrabbing = true;
            Debug.Log("The ball was moved from " + previousHand.name + " to " + newHand.name);
        }
        else if (newHand != null)
        {
            Hand = newHand;
            isGrabbing = true;
        }
        Debug.Log(newHand.name + " is holding the ball right now.");
    }

    public void StartResetHand()
    {
        StartCoroutine(ResetHand());
    }
    public IEnumerator ResetHand()
    {
        yield return new WaitForSeconds(.1f);

        Debug.Log("The ball was transitioned, so we should not reset.");
        transitionHand = false;

    }
}
