using System.Collections.Generic;
using UnityEngine;

public class ActivityDetector : MonoBehaviour
{
    // Feel free to add additional class variables here
    OculusSensorReader sensorReader;

    // Start is called before the first frame update
    void Start()
    {
        sensorReader = new OculusSensorReader();
    }

    /// <summary>
    /// Return the activity code of the current activity. Feel free to add
    /// additional parameters to this function.
    /// </summary>
    string GetCurrentActivity(Dictionary<string, Vector3> attributes)
    {
        // Here's an example of fetching an attribute from the dictionary
        var headsetVelX = attributes["headset_vel"].x;

        // TODO: Implement your algorithm here to determine the current activity based
        // on recent sensor traces.

        return "Unknown";
    }

    // Update is called once per frame
    void Update()
    {
        sensorReader.RefreshTrackedDevices();

        // Fetch attributes as a dictionary, with <device>_<measure> as a key
        // and Vector3 objects as values
        var attributes = sensorReader.GetSensorReadings();

        var currentActivity = GetCurrentActivity(attributes);

        // TODO: Update the Activity Sign text based on the detected activity
    }
}
