using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class DataLoadPopUp : MonoBehaviour
{
    public LoadDataSelectButton loadSelectbuttonPrefab;    // ロード用ボタンのプレファブ

    [SerializeField]                                       // 生成したロード用ボタン管理用のリスト。Debugしやすいようにインスペクターに表示
    private List<LoadDataSelectButton> loadSelectButtonList = new List<LoadDataSelectButton>();

    [SerializeField]
    private Transform loadSelectButtonTran;                // ロードボタンの生成位置の指定用。ScrollViewのContentをアサインする

    [SerializeField]
    private CanvasGroup canvasGroup;                       // CanvasGroupの透明度の制御用

    [SerializeField]
    private Button btnClose;                               // ポップアップを閉じるボタンの制御用

    /// <summary>
    /// ポップアップの設定
    /// </summary>
    public void SetUpDataLoadPopUp() {
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1.0f, 1.0f);

        btnClose.onClick.AddListener(OnClickClosePopUp);

        // ロードボタン生成
        CreateLoadButtons();
    }

    /// <summary>
    /// セーブされている数だけロードボタンを生成
    /// </summary>
    private void CreateLoadButtons() {
        // コンストラクタを使ってDictinaryを初期化
        Dictionary<int, string> loadDatas = new Dictionary<int, string>(GameData.instance.GetSaveDatas());

        // セーブデータがない場合にはこの処理で終了
        if (loadDatas.Count == 0) {
            Debug.Log("SaveData なし");
            return;
        }

        // 通し番号用
        int i = 0;

        // セーブデータがあった場合、セーブデータの数だけロード用ボタンを生成。ScrollViewのContent(GridLayoutGruop付)に並べる
        foreach (KeyValuePair<int, string> item in loadDatas) {
            LoadDataSelectButton loadSelectButton = Instantiate(loadSelectbuttonPrefab, loadSelectButtonTran, false);

            i++;

            // ロードボタンの初期設定
            loadSelectButton.SetUpLoadSelectButton(item.Key, item.Value, this, i);

            // ボタン管理用リストに追加
            loadSelectButtonList.Add(loadSelectButton);
        }
    }

    /// <summary>
    /// タップされていないロードボタンを重複して押せないように制御
    /// </summary>
    public void InactiveLoadSelectButtons() {

        // ロードボタンをすべて照合
        for (int i = 0; i < loadSelectButtonList.Count; i++) {

            // 重複タップ防止制御が入っていない場合
            if (loadSelectButtonList[i].isClickable) {

                // 制御をいれて重複タップを防止
                loadSelectButtonList[i].isClickable = true;

                // 徐々に透明にする
                loadSelectButtonList[i].canvasGroup.DOFade(0.0f, 0.5f);
            }
        }
        // リストをクリア
        loadSelectButtonList.Clear();
    }

    /// <summary>
    /// セーブした分岐番号からゲーム再開
    /// </summary>
    public void LoadGame() {

        // Sequenceの初期化
        Sequence sequence = DOTween.Sequence();

        // 徐々にポップアップを透明にする
        sequence.Append(canvasGroup.DOFade(0, 1.0f));

        sequence.AppendInterval(0.2f).OnComplete(() => {
            // Gameシーンへ遷移
            SceneManager.LoadScene("Game");
        });
    }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    private void OnClickClosePopUp() {

        // Sequenceの初期化
        Sequence sequence = DOTween.Sequence();

        // 徐々にポップアップを透明にする
        sequence.Append(canvasGroup.DOFade(0, 1.0f));

        sequence.AppendInterval(0.2f).OnComplete(() => {
            // ポップアップを破棄
            Destroy(gameObject);
        });
    }
}
