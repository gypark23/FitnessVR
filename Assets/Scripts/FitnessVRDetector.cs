using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

public class FitnessVRDetector : MonoBehaviour
{
    OculusSensorReader sensorReader;
    Thread mThread;
    public string connectionIP = "192.168.1.22";
    public int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;

    public TextMesh text;

    float time;

    string exercise;
    private List<List<float>> updatedData = new List<List<float>>();
    
    bool running;

    private void Update()
    {
        time += Time.deltaTime;
        text.text = exercise; //assigning exercise in SendAndReceiveData()
    }

    private void Start()
    {
        sensorReader = new OculusSensorReader();
        time = 0.0f;
        ThreadStart ts = new ThreadStart(GetInfo);
        mThread = new Thread(ts);
        mThread.Start();
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

    void GetInfo()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();

        for (int i = 0; i < 37; i++)
        {
            updatedData.Add(new List<float>());
        }

        running = true;
        while (running)
        {
            Thread.Sleep(2000);
            SendAndReceiveData();
        }
        listener.Stop();
    }

    void SendAndReceiveData()
    {
        NetworkStream nwStream = client.GetStream();
        byte[] buffer = new byte[client.ReceiveBufferSize];
        
        // update to most recent data
        for (int i = 0; i < 37; i++)
        {
            updatedData[i].Clear();
        }
        sensorReader.RefreshTrackedDevices();
        var attributes = sensorReader.GetSensorReadings();
        GetData(attributes);
        updatedData[0].Add(time * 1000);
        
        string message = ConvertListToString(updatedData);

        //---Sending Data to Host----
        byte[] myWriteBuffer = Encoding.ASCII.GetBytes(message); //Converting string to byte data
        nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); //Sending the data in Bytes to Python

        //}


        //---receiving Data from the Host----
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize); //Getting data in Bytes from Python
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Converting byte data to string

        if (dataReceived != null)
        {
            //---Using received data---
            exercise = dataReceived; //<-- assigning exercise value from Python

            
        }
    }

    void OnDestroy()
    {
        NetworkStream nwStream = client.GetStream();
        nwStream.Close();
        client.Close();
    }
}