using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGSelectButton : MonoBehaviour
{
    [SerializeField]
    private Button btnSelectCG;                      // CG選択ボタンの制御用

    [SerializeField]
    private Image imgThumbnail;                      // CG画像のサムネイル表示制御用

    [SerializeField]
    private CGController cgControllerPrefab;         // CG用ゲームオブジェクトのプレファブ

    private Transform createCGTran;                  // CG用ゲームオブジェクトの生成位置

    /// <summary>
    /// CG選択ボタンの設定
    /// </summary>
    /// <param name="cgNo"></param>
    /// <param name="createCGTran"></param>
    public void SetUpCGSelectButton(int cgNo, Transform createCGTran) {
        // 最初にフレーム画像に設定する
        imgThumbnail.sprite = Resources.Load<Sprite>("CG/frame");

        // 回収しているCGがある場合
        for (int i = 0; i < GameData.instance.getCGNos.Count; i++) {

            // このCGが回収済みのCGであるか判定
            if (GameData.instance.getCGNos[i] == cgNo) {

                // 回収している場合、ボタンの画像として、CGのサムネイルを設定
                imgThumbnail.sprite = Resources.Load<Sprite>("CG/cg_" + cgNo);
                break;
            } 
        }

        // CGの生成用の位置を取得
        this.createCGTran = createCGTran;

        // ボタンにメソッドを登録
        btnSelectCG.onClick.AddListener(OnClickCreateCG);
    }

    /// <summary>
    /// CGを生成
    /// </summary>
    private void OnClickCreateCG() {
        // 画面にCG表示用のオブジェクトを生成(CG設定なしの状態)
        CGController cg = Instantiate(cgControllerPrefab, createCGTran, false);

        // CGの画像やアニメを設定・実行
        cg.SetUpCG(imgThumbnail.sprite);
    }
}
