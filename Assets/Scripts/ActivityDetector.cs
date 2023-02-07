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
    string filePath = "Data/Lab2/Labels/stat.txt";
    List<double> STD = new List<double>();
    List<double> SIT = new List<double>();
    List<double> JOG = new List<double>();
    List<double> STR = new List<double>();
    List<double> OHD = new List<double>();
    List<double> TWS = new List<double>();



    // Start is called before the first frame update
    void Start()
    {
        sensorReader = new OculusSensorReader();
        text = GetComponent<TextMesh>();
        using (StreamReader sr = new StreamReader(filePath))
        {
            string line;
            int i = 0;
            while ((line = sr.ReadLine()) != null)
            {
                Console.WriteLine(line);
                int startIndex = line.IndexOf('[') + 1;
                int endIndex = line.IndexOf(']');
                string numbersString = line.Substring(startIndex, endIndex - startIndex);
                string[] numberStrings = numbersString.Split(',');

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
                i++;
            }
        }

        Console.WriteLine(string.Join(",", STD));
        Console.WriteLine(string.Join(",", SIT));
        Console.WriteLine(string.Join(",", JOG));
        Console.WriteLine(string.Join(",", STR));
        Console.WriteLine(string.Join(",", OHD));
        Console.WriteLine(string.Join(",", TWS));


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
        text.text = frame.ToString()++;
    }
}
