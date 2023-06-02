using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureManager : RecognizeHandPose
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        RecordHandPose(3);
    }
}
