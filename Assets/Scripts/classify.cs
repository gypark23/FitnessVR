using System;
using System.Collections.Generic;
using UnityEngine;



class Classify
{
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

    List<float> ProcessData(List<List<float>> data)
    {
        List<float> mean = new List<float>();
        List<float> var = new List<float>();
        List<float> vector = new List<float>();

        foreach (List<float> list in data)
        {
            float mean = list.Average();
            float variance = list.Sum(x => (x - mean) * (x - mean)) / list.Count;
            mean.Add(mean)
            var.Add(variance);
        }

        vector.AddRange(mean);
        vector.AddRange(var);

        return vector;
    }

    float CalcDist(List<float> StandardVector, List<float> vector)
    {
        float dist = 0.0f;

        for (int i = 0; i < list1.Count; i++)
        {
            float diff = (StandardVector[i] - vector[i])/StandardVector[i];
            dist = dist + diff*diff
        }     
    }

    string Classify(List<float> vector) 
    {
        float dist_STD = ActivityDetector.CalcDist(STD, vector)
    }


}























class CalcAttribute
{
    static void Main(Vector3 attributes)
    {
        double mean = FindMean(data);
        double SD = FindSD(data, mean);
        double threshold = mean + 2 * SD;

        Console.WriteLine("Threshold value: " + threshold);
        Console.WriteLine("Classification: ");

        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] > threshold)
            {
                Console.WriteLine(data[i] + " is classified as High");
            }
            else
            {
                Console.WriteLine(data[i] + " is classified as Low");
            }
        }
    }

    static double FindMean(double[] data)
    {
        double sum = 0;
        for (int i = 0; i < data.Length; i++)
        {
            sum += data[i];
        }
        return sum / data.Length;
    }

    static double FindSD(double[] data, double mean)
    {
        double sumOfSquaredDifferences = 0;
        for (int i = 0; i < data.Length; i++)
        {
            sumOfSquaredDifferences += Math.Pow(data[i] - mean, 2);
        }
        double variance = sumOfSquaredDifferences / data.Length;
        return Math.Sqrt(variance);
    }
}



