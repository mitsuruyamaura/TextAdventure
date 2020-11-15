using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour 
{
    public bool isNewGameButton;

    // UI関連
    [SerializeField]
    private Button btnNewGame;

    [SerializeField]
    private Button btnDataLoad;

    [SerializeField]
    private Button btnStart;

    // 追加

    [SerializeField]
    private Button btnAlbum;                      // アルバムシーンへ遷移するボタンの制御用

    // ここまで

    [SerializeField]
    private DataLoadPopUp dataLoadPopUpPrefab;    // ロード用ポップアップのプレファブアサイン用

    [SerializeField]
    private Transform canvasTran;                 // ロード用ポップアップの生成位置

    private DataLoadPopUp dataLoadPopUp;          // 生成されたロード用ポップアップの代入用。複数生成を制御

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

        // セーブされている、既読のシナリオ分岐番号を取得
        GameData.instance.LoadReadBranchNos();

        // ロードボタンにメソッドを登録
        btnDataLoad.onClick.AddListener(OnClickDataLoad);

        // 追加

        // 回収しているCGがあるか確認して、回収しているCGはリストに登録
        GameData.instance.LoadGetCGNos();

        // アルバムシーンへの遷移処理を登録
        btnAlbum.onClick.AddListener(OnClickAlbumScene);

        // ここまで
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

        // ポップアップを設定
        dataLoadPopUp.SetUpDataLoadPopUp();
    }

    /// <summary>
    /// アルバムシーンへ遷移
    /// </summary>
    private void OnClickAlbumScene() {
        SceneManager.LoadScene("Album");
    }
}

