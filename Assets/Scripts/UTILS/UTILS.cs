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
    /// RunData 불러오기
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
    /// 저장되어있는 RunData 삭제하기(reset)
    /// </summary>
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
}

//게임 중 한번의 Run에 관련된 데이터
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