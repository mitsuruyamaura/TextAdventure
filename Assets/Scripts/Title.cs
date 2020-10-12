using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour 
{
    public bool isNewGameButton;

    public Button btnNewGame;

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
        // 
        if (GameData.instance.endingCount > 0) {
            CheckEndingCount();
        }
    }

    /// <summary>
    /// エンディング・コンプリート時に追加されるボタンに登録する処理
    /// </summary>
    public void OnClickNewGameButton() {
        // 新しいゲーム、エンディングのスタート内容を記述

    }
}

