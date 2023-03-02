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
    // float to detect activity every 2.9 seconds
    private float secondsCount;
    // string to store current activity
    private string currentActivity;

    private TcpClient client;
    private NetworkStream stream;
    
    // Declare the variable you want to send to your computer
    private int myVariable = 42;

    

    // Start is called before the first frame update
    void Start()
    {
        sensorReader = new OculusSensorReader();
        text = GetComponent<TextMesh>();
        
        // Connect to your computer's IP address on a specific port
        client = new TcpClient("10.150.10.209", 1234);
        stream = client.GetStream();
        
        testText.text = "Hello";
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
        

        secondsCount -= Time.deltaTime;
        sensorReader.RefreshTrackedDevices();

        // Fetch attributes as a dictionary, with <device>_<measure> as a key
        // and Vector3 objects as values
        var attributes = sensorReader.GetSensorReadings();

        // Convert the variable to a byte array and send it to your computer
        byte[] data = Encoding.ASCII.GetBytes(myVariable.ToString());
        stream.Write(data, 0, data.Length);
        
        testText.text = Time.deltaTime.ToString();
        text.text = "TEST";
        
        
    }
}
