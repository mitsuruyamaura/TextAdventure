using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour 
{
    public bool isNewGameButton;

    public Button btnNewGame;

    public Button btnDataLoad;

    public Button btnStart;

    public DataLoadPopUp dataLoadPopUpPrefab;

    public Transform canvasTran;

    private DataLoadPopUp dataLoadPopUp;

    /// <summary>
    /// エンディングを見た数の確認
    /// </summary>
    private void CheckEndingCount() {

        // すべてのエンディングを見ているか確認。見ている場合には true 見ていない場合には false 
        isNewGameButton = GameData.instance.LoadCheckEndingData();

        // すべてのエンディングを見ている場合、新しいボタンを表示
        if (isNewGameButton == true) {
            btnNewGame.gameObject.SetActive(true);
            btnNewGame.onClick.AddListener(OnClickNewGameButton);
        }
    }

    /// <summary>
    /// Gameシーンへ遷移
    /// </summary>
    public void LoadMain() {
        SceneManager.LoadScene("Game");
    }

    void Start() {
        // 見ているエンディングがあるか確認
        if (GameData.instance.endingCount > 0) {
            CheckEndingCount();
        }

        btnStart.onClick.AddListener(LoadMain);

        // 追加

        // セーブされている、既読のシナリオ分岐番号を取得
        GameData.instance.LoadReadBranchNos();

        // ここまで

        // ロードボタンにメソッドを登録
        btnDataLoad.onClick.AddListener(OnClickDataLoad);

    }

    /// <summary>
    /// エンディング・コンプリート時に追加されるボタンに登録する処理
    /// </summary>
    public void OnClickNewGameButton() {
        // 新しいゲーム、エンディングのスタート内容を記述

    }

    /// <summary>
    /// ロード用ポップアップ生成
    /// </summary>
    public void OnClickDataLoad() {
        if (dataLoadPopUp != null) {
            // ロード用ポップアップがすでに生成されている場合には処理しない(複数生成を防止)
            return;
        }
        
        // ロード用ポップアップを生成
        dataLoadPopUp = Instantiate(dataLoadPopUpPrefab, canvasTran, false);

        dataLoadPopUp.SetUpDataLoadPopUp();
    }
}

