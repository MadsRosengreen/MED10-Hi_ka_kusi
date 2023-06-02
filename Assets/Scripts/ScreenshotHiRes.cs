using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ScreenshotHiRes : MonoBehaviour
{
    [SerializeField]
    private string screenName;
    void Update()
    {
        //Press W to take a Screen Capture
        if (Input.GetKeyDown(KeyCode.C))
        {
            ScreenCapture.CaptureScreenshot(Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), screenName + ".png"), 8);
            Debug.Log("Screenshot Captured");
        }
    }
}