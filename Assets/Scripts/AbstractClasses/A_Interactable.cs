using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public abstract class A_Interactable : MonoBehaviour, I_Interactable, I_ControlsSwapUI
{
    [SerializeField] protected TextMeshProUGUI interactText;
    [SerializeField] protected GameObject interactPromptCanvas;
    [SerializeField] protected Image interactPrompt;
    [SerializeField] protected Image interactPrompt_Background;
    protected bool interactPromptShowing = false;
    [SerializeField] protected bool interactable;
    public bool Interactable{
        protected set{interactable = value;} 
        get {return interactable;}
    }
    public Interactable_Type InteractableType{
        protected set; get;
    } = Interactable_Type.Generic;
    Tween interactTextTween;
    Tween interactPromptTween;
    Tween interactPromptBackgroundTween;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // if(interactText != null){
        //     interactText.alpha = 0f;
        // }
        if(interactPromptCanvas != null){
            interactPromptCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        }

        interactPromptTween = interactPrompt.transform.DOLocalMoveY(interactPrompt.transform.localPosition.y + .2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        interactPromptBackgroundTween = interactPrompt_Background.transform.DOLocalMoveY(interactPrompt_Background.transform.localPosition.y + .2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        interactTextTween = interactText.transform.DOLocalMoveY(interactText.transform.localPosition.y + .2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public abstract bool Interact();
    public void DisplayInteractionPrompt(bool immediate = false){
        if(interactPromptShowing){
            return;
        }

        // if(immediate){
        //     if(useInteractionPrompt){
        //         interactPromptCanvas.GetComponent<CanvasGroup>().DOKill();
        //         interactPromptCanvas.GetComponent<CanvasGroup>().alpha = 1f;
        //     } else {
        //         interactText.DOKill();
        //         interactText.alpha = 1f;
        //     }
        // } else {
        //     if(useInteractionPrompt){
        //         interactPromptCanvas.GetComponent<CanvasGroup>().DOFade(1f, .75f);
        //     } else {
        //         interactText.DOFade(1f, .75f);
        //     }
        // }

        
        if(immediate){
            interactPromptCanvas.GetComponent<CanvasGroup>().DOKill();
            interactPromptCanvas.GetComponent<CanvasGroup>().alpha = 1f;
        } else {
            interactPromptCanvas.GetComponent<CanvasGroup>().DOFade(1f, .75f);
        }

        interactPromptShowing = true;
        // interactTextTween.TogglePause();
    }
    public void HideInteractionPrompt(bool immediate = false){
        if(!interactPromptShowing){
            return;
        }

        if(immediate){
            interactPromptCanvas.GetComponent<CanvasGroup>().DOKill();
            interactPromptCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        } else {
            interactPromptCanvas.GetComponent<CanvasGroup>().DOFade(0f, .75f);
        }

        interactPromptShowing = false;
        // interactTextTween.TogglePause();
    }

    // public void UpdateControlsUI(){
    //     interactPrompt.sprite = ControlsManager.Instance.GetCurrentInteractSprite();
    // }

    public void UpdateControlsUI(){
        interactPrompt_Background.sprite = ControlsManager.Instance.GetCurrentBackgroundSprite();

        if(ControlsManager.Instance.CurrentControlType == ControlType.KEYBOARD_MOUSE){
            interactPrompt.gameObject.SetActive(false);
            interactText.gameObject.SetActive(true);
        } else {
            interactPrompt.gameObject.SetActive(true);
            interactPrompt.sprite = ControlsManager.Instance.GetCurrentInteractSprite();
            interactText.gameObject.SetActive(false);
        }
    }
}
