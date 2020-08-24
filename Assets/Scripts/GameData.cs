using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public float BGM_Volume = 1.0f;
    public float SE_Volume = 1.0f;
    public bool Mute = false;

    public SenarioSO scenarioSO;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
        // ScenarioDataのリストを作成
        CreateScenarioDataList();
    }

    /// <summary>
    /// SenarioDataのリストを作成
    /// </summary>
    private void CreateScenarioDataList() {
        // 初期化
        scenarioSO.senarioMasterData = new SenarioMasterData();

        // Jsonファイルを元にSenarioDataを作成する
        scenarioSO.senarioMasterData = LoadMasterDataFromJson.LoadSenarioMasterDataFromJson();

        // 文字列を適宜な型に変換して配列に代入
        foreach (SenarioMasterData.SenarioData senarioData in scenarioSO.senarioMasterData.senario) {
            senarioData.messages = senarioData.messageString.Split(',').ToArray();
            senarioData.charaTypes = senarioData.charaNoString.Split(',').Select(x => (CHARA_NAME_TYPE)Enum.Parse(typeof(CHARA_NAME_TYPE), x)).ToArray();
            senarioData.branchs = senarioData.branchString.Split(',').Select(x => int.Parse(x)).ToArray();

            senarioData.branchMessages = senarioData.branchMessageString.Split(',').ToArray(); // 追加。分岐メッセージを配列にする

            List<string> strList = senarioData.displayCharaString.Split('/').ToList();

            int i = 0;
            senarioData.displayCharas = new Dictionary<int, CHARA_NAME_TYPE[]>();
            foreach (string str in strList) {
                CHARA_NAME_TYPE[] displayChara = str.Split(',').Select(x => (CHARA_NAME_TYPE)Enum.Parse(typeof(CHARA_NAME_TYPE), x)).ToArray();
                senarioData.displayCharas.Add(i, displayChara);
                i++;
            }
        }
        Debug.Log("Create SenarioDataList");
    }
}
