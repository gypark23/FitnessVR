using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class OculusSensorCapture : MonoBehaviour
{
    private Dictionary<String, UnityEngine.XR.InputFeatureUsage<UnityEngine.Vector3>> loggedVec3Characteristics;
    private List<UnityEngine.XR.InputDevice> trackedDevices;

    private StreamWriter logWriter;
    private bool isLogging;

    private int curTrial;
    
    private (string, string)[] activities = {("STD", "Standing"), ("SIT", "Sitting"),
        ("JOG", "Jogging"), ("STR", "Arms stretching"), ("OHD", "Arms overhead"), 
        ("SPN", "Spinning")};
    private int curActivityIdx = 0;
    private int curGroupMemberNum = 1;

    private DateTime logStartTime;

    public TextMesh hudStatusText, wallStatusText, timerText;
    const string baseStatusText = "Press the \"A\" button to start/stop recording data.\n";

    // Start is called before the first frame update
    void Start()
    {
        loggedVec3Characteristics = new Dictionary<String, UnityEngine.XR.InputFeatureUsage<UnityEngine.Vector3>> {
            {"vel", UnityEngine.XR.CommonUsages.deviceVelocity },
            {"angularVel", UnityEngine.XR.CommonUsages.deviceAngularVelocity },
            {"pos", UnityEngine.XR.CommonUsages.devicePosition },
        };

        isLogging = false;
        curTrial = 0;
        RefreshTrackedDevices();
    }

    string GetLogHeader(List<UnityEngine.XR.InputDevice> devices) 
    {
        string logHeader = "time,";
        foreach (var device in devices)
        {
            foreach (var key in loggedVec3Characteristics.Keys)
            {
                foreach (var axis in new List<string>{"x", "y", "z"})
                {
                    logHeader += $"{MapDeviceName(device.name)}_{key}.{axis},";
                }
            }
            foreach (var axis in new List<string>{"x", "y", "z"})
            {
                logHeader += $"{MapDeviceName(device.name)}_rot.{axis},";
            }
        }

        return logHeader;
    }

    string GetDataFilePrefix()
    {
        return $"{activities[curActivityIdx].Item1}_P{curGroupMemberNum + 1}";
    }

    void RefreshTrackedDevices()
    {
        trackedDevices = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.TrackedDevice;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, trackedDevices);
    }

    void StartLogging()
    {
        curTrial += 1;

        RefreshTrackedDevices();

        string filename = $"{GetDataFilePrefix()}_{curTrial:D2}.csv";
        string path = Path.Combine(Application.persistentDataPath, filename);
        logWriter = new StreamWriter(path);
        logWriter.WriteLine(GetLogHeader(trackedDevices));
        logStartTime = DateTime.UtcNow;

        hudStatusText.text = baseStatusText + "STATUS: Recording";
    }

    void StopLogging()
    {
        logWriter.Close();
        hudStatusText.text = baseStatusText + "STATUS: Not recording";
        
    }

    void LogMeasurements()
    {
        TimeSpan timeDifference = DateTime.UtcNow - logStartTime;
        timerText.text = $"{timeDifference.TotalSeconds:F2} s";

        string logValue = $"{timeDifference.TotalMilliseconds},";
        foreach (var device in trackedDevices)
        {
            Vector3 recordedValue;
            foreach (var characteristic in loggedVec3Characteristics) 
            {
                device.TryGetFeatureValue(characteristic.Value, out recordedValue);
                logValue += $"{recordedValue.x},{recordedValue.y},{recordedValue.z},";
            }

            Quaternion rotationQuaternion;
            device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out rotationQuaternion);
            recordedValue = rotationQuaternion.eulerAngles;
            logValue += $"{recordedValue.x},{recordedValue.y},{recordedValue.z},";
        }
        logWriter.WriteLine(logValue);
    }

    int GetNumExistingDataFiles()
    {
        var matchingFiles = Directory.GetFiles(Application.persistentDataPath, $"{GetDataFilePrefix()}*");
        return matchingFiles.Length;
    }

    string MapDeviceName(string deviceName)
    {
        if (deviceName.Contains("Left"))
        {
            return "controller_left";
        }

        if (deviceName.Contains("Right"))
        {
            return "controller_right";
        }

        if (deviceName.Contains("Quest"))
        {
            return "headset";
        }

        return "unknown";
    }

    void OnDestroy()
    {
        if (logWriter != null && logWriter.BaseStream != null)
        {
            logWriter.Close();
        }
    }

    void SendImpulse(float amplitude, float duration)
    {
        foreach (var device in trackedDevices)
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
        // Check which buttons on the right controller are pressed on the current frame
        bool aButtonPressed = OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch);
        bool frontTriggerPressed = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        bool sideTriggerPressed = OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);

        if (frontTriggerPressed)
        {
            curActivityIdx = (curActivityIdx + 1) % activities.Length;
        }

        if (sideTriggerPressed)
        {
            curGroupMemberNum = (curGroupMemberNum + 1) % 3;
        }

        if (frontTriggerPressed || sideTriggerPressed)
        {
            curTrial = GetNumExistingDataFiles();
            SendImpulse(0.1f, 0.05f);
        }

        wallStatusText.text = $"Activity: {activities[curActivityIdx].Item2}\n" + 
            $"Group member: {curGroupMemberNum + 1}\nLast trial: {curTrial}"; 
        
        // Check if the button has been toggled on the current frame
        // (i.e. wasn't previously held down)
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

        if (isLogging)
        {
            LogMeasurements();
        }
    }

}
