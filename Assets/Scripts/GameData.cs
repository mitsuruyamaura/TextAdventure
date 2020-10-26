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

    public int endingCount;

    public List<int> endingNos = new List<int>();

    private string ENDING = "ending_";

    private string READ_BRANCH_NO = "readBranchNo_";

    public List<int> readBranchNoList = new List<int>();


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
                    // 最後の番号を分岐番号として代入
                    senarioData.branchs[x] = conditionalNo[1];
                    // 最初の番号を条件の番号として代入
                    senarioData.conditionalBranchNo.Add(conditionalNo[0]);                    
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

    /// <summary>
    /// 見たエンディングの番号を保存
    /// </summary>
    /// <param name="endingNo"></param>
    public void SaveEndingData(int endingNo) {
        PlayerPrefs.SetInt(ENDING + endingNo.ToString(), endingNo);
        PlayerPrefs.Save();
    }    

    /// <summary>
    /// 保存されているエンディングがあるか確認し、すべて通過している場合にはtrueとする
    /// </summary>
    /// <returns></returns>
    public bool LoadCheckEndingData() {
        // 現在までに見ているエンディングを確認     
        for (int i = 1; i < endingCount + 1; i++) {
            // 保存されているエンディングを１つずつ確認
            if (PlayerPrefs.HasKey(ENDING + i.ToString())) {
                // 見ているエンディングの番号だけ読み込み
                endingNos.Add(PlayerPrefs.GetInt(ENDING + i.ToString(), 0));
            }
        }
        // 見ているエンディングの数とエンディングの総数が同じになったらコンプリート
        return endingCount == endingNos.Count ? true : false;
    }

    /// <summary>
    /// 既読のシナリオ分岐番号の登録(現在の登録のタイミングは各シナリオの全メッセージ表示後)
    /// </summary>
    /// <param name="branchNo"></param>
    public void SaveReadBranchNo(int branchNo) {
        PlayerPrefs.SetInt(READ_BRANCH_NO + branchNo.ToString(), branchNo);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 既読のシナリオ分岐番号を取得
    /// </summary>
    public void LoadReadBranchNos() {
        // 全シナリオ分岐番号を順番に照合
        for (int i = 0; i < scenarioSO.senarioMasterData.senario.Count; i++) {
            // 保存されている分岐番号を１つずつ確認
            if (PlayerPrefs.HasKey(READ_BRANCH_NO + i.ToString())) {
                // 保存されている番号があったらListに追加
                readBranchNoList.Add(PlayerPrefs.GetInt(READ_BRANCH_NO + i.ToString()));

                Debug.Log("既読シナリオ番号 : " + PlayerPrefs.GetInt(READ_BRANCH_NO + i.ToString()));
            }
        }

        if (readBranchNoList.Count == 0) {
            Debug.Log("既読シナリオなし");
        }
    }
}
