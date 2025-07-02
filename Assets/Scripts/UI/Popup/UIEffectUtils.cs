using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public static class UIEffectUtils 
{
    public static void FadeEffect(Image obj, float val, float time){
        obj.DOFade(val, time);
    }
    public static void FadeEffect(TextMeshProUGUI obj, float val, float time){
        obj.DOFade(val, time).SetEase(Ease.OutBack);
    }
    
    public static void ScaleEffect(RectTransform obj, Vector3 endVal, float time){
        obj.DOScale(endVal, time);
    }
    public static void MoveYEffect(RectTransform obj, float startPos, float endPos, float time){
        obj.DOAnchorPosY(startPos, 0);
        obj.DOAnchorPosY(endPos, time);
    }
    public static void MoveXEffect(RectTransform obj, float startPos, float endPos, float time){
        obj.DOAnchorPosX(startPos, 0);
        obj.DOAnchorPosX(endPos, time);
    }
    public static void MoveYEffect(RectTransform obj, float endPos, float time){
        obj.DOAnchorPosY(endPos, time);
    }
    public static void MoveXEffect(RectTransform obj, float endPos, float time){
        obj.DOAnchorPosX(endPos, time);
    }
    public static void MoveYWordEffect(RectTransform obj, float startPos, float endPos, float time){
        obj.DOMoveY(startPos, 0);
        obj.DOMoveY(endPos, time);
    }
    public static void MoveXWordEffect(RectTransform obj, float startPos, float endPos, float time){
        obj.DOMoveX(startPos, 0);
        obj.DOMoveX(endPos, time);
    }
}