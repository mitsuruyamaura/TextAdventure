using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title : MonoBehaviour 
{
    public bool isNewGameButton;

    public Button btnNewGame;

    public void LoadMain() {
        SceneManager.LoadScene("Game");
    }

    void Start() {
        // すべてのエンディングを見ているか確認。見ている場合には true 見ていない場合には false 
        if (GameData.instance.endingCount > 0) {
            isNewGameButton = GameData.instance.LoadCheckEndingData();
        }

        // すべてのエンディングを見ている場合、新しいボタンを表示
        if (isNewGameButton == true) {
            btnNewGame.gameObject.SetActive(true);
        }
    }

    public void OnClickNewGameButton() {
        // 新しいゲーム、エンディングのスタート内容を記述

    }
}

