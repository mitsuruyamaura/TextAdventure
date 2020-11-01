using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class DataLoadPopUp : MonoBehaviour
{
    public LoadDataSelectButton loadSelectbuttonPrefab;   // ロード用ボタンのプレファブ

    public List<LoadDataSelectButton> loadSelectButtonList = new List<LoadDataSelectButton>();    // 生成したロード用ボタン管理用のリスト

    public Transform loadSelectButtonTran;            // ロードボタンの生成位置の指定

    public CanvasGroup canvasGroup;

    public Button btnClose;

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

        if (loadDatas.Count == 0) {
            Debug.Log("SaveData なし");
            return;
        }

        foreach (KeyValuePair<int, string> item in loadDatas) {
            LoadDataSelectButton loadSelectButton = Instantiate(loadSelectbuttonPrefab, loadSelectButtonTran, false);
            loadSelectButton.SetUpLoadSelectButton(item.Key, item.Value, this);
            loadSelectButtonList.Add(loadSelectButton);
        }
    }

    /// <summary>
    /// タップされていないロードボタンを重複して押せないように制御
    /// </summary>
    public void InactiveLoadSelectButtons() {
        for (int i = 0; i < loadSelectButtonList.Count; i++) {
            if (loadSelectButtonList[i].isClickable) {
                loadSelectButtonList[i].isClickable = true;
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
        Sequence sequence = DOTween.Sequence();

        sequence.Append(canvasGroup.DOFade(0, 1.0f));
        sequence.AppendInterval(1.0f);

        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    private void OnClickClosePopUp() {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(canvasGroup.DOFade(0, 1.0f)).AppendInterval(1.0f);

        Destroy(gameObject);
    }
}
