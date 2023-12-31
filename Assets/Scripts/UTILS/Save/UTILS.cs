using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class UTILS : MonoBehaviour
{

    #region RunData
    static string runDataName = "runData";
    /// <summary>
    /// RunData 불러오기
    /// </summary>
    /// <returns></returns>
    public static RunData GetRunData()
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + runDataName;

        if(File.Exists(finalPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(finalPath, FileMode.Open);

            RunData data =(RunData)bf.Deserialize(fileStream);
            Debug.Log("RunData 로딩 성공!");

            fileStream.Close();
            return data;
        }
        else
        {
            Debug.Log("RunData 존재하지 않음!");
            return null;
        }
    }

    /// <summary>
    /// RunData 저장하기
    /// </summary>
    /// <param name="data"></param>
    public static void SaveRunData(RunData data)
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + runDataName;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(finalPath);

        bf.Serialize(fileStream, data);
        fileStream.Close();
        Debug.Log("SaveSucccess!!!!");
    }

    /// <summary>
    /// 저장되어있는 RunData 삭제하기(reset)
    /// </summary>
    [MenuItem("MyTools/DeleteRunData")]
    public static void DeleteRunData()
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + runDataName;

        if (File.Exists(finalPath))
        {
            File.Delete(finalPath);
            Debug.Log("RunData 삭제됨!");
        }
    }

    #endregion

    #region SaveData
    static string saveDataName = "SaveData";
    public static void SaveSaveData(SaveData data)
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + saveDataName;

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(finalPath, jsonData);

    }

    public static SaveData LoadSaveData()
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + saveDataName;

        if (File.Exists(finalPath))
        {
            string jsonData = File.ReadAllText(finalPath);
            return JsonUtility.FromJson<SaveData>(jsonData);
        }
        else
        {
            return new SaveData();
        }
    }

    /// <summary>
    /// 저장되어있는 SaveData 삭제하기(reset)
    /// </summary>
    [MenuItem("MyTools/DeleteSaveData")]
    public static void DeleteSaveData()
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + saveDataName;

        if (File.Exists(finalPath))
        {
            File.Delete(finalPath);
            Debug.Log("SaveData 삭제됨!");
        }
    }

    #endregion

    #region 해금

    public static void CheckEXP()
    {
        SaveData data = UTILS.LoadSaveData();

        if(data.Exp >= 100)
        {
            unlock(data);
        }
    }
    public static void unlock(SaveData data)
    {
        data.Exp -= 100;
        data.ProgressLV++;
        //TODO: ProgressLV 체크하기.

        UTILS.SaveSaveData(data);
    }


    #endregion
}