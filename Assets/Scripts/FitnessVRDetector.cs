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
    // variables for network connection
    private string connectionIP = "10.150.57.172"; // IP address of Oculus
    private int connectionPort = 25001;
    IPAddress localAdd;
    TcpListener listener;
    TcpClient client;

    // text to show activity on
    public TextMesh text;

    // time since the start of the scene
    float time;
    // timer to send data every 0.5 seconds
    float timer;

    // string to store predicted exercise
    string exercise;

    // list of lists of floats to store one frame of VR motion data
    private List<List<float>> updatedData = new List<List<float>>();

    private void Update()
    {
        time += Time.deltaTime;
        timer += Time.deltaTime;
        if (timer > 0.5f)
        {
            timer = 0.0f;
            SendAndReceiveData();
        }
        text.text = exercise; //assigning exercise in SendAndReceiveData()
    }

    private void Start()
    {
        sensorReader = new OculusSensorReader();
        time = 0.0f;
        timer = 0.0f;
        GetConnection();
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

    // conver a list of list of motion data to a string
    public static string ConvertListToString(List<List<float>> inputList)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var subList in inputList)
        {
            foreach (var item in subList)
            {
                sb.Append(item.ToString() + " ");
            }
            sb.Remove(sb.Length - 1, 1); // remove last spaces
            sb.AppendLine();
        }

        return sb.ToString();
    }

    // listen for and accept connection
    void GetConnection()
    {
        localAdd = IPAddress.Parse(connectionIP);
        listener = new TcpListener(IPAddress.Any, connectionPort);
        listener.Start();

        client = listener.AcceptTcpClient();
        text.text = "Connected";

        // add empty lists to our variable that stores motion data
        for (int i = 0; i < 37; i++)
        {
            updatedData.Add(new List<float>());
        }
    }

    // send most updated motion data and receive a prediction
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
        updatedData[0].Add((time * 1000) % 10000);
        
        // convert data to string
        string message = ConvertListToString(updatedData);

        // send data to computer
        byte[] myWriteBuffer = Encoding.ASCII.GetBytes(message); // converting string to byte data
        nwStream.Write(myWriteBuffer, 0, myWriteBuffer.Length); // sending the data in bytes to Python

        // receiving data from computer
        int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize); // getting data in bytes from Python
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); // converting byte data to string

        if (dataReceived != null)
        {
            // update exercise to received prediction
            exercise = dataReceived;
        }
    }

    // close connections on destroy
    void OnDestroy()
    {
        NetworkStream nwStream = client.GetStream();
        nwStream.Close();
        client.Close();
    }
}