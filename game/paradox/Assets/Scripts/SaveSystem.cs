using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveData(LevelManager levelMan){
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/game.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(levelMan);
        formatter.Serialize(stream,data);
        stream.Close();
    }

    public static GameData LoadData(){
        string path = Application.persistentDataPath + "/game.data";

        if(File.Exists(path)){
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;

        } else {
            //The file doesn't exist yet
            Debug.Log("File doesn't exist");
            return null;
        }

    }

    public static void DestroyData(){
        string path = Application.persistentDataPath + "/game.data";
        if(File.Exists(path)){
            File.Delete(path);
        }
    }
 
}
