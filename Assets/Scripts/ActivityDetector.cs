using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class ActivityDetector : MonoBehaviour
{
    // Feel free to add additional class variables here
    OculusSensorReader sensorReader;
    public TextMesh text;
    private int frame;
    private string test;

    TextAsset standardData;  //standardData.text to access string data

    List<double> STD = new List<double>();
    List<double> SIT = new List<double>();
    List<double> JOG = new List<double>();
    List<double> STR = new List<double>();
    List<double> OHD = new List<double>();
    List<double> TWS = new List<double>();



    // Start is called before the first frame update
    void Start()
    {
        standardData = (TextAsset)Resources.Load("standard");
        string[] lines = standardData.text.Split('\n');

        sensorReader = new OculusSensorReader();
        text = GetComponent<TextMesh>();
        for (int i = 0; i < 6; i++)
        {
            string line = lines[i];
            int startIndex = line.IndexOf('[') + 1;
            int endIndex = line.IndexOf(']');
            string numbersString = line.Substring(startIndex, endIndex - startIndex);
            string[] numberStrings = numbersString.Split(", ");

            List<double> numbers = new List<double>();
            foreach (string numberString in numberStrings)
            {
                double number = double.Parse(numberString);
                if (i == 0)
                {
                    STD.Add(number);
                }
                else if (i == 1)
                {
                    SIT.Add(number);
                }
                else if (i == 2)
                {
                    JOG.Add(number);
                }
                else if (i == 3)
                {
                    STR.Add(number);
                }
                else if (i == 4)
                {
                    OHD.Add(number);
                }
                else if (i == 5)
                {
                    TWS.Add(number);
                }
            }
        }

        frame = 0;
    }

    /// <summary>
    /// Return the activity code of the current activity. Feel free to add
    /// additional parameters to this function.
    /// </summary>
    string GetCurrentActivity(Dictionary<string, Vector3> attributes)
    {
        

        //  Make sure you include past 3 seconds data - Kyu 

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

        // Change this text to Activity Type you determine (i.e "JOG")
        // Currently set to frames to show you it updates every frame - Kyu
        
        // text.text = GetCurrentActivity();
        text.text = "INSERT ACTIVITY HERE" + frame++.ToString();
    }
}
