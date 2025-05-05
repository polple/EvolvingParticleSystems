using UnityEngine;
using System.IO;

public class WriteDataToFile : MonoBehaviour
{

    private int ExecNum = 0; //Overwritten in Start() to mark the number of profiles that exist //A new profile is created every execution
    public string fileName = ""; //Overwritten in Start()

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //mark that a new file is created and get the current file number
        ExecNum = PlayerPrefs.GetInt("ExecutionNumber", 0);
        ExecNum++;
        PlayerPrefs.SetInt("ExecutionNumber", ExecNum);
        PlayerPrefs.Save();
        string subfolder = "PreviousRuns";

        //If subfolder isn't there then make it
        if (!Directory.Exists(subfolder))
        {
            Directory.CreateDirectory(subfolder);
        }

        //create the final filename
        fileName = Path.Combine(subfolder, "Execution_No_" + ExecNum.ToString() + ".txt");
    }


    public void WriteToFile(string contentToWrite)
    {
        using (StreamWriter writer = new StreamWriter(fileName, true))
        {
            writer.WriteLine(contentToWrite);
        }
    }

}
