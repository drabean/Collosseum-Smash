using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class UTILS : MonoBehaviour
{
    static string runDataName = "runData";
    /// <summary>
    /// RunData �ҷ�����
    /// </summary>
    /// <returns></returns>
    public static runData GetRunData()
    {
        string persistentPath = Application.persistentDataPath;
        string finalPath = persistentPath + "/" + runDataName;

        if(File.Exists(finalPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(finalPath, FileMode.Open);

            runData data =(runData)bf.Deserialize(fileStream);
            Debug.Log("RunData �ε� ����!");

            fileStream.Close();
            return data;
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
    public static void SaveRunData(runData data)
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
    /// ����Ǿ��ִ� RunData �����ϱ�(reset)
    /// </summary>
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
}

//���� �� �ѹ��� Run�� ���õ� ������
[Serializable]
public class runData
{
    public runData() { }
    public runData(int characterInfoIdx, int nextStage, List<int> item, List<int> clearedStages)
    {
        this.characterInfoIdx = characterInfoIdx;
        this.nextStage = nextStage;
        this.item = item;
        this.clearedStages = clearedStages;
    }

    public int characterInfoIdx;
    public int nextStage;
    public List<int> item = new List<int>();
    public List<int> clearedStages = new List<int>();
}