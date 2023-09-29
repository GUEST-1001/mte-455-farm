using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class SerializationManager : MonoBehaviour
{
    public static bool Save(string saveName, object saveManager)
    {
        BinaryFormatter formatter = GetBinaryFormatter();

        if (!Directory.Exists(Application.dataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.dataPath + "/saves");
        }

        string path = Application.dataPath + "/saves/" + saveName + ".sav";
        FileStream file = File.Create(path);

        formatter.Serialize(file, saveManager);
        file.Close();

        Debug.Log("saved to " + path);

        return true;
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        SurrogateSelector selector = new SurrogateSelector();

        Vector3SSurrogate vector3Surrogate = new Vector3SSurrogate();
        QuaternionSSurrogate quaternionSurrogate = new QuaternionSSurrogate();

        selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
        selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);

        formatter.SurrogateSelector = selector;

        return formatter;
    }

    public static object Load(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formatter = GetBinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            object save = formatter.Deserialize(file);
            file.Close();

            Debug.Log("loaded from " + path);

            return save;
        }
        catch
        {
            Debug.LogErrorFormat("Fail to load file at {0}", path);
            file.Close();
            return null;
        }
    }

}
