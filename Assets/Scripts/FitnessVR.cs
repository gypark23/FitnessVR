using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
public class FitnessVR : MonoBehaviour
{

    OculusSensorReader sensorReader;
    // text to display the counted reps
    public TextMesh text;
    // variables to count reps
    private bool moving = false;
    private double reps = 0;


    // variable to store VR motion data from current session
    List<List<float>> updatedData = new List<List<float>>();

    // Start is called before the first frame update
    void Start()
    {
        // Add 36 empty lists to updatedData where we will
        // store a history of real time data
        for (int i = 0; i < 36; i++)
        {
            updatedData.Add(new List<float>());
        }
        sensorReader = new OculusSensorReader();
        text = GetComponent<TextMesh>();
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

    // calculate average of mean velocities of the left controller
    float CalcAvgVelLeft()
    {
        List<float> vector = ProcessData();
        float leftAvg = (Math.Abs(vector[12]) + Math.Abs(vector[13]) + Math.Abs(vector[14])) / 3;
        return leftAvg;
    }

    // Update is called once per frame
    void Update()
    {
        sensorReader.RefreshTrackedDevices();

        // Fetch attributes as a dictionary, with <device>_<measure> as a key
        // and Vector3 objects as values
        var attributes = sensorReader.GetSensorReadings();

        // add data from current frame into our data from current session
        GetData(attributes);
        float avgVelLeft = CalcAvgVelLeft();

        // when the left controller goes from moving to still, we add a rep
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
        // reps is divided by 2 because one curl involves stopping movement twice
        text.text = "Reps: " + (int)Math.Floor(reps / 2);
        
        // only store the last 20 frames of data and delete the rest
        if (updatedData[0].Count >= 20)
        {
            for (int i = 0; i < 36; i++)
            {
                updatedData[i].RemoveAt(0);
            }
        }
    }
}
