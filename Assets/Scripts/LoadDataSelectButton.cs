using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadDataSelectButton : MonoBehaviour
{
    // UI関連
    [SerializeField]
    private Button btnLoad;                 // ボタン制御用

    [SerializeField]
    private Text txtLoadDataInfo;           // Text制御用。セーブ番号と保存時間表示

    public CanvasGroup canvasGroup;         // CanvasGroupの透明度の制御用

    public bool isClickable;                // 重複タップ防止制御用。true はタップ済

    private int branchNo;                   // セーブされている分岐番号

    private DataLoadPopUp dataLoadPopUp;    // DetaLoadPopUpへの参照


    /// <summary>
    /// ロードボタンの設定
    /// </summary>
    /// <param name="branchNo"></param>
    /// <param name="date"></param>
    /// <param name="dataLoadPopUp"></param>
    /// <param name="no"></param>
    public void SetUpLoadSelectButton(int branchNo, string date, DataLoadPopUp dataLoadPopUp, int no) {
        this.branchNo = branchNo;
        this.dataLoadPopUp = dataLoadPopUp;

        // ボタンに保存時間を表示
        txtLoadDataInfo.text = "セーブ番号 : " + no + "   保存時間 : " + date;

        // ボタンにメソッドを登録
        btnLoad.onClick.AddListener(OnClickLoadButton);
    }

    /// <summary>
    /// ボタンを押下した際の処理
    /// </summary>
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
