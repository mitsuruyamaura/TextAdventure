using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Album : MonoBehaviour
{
    [SerializeField]
    private CGSelectButton cgSelectButtonPrefab;     // CG選択用ボタンのプレファブ

    [SerializeField]
    private Transform cgSelectButtonTran;            // CG選択用ボタンの生成位置

    [SerializeField]
    private Button btnReturnTitle;                   // タイトルへ戻るボタンの制御用

    [SerializeField]
    private Transform createCGTran;                  // CGの生成位置


    void Start()
    {
        // CGの総数分だけ、CG選択ボタンを作成する
        for (int i = 0; i < GameData.instance.cgTotalCount; i++) {
            CGSelectButton cgSelectButton = Instantiate(cgSelectButtonPrefab, cgSelectButtonTran, false);

            // CGの番号を渡してCG選択ボタンを設定
            cgSelectButton.SetUpCGSelectButton(i, createCGTran);
        }

        // タイトルへ戻るためのボタンにメソッドを登録
        btnReturnTitle.onClick.AddListener(OnClickReturnTitle);
    }

    /// <summary>
    /// タイトルシーンへ戻る
    /// </summary>
    private void OnClickReturnTitle() {

        // TODO 暗転などの演出を好みで入れる

        // Title シーンへ遷移
        SceneManager.LoadScene("Title");
    }
}
