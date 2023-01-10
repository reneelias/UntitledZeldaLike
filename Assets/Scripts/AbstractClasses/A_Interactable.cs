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
    [SerializeField] bool useInteractionPrompt = true;
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
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        if(interactText != null){
            interactText.alpha = 0f;
        }
        if(interactPromptCanvas != null){
            interactPromptCanvas.GetComponent<CanvasGroup>().alpha = 0f;
        }

        if(useInteractionPrompt){
            interactPromptTween = interactPrompt.transform.DOLocalMoveY(interactText.transform.localPosition.y + .2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        } else {
            interactTextTween = interactText.transform.DOLocalMoveY(interactText.transform.localPosition.y + .2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }
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

        if(immediate){
            if(useInteractionPrompt){
                interactPromptCanvas.GetComponent<CanvasGroup>().DOKill();
                interactPromptCanvas.GetComponent<CanvasGroup>().alpha = 1f;
            } else {
                interactText.DOKill();
                interactText.alpha = 1f;
            }
        } else {
            if(useInteractionPrompt){
                interactPromptCanvas.GetComponent<CanvasGroup>().DOFade(1f, .75f);
            } else {
                interactText.DOFade(1f, .75f);
            }
        }

        interactPromptShowing = true;
        // interactTextTween.TogglePause();
    }
    public void HideInteractionPrompt(bool immediate = false){
        if(!interactPromptShowing){
            return;
        }

        if(immediate){
            if(useInteractionPrompt){
                interactPromptCanvas.GetComponent<CanvasGroup>().DOKill();
                interactPromptCanvas.GetComponent<CanvasGroup>().alpha = 0f;
            } else {
                interactText.DOKill();
                interactText.alpha = 0f;
            }
        } else {
            if(useInteractionPrompt){
                interactPromptCanvas.GetComponent<CanvasGroup>().DOFade(0f, .75f);
            } else {
                interactText.DOFade(0f, .75f);
            }
            
        }

        interactPromptShowing = false;
        // interactTextTween.TogglePause();
    }

    public void UpdateControlsUI(){
        interactPrompt.sprite = ControlsManager.Instance.GetCurrentInteractSprite();
    }
}
