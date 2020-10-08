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

    public List<int> chooseBranchList = new List<int>();

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
            
            // 分岐番号の文字列をカンマ位置で分割して配列に代入
            string[] tempBranchNos = senarioData.branchString.Split(',').ToArray();

            // 分岐番号用の配列を初期化
            senarioData.branchs = new int[tempBranchNos.Length];

            // 分岐の数だけ繰り返して、条件付きの分岐と条件のない通常の分岐とを分ける
            for (int x = 0; x < tempBranchNos.Length; x++) {    // - は、マイナスとして判定されてしまって通らないので使わない
                if (tempBranchNos[x].Contains("/")) {
                    // 条件付きの分岐の場合、/ の位置で文字列を分割
                    int[] conditionalNo = tempBranchNos[x].Split('/').Select(s => int.Parse(s)).ToArray();
                    // 最初の番号を分岐番号として代入
                    senarioData.branchs[x] = conditionalNo[0];
                    // 次の番号を条件の番号として代入
                    senarioData.conditionalBranchNo.Add(conditionalNo[1]);                    
                } else {
                    // 条件のない通常の分岐はそのまま配列に代入
                    senarioData.branchs[x] = int.Parse(tempBranchNos[x]); 
                }
            } 
  
            // 今までの分岐番号の代入は不要になる
            //senarioData.branchs = senarioData.branchString.Split(',').Select(x => int.Parse(x)).ToArray();

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
