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
    private bool prevFrameButtonPressed;
    public string filenamePrefix = "Logs/log";

    public int logCount = 1;

    public TextMesh statusText;
    const string baseStatusText = "Press the \"A\" button to start/stop recording data.\n";

    // Start is called before the first frame update
    void Start()
    {
        loggedVec3Characteristics = new Dictionary<String, UnityEngine.XR.InputFeatureUsage<UnityEngine.Vector3>> {
            {"vel", UnityEngine.XR.CommonUsages.deviceVelocity },
            {"angularVel", UnityEngine.XR.CommonUsages.deviceAngularVelocity },
        };

        isLogging = false;
        prevFrameButtonPressed = false;
    }

    string GetLogHeader(List<UnityEngine.XR.InputDevice> devices) 
    {
        string logHeader = "timestamp,";
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

    void StartLogging()
    {
        // Refresh currently connected devices
        trackedDevices = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.TrackedDevice;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, trackedDevices);

        logWriter = new StreamWriter($"{filenamePrefix}-{logCount}.csv");
        logWriter.WriteLine(GetLogHeader(trackedDevices));

        statusText.text = baseStatusText + "STATUS: Recording";
    }

    void StopLogging()
    {
        logWriter.Close();
        statusText.text = baseStatusText + "STATUS: Not recording";
        logCount += 1;
    }

    void LogMeasurements()
    {
        DateTime recordedTime = DateTime.UtcNow;
        string logValue = recordedTime.ToString("MM/dd/yyyy hh:mm:ss.fff") + ",";
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

    // Update is called once per frame
    void Update()
    {
        // Update controller states
        OVRInput.Update();

        // Check if the "A" button on the right controller is held down
        bool buttonPressed = OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch);
        
        // Check if the button has been toggled on the current frame
        // (i.e. wasn't previously held down)
        if (buttonPressed && !prevFrameButtonPressed)
        {
            isLogging = !isLogging;
            if (isLogging)
            {
                StartLogging();
            } else 
            {
                StopLogging();
            }
            
        }

        prevFrameButtonPressed = buttonPressed;

        if (isLogging)
        {
            LogMeasurements();
        }
    }

}
