using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadDataSelectButton : MonoBehaviour
{
    public Button btnLoad;

    public Text txtLoadDataInfo;

    private int branchNo;

    private DataLoadPopUp dataLoadPopUp;

    public bool isClickable;

    public CanvasGroup canvasGroup;

    public void SetUpLoadSelectButton(int branchNo, string date, DataLoadPopUp dataLoadPopUp) {
        this.branchNo = branchNo;
        this.dataLoadPopUp = dataLoadPopUp;

        // ボタンに保存時間を表示
        txtLoadDataInfo.text = "保存時間 : " + date;

        // ボタンにメソッドを登録
        btnLoad.onClick.AddListener(OnClickLoadButton);
    }

    private void OnClickLoadButton() {
        if (isClickable) {
            // 一度タップしたら処理しない
            return;
        }

        // 一度タップしたら押せなくする
        isClickable = true;

        // 他のロードボタンも押せなくする
        dataLoadPopUp.InactiveLoadSelectButtons();

        // 選択したボタンの分岐番号をGameDataに保存
        GameData.instance.loadBranchNo = branchNo;

        Debug.Log("データロード完了 分岐番号 : " + GameData.instance.loadBranchNo + "からゲーム再開");

        // ゲームシーンをロード
        dataLoadPopUp.LoadGame();
    }
}
