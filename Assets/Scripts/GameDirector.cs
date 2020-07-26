using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using System;

public class GameDirector : MonoBehaviour
{
    public List<BranchSelectButton> branchSelectButtonList = new List<BranchSelectButton>();
    public BranchSelectButton BranchSelectButtonPrefab;
    public Transform branchButtonTran;

    public SenarioSO senarioSO;
    public TextMessageViewer textMessageViewer;

    private int currentSenarioNo;

    void Start(){
        // ゲーム画面の準備

        // SenarioDataの準備
        //if (senarioSO.senarioMasterData.senario.Count > 0) {
            // SenarioDataの作成
            CreateSenarioData();           
        //} else {
        //    // SenarioDataの読み込み
        //    LoadSenarioData();
        //}
        // シナリオの初期値
        currentSenarioNo = 0;

        // 最初のSenarioを読み込んでゲームスタート
        SetUpSenario(currentSenarioNo);

        // 分岐用ボタン生成のDebug
        //StartCoroutine(CreateBranchSelectButton(3));
    }

    /// <summary>
    /// SenarioDataの作成
    /// </summary>
    private void CreateSenarioData() {
        // 初期化
        senarioSO.senarioMasterData = new SenarioMasterData();
        // SenarioDataを作成してない場合にはJsonファイルを元にSenarioDataを作成する

        // ジェネリクス版　問題なく動く
        //senarioSO.senarioMasterData = LoadMasterDataFromJson.LoadFromJson<SenarioMasterData>();

        // SenarioDataを作成してない場合にはJsonファイルを元にSenarioDataを作成する
        senarioSO.senarioMasterData = LoadMasterDataFromJson.LoadSenarioMasterDataFromJson();        

        // 文字列を適宜な型に変換して配列に代入
        foreach (SenarioMasterData.SenarioData senarioData in senarioSO.senarioMasterData.senario) {
            senarioData.messages = senarioData.messageString.Split(',').ToArray();
            senarioData.charaTypes = senarioData.charaNoString.Split(',').Select(x => (CHARA_NAME_TYPE)Enum.Parse(typeof(CHARA_NAME_TYPE), x)).ToArray();
            senarioData.branchs = senarioData.branchString.Split(',').Select(x => int.Parse(x)).ToArray();

            List<string> strList = senarioData.displayCharaString.Split('/').ToList();

            int i = 0;
            senarioData.displayCharas = new Dictionary<int, CHARA_NAME_TYPE[]>();
            foreach (string str in strList) {
                CHARA_NAME_TYPE[] displayChara = str.Split(',').Select(x => (CHARA_NAME_TYPE)Enum.Parse(typeof(CHARA_NAME_TYPE), x)).ToArray();
                senarioData.displayCharas.Add(i, displayChara);
                i++;
            }
        }
        Debug.Log("Create SenarioData");
    }

    /// <summary>
    /// SenarioDataの読み込み
    /// </summary>
    private void LoadSenarioData() {
        // ResousesフォルダよりScriptableObjectを読み込む
        senarioSO = Resources.Load<SenarioSO>("MasterData/SenarioMasterData");
        Debug.Log("Load SenarioData");
    }

    private void SetUpSenario(int nextSenarioNo) {
        // 現在のシナリオ番号を更新
        currentSenarioNo = nextSenarioNo;

        SenarioMasterData.SenarioData senarioData = senarioSO.senarioMasterData.senario.Find(x => x.senarioNo == currentSenarioNo);
        textMessageViewer.SetUpSenarioData(senarioData);
    }

    private void SetupEnding(int endingSenarioNo) {

    }

    /// <summary>
    /// 全メッセージ再生後に分岐用ボタンを作成
    /// </summary>
    public IEnumerator CreateBranchSelectButton(int branchCount) {
        // 分岐ボタンの生成
        for (int i = 0; i < branchCount; i++) {
            BranchSelectButton branchSelectButton = Instantiate(BranchSelectButtonPrefab, branchButtonTran, false);
            branchSelectButton.InitializeBranchSelect("選択肢" + i, i, this, i);
            branchSelectButtonList.Add(branchSelectButton);
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// 選択した分岐の番号で次のシナリオを設定
    /// </summary>
    /// <param name="branchNo"></param>
    public void ChooseBranch(int senarioNo) {
        Debug.Log("分岐選択 シナリオ番号 :" + senarioNo);

        // 次のシナリオの呼び出し
        SetUpSenario(senarioNo);
    }

    public void InactiveBranchSelectButtons() {
        // 分岐ボタンを重複して押せないように制御

        // 動いたがBoolのリストがそのあと使えないので保留(Selectの結果の値が左辺で代入されるので、同じ型を用意する)
        //List<bool> result = branchSelectButtonList.Where(x => x.isClickable == false).Select(x => x.btnBranchSelectButton.interactable = false).ToList();
        //Debug.Log(result.Count);

        for (int i = 0; i < branchSelectButtonList.Count; i++) {
            if (!branchSelectButtonList[i].isClickable) {
                branchSelectButtonList[i].isClickable = true;
                branchSelectButtonList[i].canvasGroup.DOFade(0.0f, 0.5f);
            }
        }
        // リストをクリア
        branchSelectButtonList.Clear();
    }
}
