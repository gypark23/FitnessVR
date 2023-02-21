using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Lab3Capture : MonoBehaviour
{
    OculusSensorReader sensorReader;

    private StreamWriter logWriter;
    private bool isLogging = false;
    private float secondsCount;

    private (string, string)[] activities = {("STD", "Standing"), ("SIT", "Sitting"),
        ("JOG", "Jogging"), ("STR", "Arms stretching"), ("OHD", "Arms overhead"), 
        ("TWS", "Twisting")};

    private int curActivityIdx = 0;
    private int curGroupMemberNum = 1;
    private int curTrial = 0;

    private DateTime logStartTime;

    public TextMesh hudStatusText, wallStatusText, timerText;
    const string baseStatusText = "Press \"A\" to start/stop recording data, and please walk forward until the timer stops.\n";

    // Start is called before the first frame update
    void Start()
    {
        secondsCount = 10.0f;
        sensorReader = new OculusSensorReader();
    }
    
    /// <summary>
    /// Get the filename prefix of a logged data file, based on the selected activity
    /// and group member.
    /// </summary>
    string GetDataFilePrefix()
    {
        return $"Lab3_P{curGroupMemberNum + 1}";
    }

    void StartLogging()
    {
        curTrial += 1;

        sensorReader.RefreshTrackedDevices();

        string filename = $"{GetDataFilePrefix()}_{curTrial:D2}.csv";
        string path = Path.Combine(Application.persistentDataPath, filename);
        
        logWriter = new StreamWriter(path);
        logWriter.WriteLine(GetLogHeader());
        
        logStartTime = DateTime.UtcNow;
        hudStatusText.text = baseStatusText + "STATUS: Recording";
    }

    void StopLogging()
    {
        logWriter.Close();
        hudStatusText.text = baseStatusText + "STATUS: Not recording";
    }

    /// <summary>
    /// Fetch the header of the CSV sensor log, based on the current tracked devices,
    /// available attributes, and dimensions of each attribute.
    /// </summary>
    string GetLogHeader() 
    {
        string logHeader = "time,";

        var attributes = sensorReader.GetAvailableAttributes();
        logHeader += String.Join(",", attributes);

        return logHeader;
    }

    /// <summary>Write the current sensor values to the open CSV file.</summary>
    void LogAttributes()
    {
        // Display the current time on the timer on the wall, then log it
        // in the CSV file
        TimeSpan timeDifference = DateTime.UtcNow - logStartTime;
        timerText.text = $"Remaining: {secondsCount.ToString()} s";

        string logValue = $"{timeDifference.TotalMilliseconds},";
        
        var attributes = sensorReader.GetSensorReadings();
        foreach (var attribute in attributes)
        {
            logValue += $"{attribute.Value.x},{attribute.Value.y},{attribute.Value.z},";
        }

        logWriter.WriteLine(logValue);
    }

    /// <returns>The number of saved data files for the current user and activity.</returns>
    int GetNumExistingDataFiles()
    {
        var matchingFiles = Directory.GetFiles(Application.persistentDataPath, $"{GetDataFilePrefix()}*");
        return matchingFiles.Length;
    }

    /// <summary>
    /// Send a haptic vibration of the given amplitude and duration to all connected controllers.
    /// </summary>
    void SendImpulse(float amplitude, float duration)
    {
        foreach (var device in sensorReader.GetTrackedDevices())
        {
            if (device.TryGetHapticCapabilities(out var capabilities) &&
                capabilities.supportsImpulse)
            {
                device.SendHapticImpulse(0u, amplitude, duration);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        wallStatusText.text = $"Group member: {curGroupMemberNum + 1}\nLast trial: {curTrial}"; 

        if(isLogging) {
            secondsCount -= Time.deltaTime;
        }

        // Check which buttons on the right controller are pressed on the current frame
        bool aButtonPressed = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
        bool frontTriggerPressed = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        bool sideTriggerPressed = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);

        // Change selected activity
        if (frontTriggerPressed)
        {
            curActivityIdx = (curActivityIdx + 1) % activities.Length;
        }

        // Change selected group member
        if (sideTriggerPressed)
        {
            curGroupMemberNum += 1;
        }

        // Send a small vibration for feedback and refresh the number of collected
        // data files on the UI
        if (frontTriggerPressed || sideTriggerPressed)
        {
            curTrial = GetNumExistingDataFiles();
            SendImpulse(0.1f, 0.05f);
        }


        // Toggle logging on/off
        if (aButtonPressed)
        {
            isLogging = !isLogging;
            if (isLogging)
            {
                StartLogging();
            } else 
            {
                StopLogging();
            }

            SendImpulse(0.2f, 0.1f);
        }

        if (secondsCount <= 0)
        {
            StopLogging();
            SendImpulse(0.2f, 0.1f);
            isLogging = false;
            secondsCount = 10.0f;
        }

        // Log attributes once for each frame if we are recording
        if (isLogging)
        {
            LogAttributes();
        }
    }
    
    /// <summary>
    /// Automatically close a log file if the app is closed while recording is in progress.
    /// Run when the scene is destroyed.
    /// </summary>
    void OnDestroy()
    {
        if (logWriter != null && logWriter.BaseStream != null)
        {
            logWriter.Close();
        }
    }

}
