using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCaller : MonoBehaviour
{
    // This is called from the NPC's trick animation
    private void GameStageStepEvent()
    {
        GameManager.Instance.WhoThrows();
    }
}
