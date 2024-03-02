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
    /// RunData �ҷ�����
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

            if (fileStream != null)
            {
                RunData data = (RunData)bf.Deserialize(fileStream);
                Debug.Log("RunData �ε� ����!");

                fileStream.Close();
                return data;
            }
            else
            {
                Debug.Log("������ �д� �������� ���� �߻�");
                return null;
            }
        }
        else
        {
            Debug.Log("RunData �������� ����!");
            return null;
        }
    }

    /// <summary>
    /// RunData �����ϱ�
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
            Debug.Log("SAVE DATA �ε� ����!");
            return JsonUtility.FromJson<SaveData>(jsonData);
        }
        else
        {
            Debug.Log("SAVE DATA �ε� ���� / ���ο� ������ ����");
            return new SaveData();
        }
    }

    #endregion

    #region Setting
    static string settingDataName = "Settings";
    public static Settings GetSettingData()
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + settingDataName;

        if (File.Exists(finalPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(finalPath, FileMode.Open);

            Settings data =(Settings)bf.Deserialize(fileStream);
            Debug.Log("Setting �ε� ����!");

            fileStream.Close();
            return data;
        }
        else
        {
            Debug.Log("Setting �������� ����!");
            Settings newSetting = new Settings();
            SaveSettingData(newSetting);

            return newSetting;
        }

    }

    public static void SaveSettingData(Settings data)
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + settingDataName;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Create(finalPath);

        bf.Serialize(fileStream, data);
        fileStream.Close();
    }



    #endregion


#if UNITY_EDITOR

    /// <summary>
    /// ����Ǿ��ִ� SaveData �����ϱ�(reset)
    /// </summary>
    [MenuItem("MyTools/DeleteSaveData")]
#endif
    public static void DeleteSaveData()
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + saveDataName;

        if (File.Exists(finalPath))
        {
            File.Delete(finalPath);
            Debug.Log("SaveData ������!");
        }
    }
#if UNITY_EDITOR

    /// <summary>
    /// ����Ǿ��ִ� RunData �����ϱ�(reset)
    /// </summary>
    [MenuItem("MyTools/DeleteRunData")]
#endif
    public static void DeleteRunData()
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + runDataName;

        if (File.Exists(finalPath))
        {
            File.Delete(finalPath);
            Debug.Log("RunData ������!");
        }
    }
#if UNITY_EDITOR

    /// <summary>
    /// ����Ǿ��ִ� Setting �����ϱ�(reset)
    /// </summary>
    [MenuItem("MyTools/DeleteSettingData")]
#endif
    public static void DeleteSettingData()
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + settingDataName;

        if (File.Exists(finalPath))
        {
            File.Delete(finalPath);
            Debug.Log("SaveData ������!");
        }
    }

    #region �ر�

    public static void CheckEXP()
    {
    }
    public static void unlock(SaveData data)
    {
        data.Exp -= 100;
        //TODO: ProgressLV üũ�ϱ�.
    }


    #endregion
}