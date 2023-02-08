using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class ActivityDetector : MonoBehaviour
{
    OculusSensorReader sensorReader;
    public TextMesh text;
    private int frame;
    private string test;

    TextAsset standardData;  //standardData.text to access string data
    List<float> STD = new List<float>();
    List<float> SIT = new List<float>();
    List<float> JOG = new List<float>();
    List<float> STR = new List<float>();
    List<float> OHD = new List<float>();
    List<float> TWS = new List<float>();

    List<string> Acts = new List<string> {"STD", "SIT", "JOG", "STR", "OHD", "TWS"};

    // Start is called before the first frame update
    void Start()
    {
        // Loading the lib for the standard vector of each activity
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

        frame = 0;
    }

    // convert Vec3 into an array of all its components
    List<List<float>> GetData(Dictionary<string, Vector3> attributes)
    {
        // get the 36 attributes
        var h_vel = attributes["headset_vel"];
        var h_ang = attributes["headset_ang"];
        var h_pos = attributes["headset_pos"];
        var h_rot = attributes["headset_rot"];

        var c_l_vel = attributes["controller_left_vel"];
        var c_l_ang = attributes["controller_left_ang"];
        var c_l_pos = attributes["controller_left_pos"];
        var c_l_rot = attributes["controller_left_rot"];

        var c_r_vel = attributes["controller_right_vel"];
        var c_r_ang = attributes["controller_right_ang"];
        var c_r_pos = attributes["controller_right_pos"];
        var c_r_rot = attributes["controller_right_rot"];


        List<List<double>> data = new List<List<double>>
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


        return data;
    }

    // calculate the mean and var of the raw data
    List<float> ProcessData(List<List<float>> data)
    {
        List<float> mean = new List<float>();
        List<float> var = new List<float>();
        List<float> vector = new List<float>();

        foreach (List<float> list in data)
        {
            float mean = list.Average();
            float variance = list.Sum(x => (x - mean) * (x - mean)) / list.Count;
            mean.Add(mean);
            var.Add(variance);
        }

        vector.AddRange(mean);
        vector.AddRange(var);

        return vector;
    }

    // calculate the distance between 2 arrays
    float CalcDist(List<float> StandardVector, List<float> vector)
    {
        float dist = 0.0f;

        for (int i = 0; i < list1.Count; i++)
        {
            float diff = (StandardVector[i] - vector[i])/StandardVector[i];
            dist = dist + diff*diff;
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
        int MinIndex = dists.IndexOf(dists, dists.Min());
        return Acts[MinIndex];
    }


    string GetCurrentActivity(Dictionary<string, Vector3> attributes)
    {
        
        List<List<float>> RawData = GetData(attributes);
        List<float> vector = ProcessData(RawData);
        string output = Classify(vector);
        return output;
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
