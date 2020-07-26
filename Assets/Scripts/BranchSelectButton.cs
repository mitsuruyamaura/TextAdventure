using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class BranchSelectButton : MonoBehaviour
{
    public Text txtBranchMessage;
    public Button btnBranchSelectButton;
    public CanvasGroup canvasGroup;

    public int branchNo;   
    public bool isClickable;
    public Ease easeType;

    private GameDirector gameDirector;
    private Sequence sequence;

    /// <summary>
    /// 分岐ボタンの初期設定
    /// </summary>
    /// <param name="message">分岐のメッセージ</param>
    /// <param name="no">分岐の番号</param>
    /// <param name="director">GameDirector</param>
    public void InitializeBranchSelect(string message, int no, GameDirector director, int count) {
        // 位置の調整
        canvasGroup.alpha = 0.0f;
        transform.position = new Vector3(transform.position.x, transform.position.y - (count * 150), transform.position.z);

        // 透明な状態で画面左端から中央にアニメ移動して表示
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOLocalMoveX(2000, 1.0f).SetEase(easeType));
        sequence.Join(canvasGroup.DOFade(1.0f, 1.0f));

        // 初期設定
        txtBranchMessage.text = message;
        branchNo = no;
        gameDirector = director;

        // ボタン設定
        btnBranchSelectButton.onClick.AddListener(OnClickChooseBranch);
    }

    /// <summary>
    /// 分岐ボタンを押すと呼ばれる
    /// 選択した分岐の番号をGameDirectorへ渡す
    /// </summary>
    private void OnClickChooseBranch() {
        if (isClickable) {
            return;
        }
        isClickable = true;

        // 他の分岐ボタンを押せなくする
        gameDirector.InactiveBranchSelectButtons();

        // 画面中央から画面右端にアニメ移動し、徐々に透明化
        sequence = DOTween.Sequence();

        sequence.Append(transform.DOLocalMoveX(4000, 1.0f).SetEase(easeType))
            .Join(canvasGroup.DOFade(0.0f, 1.0f))
            .AppendCallback(() => {
                Debug.Log("移動終了");
                // 選択した分岐の番号を渡して次のシナリオを作る
                gameDirector.ChooseBranch(branchNo);
            }
        );
    }
}
