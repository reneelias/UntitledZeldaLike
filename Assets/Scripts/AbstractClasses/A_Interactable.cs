using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public abstract class A_Interactable : MonoBehaviour, I_Interactable
{
    [SerializeField] protected TextMeshProUGUI interactText;
    protected bool interactTextShowing = false;

    [SerializeField] protected bool interactable;
    public bool Interactable{
        protected set{interactable = value;} 
        get {return interactable;}
    }
    public Interactable_Type InteractableType{
        protected set; get;
    } = Interactable_Type.Generic;
    Tween interactTextTween;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        interactText.alpha = 0;
        interactTextTween = interactText.transform.DOLocalMoveY(interactText.transform.localPosition.y + .2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public abstract bool Interact();
    public void DisplayInteractionText(bool immediate = false){
        if(interactTextShowing){
            return;
        }
        if(immediate){
            interactText.DOKill();
            interactText.alpha = 1f;
        } else {
            interactText.DOFade(1f, .75f);
        }

        interactTextShowing = true;
        // interactTextTween.TogglePause();
    }
    public void HideInteractionText(bool immediate = false){
        if(!interactTextShowing){
            return;
        }

        if(immediate){
            interactText.DOKill();
            interactText.alpha = 0f;
        } else {
            interactText.DOFade(0f, .75f);
        }

        interactTextShowing = false;
        // interactTextTween.TogglePause();
    }    
}
