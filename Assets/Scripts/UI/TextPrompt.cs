using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TextPrompt : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textObject;
    [SerializeField] Image backgroundImage;
    // Start is called before the first frame update
    Vector3 originalBackgroundPosition;
    Vector3 originalTextPosition;
    [SerializeField] bool active = false;
    public bool Active{
        get;
        protected set;
    }
    [SerializeField] float easeOvershootOrAmplitude = 1.25f;
    [SerializeField] Image buttonImage;
    [SerializeField] TextMeshProUGUI buttonTextObject;
    Tween buttonTween;
    Tween buttonTextTween;
    [SerializeField] float buttonTweenDuration = 1f;
    [SerializeField] float buttonTweenScaler = 1.2f;
    float originalTextFontSize;
    float originalButtonTexFontSize;
    Vector3 originalButtonScale;
    List<(GameObject character, string text)> dialogueTupleList;
    int textIndex = 0;
    void Start()
    {
        originalTextPosition = textObject.transform.position;
        originalBackgroundPosition = backgroundImage.transform.position;
        originalTextFontSize = textObject.fontSize;
        originalButtonTexFontSize = buttonTextObject.fontSize;
        originalButtonScale = buttonImage.transform.localScale;

        if(!active){
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public void ShowPrompt(string text, float scaling = 1f, float fadeDuration = .75f){
    //     gameObject.SetActive(true);
    //     Color newColor = backgroundImage.color;
    //     newColor.a = 0f;
    //     backgroundImage.color = newColor;
    //     backgroundImage.DOFade(1f, fadeDuration);
    //     textObject.alpha = 0f;
    //     textObject.DOFade(1f, fadeDuration);
    //     textObject.fontSize = originalTextFontSize * scaling;

    //     textObject.transform.DOMoveY(textObject.transform.position.y - backgroundImage.rectTransform.rect.height / 10f, fadeDuration)
    //         .From()
    //         .SetEase(Ease.OutBack).easeOvershootOrAmplitude = easeOvershootOrAmplitude;
    //     backgroundImage.transform.DOMoveY(backgroundImage.transform.position.y - backgroundImage.rectTransform.rect.height / 10f, fadeDuration)
    //         .From()
    //         .SetEase(Ease.OutBack)
    //         .easeOvershootOrAmplitude = easeOvershootOrAmplitude;

    //     SetText(text);

    //     buttonImage.transform.localScale = originalButtonScale * scaling;
    //     buttonTextObject.fontSize = originalButtonTexFontSize * scaling;
    //     buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);
    //     buttonTextObject.alpha = 0f;
    //     buttonImage.DOFade(1f, fadeDuration);
    //     buttonTextObject.DOFade(1f, fadeDuration);

    //     if(buttonTween == null){

    //         buttonTween = buttonImage.transform.DOScale(originalButtonScale * scaling * buttonTweenScaler, buttonTweenDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    //         buttonTextTween = DOTween.To(()=> buttonTextObject.fontSize, x => buttonTextObject.fontSize = x, originalButtonTexFontSize * buttonTweenScaler, buttonTweenDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    //         // buttonTextTween = buttonTextObject.transform.DOScale(1.25f, 1f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
    //     }

    //     buttonTween.Restart(false);
    //     buttonTextTween.Restart(false);
    // }

    public void ShowPrompt(List<(GameObject character, string text)> dialogueTupleList, float scaling = 1f, float fadeDuration = .75f){
        gameObject.SetActive(true);
        Color newColor = backgroundImage.color;
        newColor.a = 0f;
        backgroundImage.color = newColor;
        backgroundImage.DOFade(1f, fadeDuration);
        textObject.alpha = 0f;
        textObject.DOFade(1f, fadeDuration);
        textObject.fontSize = originalTextFontSize * scaling;

        textObject.transform.DOMoveY(textObject.transform.position.y - backgroundImage.rectTransform.rect.height / 10f, fadeDuration)
            .From()
            .SetEase(Ease.OutBack).easeOvershootOrAmplitude = easeOvershootOrAmplitude;
        backgroundImage.transform.DOMoveY(backgroundImage.transform.position.y - backgroundImage.rectTransform.rect.height / 10f, fadeDuration)
            .From()
            .SetEase(Ease.OutBack)
            .easeOvershootOrAmplitude = easeOvershootOrAmplitude;

        textIndex = 0;
        SetText(dialogueTupleList);

        buttonImage.transform.localScale = originalButtonScale * scaling;
        buttonTextObject.fontSize = originalButtonTexFontSize * scaling;
        buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, 0f);
        buttonTextObject.alpha = 0f;
        buttonImage.DOFade(1f, fadeDuration);
        buttonTextObject.DOFade(1f, fadeDuration);

        if(buttonTween == null){

            buttonTween = buttonImage.transform.DOScale(originalButtonScale * scaling * buttonTweenScaler, buttonTweenDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
            buttonTextTween = DOTween.To(()=> buttonTextObject.fontSize, x => buttonTextObject.fontSize = x, originalButtonTexFontSize * buttonTweenScaler, buttonTweenDuration).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
            // buttonTextTween = buttonTextObject.transform.DOScale(1.25f, 1f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        }

        buttonTween.Restart(false);
        buttonTextTween.Restart(false);
        Active = true;
    }
    public void HidePrompt(float fadeDuration = .75f){
        backgroundImage.DOFade(0f, fadeDuration);
        textObject.DOFade(0f, fadeDuration);
        backgroundImage.transform.DOMoveY(backgroundImage.transform.position.y - backgroundImage.rectTransform.rect.height / 10f, fadeDuration)
            .SetEase(Ease.InBack)
            .OnComplete(()=>{
                backgroundImage.transform.position = originalBackgroundPosition;
            }).easeOvershootOrAmplitude = easeOvershootOrAmplitude;

        textObject.transform.DOMoveY(textObject.transform.position.y - backgroundImage.rectTransform.rect.height / 10f, fadeDuration)
            .SetEase(Ease.InBack)
            .OnComplete(()=>{
                textObject.transform.position = originalTextPosition;
                gameObject.SetActive(false);
            })
            .easeOvershootOrAmplitude = easeOvershootOrAmplitude;
        buttonImage.DOFade(0f, fadeDuration / 2f);
        buttonTextObject.DOFade(0f, fadeDuration / 2f);
        Active = false;
    }

    public void SetText(List<(GameObject character, string text)> dialogueTupleList){
        this.dialogueTupleList = dialogueTupleList;
        textObject.text = dialogueTupleList[textIndex].text;
    }

    public bool NextText(){
        if(++textIndex >= dialogueTupleList.Count){
            return false;
        }

        textObject.text = dialogueTupleList[textIndex].text;

        return true;
    }
}
