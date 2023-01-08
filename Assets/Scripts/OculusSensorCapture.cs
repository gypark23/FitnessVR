using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class OculusSensorCapture : MonoBehaviour
{
    // List of logged sensor characteristics that are stored as 3D vectors
    private Dictionary<String, UnityEngine.XR.InputFeatureUsage<UnityEngine.Vector3>> loggedVec3Characteristics;
    private List<UnityEngine.XR.InputDevice> trackedDevices;
    private List<string> dimensions = new List<string>{"x", "y", "z"};

    private StreamWriter logWriter;
    private bool isLogging = false;
    
    private (string, string)[] activities = {("STD", "Standing"), ("SIT", "Sitting"),
        ("JOG", "Jogging"), ("STR", "Arms stretching"), ("OHD", "Arms overhead"), 
        ("TWS", "Twisting")};

    private int curActivityIdx = 0;
    private int curGroupMemberNum = 1;
    private int curTrial = 0;

    private DateTime logStartTime;

    public TextMesh hudStatusText, wallStatusText, timerText;
    const string baseStatusText = "Press \"A\" to start/stop recording data.\n";

    // Start is called before the first frame update
    void Start()
    {
        loggedVec3Characteristics = new Dictionary<String, UnityEngine.XR.InputFeatureUsage<UnityEngine.Vector3>> {
            {"vel", UnityEngine.XR.CommonUsages.deviceVelocity },
            {"angularVel", UnityEngine.XR.CommonUsages.deviceAngularVelocity },
            {"pos", UnityEngine.XR.CommonUsages.devicePosition },
        };

        RefreshTrackedDevices();
    }
    
    /// <summary>
    /// Reload the list of devices that have data logged based on connected devices.
    /// </summary>
    void RefreshTrackedDevices()
    {
        trackedDevices = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.TrackedDevice;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, trackedDevices);
    }
    
    /// <summary>
    /// Get the filename prefix of a logged data file, based on the selected activity
    /// and group member.
    /// </summary>
    string GetDataFilePrefix()
    {
        return $"{activities[curActivityIdx].Item1}_P{curGroupMemberNum + 1}";
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

    /// <summary>
    /// Fetch the header of the CSV sensor log, based on the current tracked devices,
    /// available attributes, and dimensions of each attribute.
    /// </summary>
    string GetLogHeader(List<UnityEngine.XR.InputDevice> devices) 
    {
        string logHeader = "time,";
        foreach (var device in devices)
        {
            foreach (var key in loggedVec3Characteristics.Keys)
            {
                foreach (var axis in dimensions)
                {
                    logHeader += $"{MapDeviceName(device.name)}_{key}.{axis},";
                }
            }
            foreach (var axis in dimensions)
            {
                logHeader += $"{MapDeviceName(device.name)}_rot.{axis},";
            }
        }

        return logHeader;
    }

    /// <summary>Write the current sensor values to the open CSV file.</summary>
    void LogAttributes()
    {
        // Display the current time on the timer on the wall, then log it
        // in the CSV file
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

            // Rotation data is recorded as a quaternion, then converted into XYZ angles
            Quaternion rotationQuaternion;
            device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out rotationQuaternion);
            recordedValue = rotationQuaternion.eulerAngles;
            logValue += $"{recordedValue.x},{recordedValue.y},{recordedValue.z},";
        }

        logWriter.WriteLine(logValue);
    }

    /// <returns>The number of saved data files for the current user and activity.</returns>
    int GetNumExistingDataFiles()
    {
        var matchingFiles = Directory.GetFiles(Application.persistentDataPath, $"{GetDataFilePrefix()}*");
        return matchingFiles.Length;
    }

    /// <returns>The CSV header string corresponding to a given Unity device name.</returns>
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

    /// <summary>
    /// Send a haptic vibration of the given amplitude and duration to all connected controllers.
    /// </summary>
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

        // Change selected activity
        if (frontTriggerPressed)
        {
            curActivityIdx = (curActivityIdx + 1) % activities.Length;
        }

        // Change selected group member
        if (sideTriggerPressed)
        {
            curGroupMemberNum = (curGroupMemberNum + 1) % 3;
        }

        // Send a small vibration for feedback and refresh the number of collected
        // data files on the UI
        if (frontTriggerPressed || sideTriggerPressed)
        {
            curTrial = GetNumExistingDataFiles();
            SendImpulse(0.1f, 0.05f);
        }

        // Change the wall UI text
        wallStatusText.text = $"Activity: {activities[curActivityIdx].Item2}\n" + 
            $"Group member: {curGroupMemberNum + 1}\nLast trial: {curTrial}"; 
        
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
