using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameDebugConsole : MonoBehaviour
{
    [SerializeField] private Text logText;

    void Awake()
    {
        Application.logMessageReceived += WriteLogMessage;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= WriteLogMessage;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void WriteLogMessage(string msg, string stackTrace, LogType type)
    {
        string logMsg;
        switch (type)
        {
            case LogType.Assert:
            case LogType.Error:
            case LogType.Exception:
                logMsg = "<color=red>";
                break;
            case LogType.Warning:
                logMsg = "<color=yellow>";
                break;
            case LogType.Log:
                logMsg = "<color=white>";
                break;
            default:
                logMsg = "";
                Debug.Assert(false);
                break;
        }
        
        logMsg += $"{msg}\n";
        logMsg += "</color>";

        logText.text += logMsg;
    }
}