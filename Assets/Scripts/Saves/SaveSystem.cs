using System.IO;
using UnityEngine;

public class SaveSystem
{
    // public static readonly string SAVE_FOLDER = Application.persistentDataPath  + "/Saves";
    //Later can use persistent for builds
    public static readonly string SAVE_FOLDER = Application.dataPath  + "/Saves/";

    public static void Init()
    {
        //Test if save Folder exists
        if (!Directory.Exists((SAVE_FOLDER)))
        {
            //Create Save Folder
            Directory.CreateDirectory(SAVE_FOLDER);
            Debug.Log(SAVE_FOLDER);
        }
    }

    public static void Save(string saveString)
    {
        Debug.Log("SAVING");
        File.WriteAllText(SAVE_FOLDER + "save.json", saveString);
    }

    public static string Load()
    {
        Debug.Log("LOADING");
        if (File.Exists(SAVE_FOLDER + "save.json"))
        {
            string saveString = File.ReadAllText(SAVE_FOLDER + "save.json");
            return saveString;
        }
        else
        {
            
            return null;
        }
    }
}
