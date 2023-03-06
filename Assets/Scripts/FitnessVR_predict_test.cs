using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

public class FitnessVR_predict_test : MonoBehaviour
{
    OculusSensorReader sensorReader;
    // text to display the detected activity
    public TextMesh text;

    public TextMesh testText;
    // float to keep track of time
    private float time;
    private float secondsCount;
    // string to store current activity
    private string currentActivity;

    private TcpClient client;
    private NetworkStream stream;
    
    // Declare the variable you want to send to your computer
    private List<List<float>> updatedData = new List<List<float>>();

    

    // Start is called before the first frame update
    void Start()
    {
        // Add 37 empty lists to updatedData where we will
        // store a history of real time data
        for (int i = 0; i < 37; i++)
        {
            updatedData.Add(new List<float>());
        }
        
        time = 0.0f;
        sensorReader = new OculusSensorReader();
        text = GetComponent<TextMesh>();
        
        // Connect to your computer's IP address on a specific port
        client = new TcpClient("10.150.108.174", 1234);
        stream = client.GetStream();
        
        testText.text = "Hello";
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
            updatedData[i+1].Add(data[i]);
        }
    }

    public static string ConvertListToString(List<List<float>> inputList)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var subList in inputList)
        {
            foreach (var item in subList)
            {
                sb.Append(item.ToString() + " ");
            }
            sb.Remove(sb.Length - 1, 1); // remove last space
            sb.AppendLine();
        }

        return sb.ToString();
    }

    void OnDestroy()
    {
        // Close the network connection when the app is closed
        stream.Close();
        client.Close();
    }


    // Update is called once per frame
    void Update()
    {
        sensorReader.RefreshTrackedDevices();
        time += Time.deltaTime;
        secondsCount += Time.deltaTime;
        if (secondsCount > 2)
        {
            // Fetch attributes as a dictionary, with <device>_<measure> as a key
            // and Vector3 objects as values
            var attributes = sensorReader.GetSensorReadings();
            // add data from current frame into our data from current session
            GetData(attributes);
            // add time to updatedData
            
            updatedData[0].Add(time);

            //if (updatedData[0].Count == 144){ // assuming 72 HZ, send 2 seconds of data
            // Convert the variable to a byte array and send it to your computer
            string outputString = ConvertListToString(updatedData);
            byte[] data = Encoding.ASCII.GetBytes(outputString);
            stream.Write(data, 0, data.Length);
            // byte[] data2 = Encoding.ASCII.GetBytes(data.Length.ToString());
            // text.text = data.Length.ToString();
            // stream.Write(data2, 0, data2.Length); //TO FIND LENGTH OF STRING


            // clear all data
            for (int i = 0; i < 37; i++)
            {
                //time = 0.0f;
                updatedData[i].Clear();
            }
            secondsCount = 0.0f;
        }
        
        //}

        

        //testText.text = Time.deltaTime.ToString();
        //text.text = "TEST";
        
        
    }
}
