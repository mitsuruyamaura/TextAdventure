using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CGController : MonoBehaviour
{
    [SerializeField]
    private Image imgCG;          // CGの画像の制御用

    [SerializeField]
    private Button btnCloseCG;    // CG全体をボタンにしてあり、その制御用

    /// <summary>
    /// 選択したサムネイルの画像をCGとして設定
    /// </summary>
    /// <param name="spriteCG"></param>
    public void SetUpCG(Sprite spriteCG) {

        // CGの画像をCG選択ボタンのサムネイル画像と同じ画像に設定
        imgCG.sprite = spriteCG;

        // 最初の大きさを保持
        Vector3 scale = transform.localScale;

        // 最小化する
        transform.localScale = Vector3.zero;

        // 大きくアニメ表示
        imgCG.transform.DOScale(scale, 1.0f).SetEase(Ease.Linear);

        // ボタンにメソッドを登録
        btnCloseCG.onClick.AddListener(OnClickCloseCG);
    }

    /// <summary>
    /// CGを破棄。CGがボタンになっているので、CGをクリックすると呼び出される
    /// </summary>
    private void OnClickCloseCG() {

        // 小さくアニメ表示して、見えなくなったら破棄
        imgCG.transform.DOScale(Vector3.zero, 1.0f).SetEase(Ease.Linear).OnComplete(() => { Destroy(gameObject); });
    }
}
