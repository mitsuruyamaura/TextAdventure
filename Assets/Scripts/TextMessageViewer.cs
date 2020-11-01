using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.EventSystems;        // 追加

public class TextMessageViewer : MonoBehaviour {

    public string[] messages;　　                            // 表示するメッセージの配列
    public CHARA_NAME_TYPE[] charaTypes;
    public int[] branchs;
    public Dictionary<int, CHARA_NAME_TYPE[]> displayCharas;

    public int bgmNo;                                        // 追加。再生するBGMの設定番号
    public string[] branchMessages;                          // 追加。分岐用のメッセージの配列

    public float wordSpeed;                                  // 1文字当たりの表示速度

    public Text txtMessage;                                  // メッセージ表示用
    public Text txtCharaType;
    public Image imgBackground;
    public List<DisplayChara> displayCharasList;             // ImageだとEnebledを切っても位置が変わらない
    public Transform charaTran;

    public GameObject iconNextTap;             　            // タップを促す画像表示
    public GameDirector gameDirector;

    private int messagesIndex = 0;                           // 表示するメッセージの配列番号
    private int wordCount;                                   // １メッセージ当たりの文字の総数
    private bool isTapped = false;                      　   // 全文表示後にタップを待つフラグ
    private bool isDisplayedAllMessage = false;              // 全メッセージ表示完了のフラグ

    private IEnumerator waitCoroutine;                       // 全文表示までの待機時間メソッド代入用。Stopできるようにしておく
    private Tween tween;                                     // DoTween再生用。Killできるように代入して使用する

    public int autoScenerioNo;                               // 分岐がないパラグラフ(分岐番号 -1)の場合には、自動的にこの番号の分岐に移動する

    public List<int> conditionalBranchNo;                    // 条件付きの分岐がある場合、その条件となる番号を設定するリスト

    public int endingNo;

    public Button btnAutoPlay;

    public bool isAutoPlay;

    public Button btnSkip;

    public bool isSkipReadingMessage;

    public bool isReadBranchNo;

    public int currentBranchNo;

    public float skipSpeed = 0f;

    private float currentWordSpeed;

    public float autoPlayWaitTime = 1.0f;

    public Button btnSave;

    public Button btnLoad;

    public DataLoadPopUp dataLoadPopUpPrefab;

    public Transform canvasTran;

    private DataLoadPopUp dataLoadPopUp;

    private bool isSaving;


    void Start() {
        iconNextTap.SetActive(false);

        // Debug １文字ずつ文字を表示する処理をスタート
        //StartCoroutine(DisplayMessage());

        btnAutoPlay.onClick.AddListener(OnClickAutoPlay);
        btnSkip.onClick.AddListener(OnClickSkipReadingMessage);

        btnSave.onClick.AddListener(OnClickSave);
        btnLoad.onClick.AddListener(OnClickDataLoad);

        currentWordSpeed = wordSpeed;
    }

    /// <summary>
    /// シナリオのメッセージや分岐などを設定
    /// </summary>
    /// <param name="senarioData"></param>
    public void SetUpScenarioData(SenarioMasterData.SenarioData senarioData) {
        // シナリオの各データを準備
        messages = new string[senarioData.messages.Length];
        messages = senarioData.messages;

        charaTypes = new CHARA_NAME_TYPE[senarioData.charaTypes.Length];
        charaTypes = senarioData.charaTypes;

        branchs = new int[senarioData.branchs.Length];
        branchs = senarioData.branchs;

        displayCharas = new Dictionary<int, CHARA_NAME_TYPE[]>(senarioData.displayCharas);
  
        // 再生するBGMを設定
        bgmNo = senarioData.bgmNo;

        // 取得した番号のBGMを再生
        SoundManager.Instance.PlayBGM((SoundManager.BGM_Type)bgmNo);

        // 分岐用のメッセージを設定
        branchMessages = new string[senarioData.branchMessages.Length];
        branchMessages = senarioData.branchMessages;

        // 条件付きの分岐番号を設定
        conditionalBranchNo = new List<int>(senarioData.conditionalBranchNo);

        // 初期化
        messagesIndex = 0;
        isDisplayedAllMessage = false;

        // シナリオの背景を設定
        imgBackground.sprite = Resources.Load<Sprite>("BackGround/" + senarioData.backgroundImageNo);

        autoScenerioNo = senarioData.autoScenarioNo;

        if (senarioData.endingNo != 0) {
            endingNo = senarioData.endingNo;
        }

        // 未読/既読シナリオの判定を初期化(未読とする)
        isReadBranchNo = false;

        // 現在のシナリオの分岐番号を設定
        currentBranchNo = senarioData.senarioNo;

        // 既読のシナリオ分岐番号かどうか確認
        foreach (int readBranchNo in GameData.instance.readBranchNoList) {
            if (readBranchNo == currentBranchNo) {
                // このシナリオデータは既読状態
                isReadBranchNo = true;
            }
        }

        // 既読のシナリオではない場合には自動再生を停止して文字送りの速度を戻す
        if (!isReadBranchNo) {
            isAutoPlay = false;
            currentWordSpeed = wordSpeed;
        }

        // 既読スキップ
        SkipMessage();

        // セーブできるようにする
        isSaving = false;
        btnSave.interactable = true;

        // 1文字ずつメッセージ表示を開始
        StartCoroutine(DisplayMessage());
    }

    void Update() {
        if (isDisplayedAllMessage) {
            // 全てのメッセージ表示が終了していたら処理を行わない
            return;
        }

        if (Input.GetMouseButtonDown(0) && wordCount == messages[messagesIndex].Length) {

            // ボタンがクリックされていたら、画面クリックは無効にする
            if (EventSystem.current.currentSelectedGameObject != null) {
                return;
            }

            // 全文表示中にタップしたら全文表示を終了
            isTapped = true;
        }

        if (Input.GetMouseButtonDown(0) && tween != null) {

            // ボタンがクリックされていたら、画面クリックは無効にする
            if (EventSystem.current.currentSelectedGameObject != null){ 
                return; 
            }

            // 文字送り中にタップした場合、文字送りを停止
            tween.Kill();
            tween = null;
            // 文字送りのための待機時間も停止
            if (waitCoroutine != null) {
                StopCoroutine(waitCoroutine);
                waitCoroutine = null;
            }
            // 全文をまとめて表示
            txtMessage.text = messages[messagesIndex];

            // メッセージ表示完了処理
            CompleteOneMessage();

            // タップするまで全文を表示したまま待機
            StartCoroutine(NextTouch());
        }
    }

    /// <summary>
    /// １文字ずつ文字を表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayMessage() {
        isTapped = false;

        // 表示テキストとTweenをリセット
        txtMessage.text = "";
        txtCharaType.text = "";
        tween = null;

        // 文字送りの待機時間を初期化
        if (waitCoroutine != null) {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
        }

        // メッセージ表示中のキャラ名表示
        if (charaTypes[messagesIndex] != CHARA_NAME_TYPE.NO_NAME) {
            // NoNameの場合だけキャラ名表示
            txtCharaType.text = charaTypes[messagesIndex].ToString();
        }

        // 立ち絵表示するキャラを設定（メッセージ表示中のキャラだけはない）
        foreach (DisplayChara chara in displayCharasList) {
            chara.gameObject.SetActive(false);
            // 表示させるキャラの確認
            foreach (KeyValuePair<int, CHARA_NAME_TYPE[]> item in displayCharas) {
                // 何番目のメッセージか確認
                if (item.Key == messagesIndex) {
                    for (int i = 0; i < item.Value.Length; i++) {
                        // 該当するキャラか確認
                        if (item.Value[i] == chara.charaNameType) {
                            // 表示させる設定なら表示
                            chara.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        // 1文字ずつの文字送り表示が終了するまでループ
        while (messages[messagesIndex].Length > wordCount) {
            // wordSpeed秒ごとに、文字を1文字ずつ表示。SetEase(Ease.Linear)をセットすることで一定の表示間隔で表示
            tween = txtMessage.DOText(messages[messagesIndex], messages[messagesIndex].Length * currentWordSpeed).
                SetEase(Ease.Linear).OnComplete(() => {
                    Debug.Log("全文表示 完了");

                        // メッセージ表示完了処理
                        CompleteOneMessage();

                    if (isAutoPlay) {
                        StartCoroutine(NextTouch());
                        Debug.Log("オート再生中");
                    }
                });
            // 文字送り表示が終了するまでの待機時間を設定して待機を実行
            waitCoroutine = WaitTime();
            yield return StartCoroutine(waitCoroutine);
            break;
        }
    }

    /// <summary>
    /// 全文表示までの待機時間(文字数×1文字当たりの表示時間)
    /// タップして全文をまとめて表示した場合にはこの待機時間を停止
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitTime() {
        yield return new WaitForSeconds(messages[messagesIndex].Length * currentWordSpeed);
    }

    /// <summary>
    /// １つ分のメッセージ表示完了処理
    /// </summary>
    private void CompleteOneMessage() {
        // 表示した文字の総数を更新
        wordCount = messages[messagesIndex].Length;

        // タップを促すイメージ表示
        iconNextTap.SetActive(true);
    }

    /// <summary>
    /// タップするまで全文を表示したまま待機
    /// </summary>
    /// <returns></returns>
    private IEnumerator NextTouch() {
        if (!isAutoPlay) {
            // タップを待つ
            yield return new WaitUntil(() => isTapped);
            Debug.Log("非オート。タップ待ち");
        } else {
            // 自動再生中は１つのメッセージ全文を表示したら、指定した秒数待機
            yield return new WaitForSeconds(autoPlayWaitTime);
            Debug.Log("オート中 : " + autoPlayWaitTime + " 秒待機");
        }
        
        iconNextTap.SetActive(false);

        // 次のメッセージ準備
        messagesIndex++;
        wordCount = 0;

        // リストに未表示のメッセージが残っている場合
        if (messagesIndex < messages.Length) {
            // １文字ずつ文字を表示する処理をスタート
            StartCoroutine(DisplayMessage());
        } else {
            // 全メッセージ表示終了
            isDisplayedAllMessage = true;

            // 現在のシナリオの分岐番号を既読番号として保存
            GameData.instance.SaveReadBranchNo(currentBranchNo);

            // エンディングか確認
            if (JudgeEnding()) {
                // エンディングの場合の処理

                // 立ち絵キャラを非表示にする
                for (int i = 0; i < displayCharasList.Count; i++) {
                    displayCharasList[i].gameObject.SetActive(false);
                }

                // まだこのエンディングを見ていなければ(エンディングの番号リストになければ)、エンディングの番号を保存
                if (GameData.instance.endingNos.Contains(endingNo)) {
                    GameData.instance.SaveEndingData(endingNo);
                    Debug.Log("Ending SaveNo :" + endingNo);
                }

            } else {
                if (branchs[0] == -1) {
                    // 分岐なしの場合は自動的に次のシナリオを再生
                    // 次のシナリオの呼び出し
                    gameDirector.ChooseBranch(autoScenerioNo);

                } else {

                    // 分岐ボタンの作成
                    StartCoroutine(gameDirector.CreateBranchSelectButton(branchMessages, branchs, conditionalBranchNo));  // <=　引数変更
                }
            }
        }
    }

    private bool JudgeEnding() {
        // エンディングの条件によって分岐

        if (endingNo != 0) {
            // Trueならエンディング
            return true;
        }        

        return false;
    }


    // 追加

    /// <summary>
    /// 自動再生ボタン
    /// </summary>
    public void OnClickAutoPlay() {
        isAutoPlay = !isAutoPlay;

        if (isAutoPlay) {
            btnAutoPlay.image.color = btnAutoPlay.colors.pressedColor;
        } else {
            btnAutoPlay.image.color = btnAutoPlay.colors.normalColor;
        }
    }

    /// <summary>
    /// 既読スキップボタン
    /// </summary>
    public void OnClickSkipReadingMessage() {
        isSkipReadingMessage = !isSkipReadingMessage;

        if (isSkipReadingMessage) {
            btnSkip.image.color = btnSkip.colors.pressedColor;
        } else {
            btnSkip.image.color = btnSkip.colors.normalColor;
        }

        // 既読スキップ
        SkipMessage();
    }

    /// <summary>
    /// 既読メッセージのスキップを実行
    /// </summary>
    private void SkipMessage() {
        // 既読スキップ実行中で、かつ、既読のシナリオの場合には自動的にオート再生にしてスキップする
        if (isSkipReadingMessage && isReadBranchNo) {
            isAutoPlay = true;
            currentWordSpeed = skipSpeed;
            Debug.Log("既読スキップ中");
        }
    }


    // ここまで


    /// <summary>
    /// セーブ呼び出し
    /// </summary>
    private void OnClickSave() {
        
        // 一度だけセーブさせる
        if (isSaving) {
            return;
        }

        isSaving = true;
        btnSave.interactable = false;

        GameData.instance.Save(currentBranchNo);
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
