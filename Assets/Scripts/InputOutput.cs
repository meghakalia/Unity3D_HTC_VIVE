using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class InputOutput : MonoBehaviour
{

    //string filePath = getPath();
    public string filePath = "test.csv"; // to give in the editor 

    bool onlyOnce = true;

    void writeToFile(string str_output)
    {
        //do this only once 
        //FileStream writer = File.OpenWrite(filePath);

        using FileStream fileStream = File.Open(filePath, FileMode.Append);
        using StreamWriter writer = new StreamWriter(fileStream);

        writer.WriteLine(str_output);

        writer.Flush();
        writer.Close();

    }

    // Start is called before the first frame update
    void Start()
    {
        writeToFile("blocCounter, AsynchronyVal, tactileFirst, tactileOdd,visOdd, correctRes, subResponse,stimulusDuration");
    }

    // Update is called once per frame
    void Update()
    {
        if (onlyOnce)
        {
            decimal[] arr = { 10.0m, 5.0m, 6.0m, 90.00m, 45.8m, 20.9m, 60.2m, 5.0m };
            writeToFile(string.Join(", ", arr));
            onlyOnce = false;
        }
    }
}
