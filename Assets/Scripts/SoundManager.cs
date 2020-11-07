using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 音管理クラス
/// </summary>
public class SoundManager : MonoBehaviour {

    protected static SoundManager instance;

    public static SoundManager Instance {
        get {
            if (instance == null) {
                instance = (SoundManager)FindObjectOfType(typeof(SoundManager));

                if (instance == null) {
                    Debug.LogError("SoundManager Instance Error");
                }
            }
            return instance;
        }
    }

    // BGM管理(適宜追加する)
    public enum BGM_Type : int {
        TITLE,
        MAIN,
        NO_MUSIC = 999,
    }

    // 効果音管理(適宜追加する)
    public enum SE_Type : int {
        OK,
        NO
    }

    // クロスフェード時間
    public const float CROSS_FADE_TIME = 1.5f;

    // === AudioSources ===
    // BGM
    private AudioSource[] BGMsources = new AudioSource[2];
    // SE
    private AudioSource[] SEsources = new AudioSource[16];

    // === AudioClip ===
    // BGM
    public AudioClip[] BGM;
    // SE
    public AudioClip[] SE;

    // SE用AudioMixer(未使用)
    //public AudioMixer audioMixer;

    bool isCrossFading;

    int currentBgmIndex = -1;

    void Awake() {
        GameObject[] obj = GameObject.FindGameObjectsWithTag("SoundManager");

        if (obj.Length > 1) {
            // 既に存在しているなら削除
            Destroy(gameObject);
        } else {
            // 音管理はシーン遷移では破棄させない
            DontDestroyOnLoad(gameObject);
        }

        // BGM AudioSource
        BGMsources[0] = gameObject.AddComponent<AudioSource>();
        BGMsources[1] = gameObject.AddComponent<AudioSource>();

        // SE AudioSource
        for (int i = 0; i < SEsources.Length; i++) {
            SEsources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="bgmNo"></param>
    /// <param name="loopFlg"></param>
    public void PlayBGM(BGM_Type bgmNo, bool loopFlg = true) {
        // BGMなしの場合
        if ((int)bgmNo == 999) {
            StopAllBGM();
            return;
        }

        int index = (int)bgmNo;
        currentBgmIndex = index;

        if (index < 0 || BGM.Length <= index) {
            return;
        }

        // 同じBGMの場合は何もしない
        if (BGMsources[0].clip != null && BGMsources[0].clip == BGM[index]) {
            return;
        } else if (BGMsources[1].clip != null && BGMsources[1].clip == BGM[index]) {
            return;
        }

        // フェードでBGM開始
        if (BGMsources[0].clip == null && BGMsources[1].clip == null) {
            BGMsources[0].loop = loopFlg;
            BGMsources[0].clip = BGM[index];
            BGMsources[0].Play();
            BGMsources[0].DOFade(GameData.instance.BGM_Volume, CROSS_FADE_TIME);
        } else {
            // クロスフェード
            StartCoroutine(CrossfadeChangeBMG(index, loopFlg));
        }
    }

    /// <summary>
    /// クロスフェード再生
    /// </summary>
    /// <param name="index"></param>
    /// <param name="loopFlg"></param>
    /// <returns></returns>
    private IEnumerator CrossfadeChangeBMG(int index, bool loopFlg) {
        isCrossFading = true;

        if (BGMsources[0].clip != null) {
            // 0がなっていて、1を新しい曲としてPlay
            BGMsources[1].volume = 0;
            BGMsources[1].clip = BGM[index];
            BGMsources[1].loop = loopFlg;
            BGMsources[1].Play();
            BGMsources[0].DOFade(0, CROSS_FADE_TIME).SetEase(Ease.Linear);
            BGMsources[1].DOFade(GameData.instance.BGM_Volume, CROSS_FADE_TIME).SetEase(Ease.Linear);
            yield return new WaitForSeconds(CROSS_FADE_TIME);
            BGMsources[0].Stop();
            BGMsources[0].clip = null;
        } else {
            // 1がなっていて、0を新しい曲としてPlay
            BGMsources[0].volume = 0;
            BGMsources[0].clip = BGM[index];
            BGMsources[0].loop = loopFlg;
            BGMsources[0].Play();
            BGMsources[1].DOFade(0, CROSS_FADE_TIME).SetEase(Ease.Linear);
            BGMsources[0].DOFade(GameData.instance.BGM_Volume, CROSS_FADE_TIME).SetEase(Ease.Linear);
            yield return new WaitForSeconds(CROSS_FADE_TIME);
            BGMsources[1].Stop();
            BGMsources[1].clip = null;
        }
        isCrossFading = false;
    }

    /// <summary>
    /// BGM停止
    /// </summary>
    public void StopAllBGM() {
        BGMsources[0].Stop();
        BGMsources[1].Stop();
        BGMsources[0].clip = null;
        BGMsources[1].clip = null;
    }

    /// <summary>
    /// SE再生
    /// </summary>
    /// <param name="seNo"></param>
    public void PlaySE(SE_Type seNo) {
        int index = (int)seNo;
        if (0 > index || SE.Length <= index) {
            return;
        }

        // 再生中で無いAudioSouceで鳴らす
        foreach (AudioSource source in SEsources) {
            if (false == source.isPlaying) {
                source.clip = SE[index];
                source.Play();
                return;
            }
        }
    }

    /// <summary>
    /// SE停止
    /// </summary>
    public void StopAllSE() {
        // 全てのSE用のAudioSouceを停止する
        foreach (AudioSource source in SEsources) {
            source.Stop();
            source.clip = null;
        }
    }
}
