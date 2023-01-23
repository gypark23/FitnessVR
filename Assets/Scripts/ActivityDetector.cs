using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityDetector : MonoBehaviour
{
    OculusSensorReader sensorReader;

    // Start is called before the first frame update
    void Start()
    {
        sensorReader = new OculusSensorReader();
    }

    // Update is called once per frame
    void Update()
    {
        sensorReader.RefreshTrackedDevices();

        // Fetch attributes as a dictionary, with <device>_<measure> as a key
        // and Vector3 objects as values
        var attributes = sensorReader.GetSensorReadings();

        // Here's an example of fetching an attribute from the dictionary
        var headsetVelX = attributes["headset_vel"].x;

        // Implement your algorithm here to determine the current activity based
        // on recent sensor traces

        // Update the Activity Sign text based on the detected activity
    }
}
