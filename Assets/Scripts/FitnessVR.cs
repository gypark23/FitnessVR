using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

public class FitnessVR : MonoBehaviour
{
    OculusSensorReader sensorReader;
    // text to display the detected activity
    public TextMesh text;
    // float to detect activity every 2.9 seconds
    private float secondsCount;
    // string to store current activity
    private string currentActivity;

    private bool moving = false;
    private double reps = 0;

    // variables to store standard data to compare to real time data
    TextAsset standardData;  //standardData.text to access string data
    List<float> STD = new List<float>();
    List<float> SIT = new List<float>();
    List<float> JOG = new List<float>();
    List<float> STR = new List<float>();
    List<float> OHD = new List<float>();
    List<float> TWS = new List<float>();

    // collects real time VR data for the most recent 2.9 seconds
    List<List<float>> updatedData = new List<List<float>>();

    // list of activities we want to identify
    List<string> Acts = new List<string> {"STD", "SIT", "JOG", "STR", "OHD", "TWS"};

    // Start is called before the first frame update
    void Start()
    {
        // Loading the lib for the standard vector of each activity
        standardData = (TextAsset)Resources.Load("standard");
        string[] lines = standardData.text.Split('\n');

        // Add 36 empty lists to updatedData where we will
        // store a history of real time data
        for (int i = 0; i < 36; i++)
        {
            updatedData.Add(new List<float>());
        }
        secondsCount = 2.9f;
        sensorReader = new OculusSensorReader();
        text = GetComponent<TextMesh>();
        
        // parse standard activity data into a list for each activity
        for (int i = 0; i < 6; i++)
        {
            string line = lines[i];
            int startIndex = line.IndexOf('[') + 1;
            int endIndex = line.IndexOf(']');
            string numbersString = line.Substring(startIndex, endIndex - startIndex);
            string[] numberStrings = numbersString.Split(", ");

            List<float> numbers = new List<float>();
            foreach (string numberString in numberStrings)
            {
                float number = float.Parse(numberString);
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
    }

    // convert Vec3 into a 2-D array of all its components
    void GetData(Dictionary<string, Vector3> attributes)
    {
        // get the 36 attributes
        var h_vel = attributes["headset_vel"];
        var h_ang = attributes["headset_angularVel"];
        var h_pos = attributes["headset_pos"];
        var h_rot = attributes["headset_rot"];

        var c_l_vel = attributes["controller_left_vel"];
        var c_l_ang = attributes["controller_left_angularVel"];
        var c_l_pos = attributes["controller_left_pos"];
        var c_l_rot = attributes["controller_left_rot"];

        var c_r_vel = attributes["controller_right_vel"];
        var c_r_ang = attributes["controller_right_angularVel"];
        var c_r_pos = attributes["controller_right_pos"];
        var c_r_rot = attributes["controller_right_rot"];


        List<float> data = new List<float>
        {
            h_vel.x, h_vel.y, h_vel.z,
            h_ang.x, h_ang.y, h_ang.z,
            h_pos.x, h_pos.y, h_pos.z,
            h_rot.x, h_rot.y, h_rot.z,
            c_l_vel.x, c_l_vel.y, c_l_vel.z,
            c_l_ang.x, c_l_ang.y, c_l_ang.z,
            c_l_pos.x, c_l_pos.y, c_l_pos.z,
            c_l_rot.x, c_l_rot.y, c_l_rot.z,
            c_r_vel.x, c_r_vel.y, c_r_vel.z,
            c_r_ang.x, c_r_ang.y, c_r_ang.z,
            c_r_pos.x, c_r_pos.y, c_r_pos.z,
            c_r_rot.x, c_r_rot.y, c_r_rot.z,
        };


        for (int i = 0; i < 36; i++)
        {
            updatedData[i].Add(data[i]);
        }
    }

    // calculate the mean and var of the raw data
    List<float> ProcessData()
    {
        List<float> means = new List<float>();
        List<float> vars = new List<float>();
        List<float> vector = new List<float>();

        foreach (List<float> list in updatedData)
        {
            float mean = list.Average();
            float variance = list.Sum(x => (x - mean) * (x - mean)) / list.Count;
            means.Add(mean);
            vars.Add(variance);
        }


        vector.AddRange(means);
        vector.AddRange(vars);

        return vector;
    }

    float CalcAvgVel()
    {
        List<float> vector = ProcessData();
        float leftAvg = (Math.Abs(vector[12]) + Math.Abs(vector[13]) + Math.Abs(vector[14])) / 3;
        return leftAvg;
    }

    // calculate the distance between 2 arrays
    float CalcDist(List<float> StandardVector, List<float> vector)
    {
        float dist = 0.0f;

        for (int i = 0; i < vector.Count; i++)
        {
            if (i==6||i==7||i==8||i==42||i==43||i==51||i==57||i==69||i==63||i==55||i == 66)
            {
                float diff = (StandardVector[i] - vector[i])/StandardVector[i];
                dist = dist + diff*diff;
            }
        }
        return dist;     
    }

    // classify by finding the min dist activity
    string Classify(List<float> vector) 
    {
        float dist_STD = CalcDist(STD, vector);
        float dist_SIT = CalcDist(SIT, vector);
        float dist_JOG = CalcDist(JOG, vector);
        float dist_STR = CalcDist(STR, vector);
        float dist_OHD = CalcDist(OHD, vector);
        float dist_TWS = CalcDist(TWS, vector);

        List<float> dists = new List<float> {dist_STD, dist_SIT, dist_JOG, dist_STR, dist_OHD, dist_TWS};
        // int MinIndex = dists.IndexOf(dists, dists.Min());
        int MinIndex = dists.IndexOf(dists.Min());
        return Acts[MinIndex];
    }

    string GetCurrentActivity(Dictionary<string, Vector3> attributes)
    {
        List<float> vector = ProcessData();
        string output = Classify(vector);
        return output;
    }

    // Update is called once per frame
    void Update()
    {
        secondsCount -= Time.deltaTime;
        sensorReader.RefreshTrackedDevices();

        // Fetch attributes as a dictionary, with <device>_<measure> as a key
        // and Vector3 objects as values
        var attributes = sensorReader.GetSensorReadings();

        // add data from current frame into our data from current session
        GetData(attributes);
        float avgVelLeft = CalcAvgVel();

        if (avgVelLeft >= 0.01)
        {
            moving = true;
        } else {
            if (moving == true)
            {
                reps += 1;
                moving = false;
            }
        }

        text.text = "Reps: " + (int)Math.Floor(reps / 2);
        //get current activity every 2.9 seconds
        if (updatedData[0].Count >= 100)
        {
            //currentActivity = GetCurrentActivity(attributes);

            // clear updatedData so each update is based on most recent 2.9 seconds
            for (int i = 0; i < 36; i++)
            {
                updatedData[i].RemoveAt(0);
            }
            //secondsCount = 2.9f;
        }
    }
}
