using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOverCanvas : MonoBehaviour
{
    Player player;

    private void Awake()
    {
        if (!LoadedData.Inst.isDataLoaded) LoadedData.Inst.LoadData();

        runData data = UTILS.GetRunData();

        if(data == null)
        {
            data = new runData(0, 0, new List<int>(), new List<int>());
        }

        player = Instantiate(LoadedData.Inst.getCharacterInfoByID(data.characterInfoIdx).playerPrefab, new Vector3(0,0,0), Quaternion.identity);
        player.StartDeadMotion();
    }

    private void Start()
    {
        SoundMgr.Inst.PlayBGM("GameOver");
    }

    public void BtnContinueStage()
    {
        player.EndDeadMotion();
        player.AutoMove(new Vector3(15, 0, 0));
        StartCoroutine(SoundMgr.Inst.co_BGMFadeOut());
        StartCoroutine(co_ContinueStage());
    }

    IEnumerator co_ContinueStage()
    {
        yield return new WaitForSeconds(2.0f);
        LoadSceneMgr.LoadSceneAsync("Main");
    }

    public void BtnEndStage()
    {
        LoadSceneMgr.LoadSceneAsync("Start");
    }
}
