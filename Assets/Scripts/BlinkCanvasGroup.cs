using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class BlinkCanvasGroup : MonoBehaviour
{
    public float durationSeconds;
    public Ease easeType;

    public CanvasGroup canvasGroup;

    void Start() {
        canvasGroup.DOFade(0.0f, durationSeconds).SetEase(easeType).SetLoops(-1, LoopType.Yoyo);
    }
}
