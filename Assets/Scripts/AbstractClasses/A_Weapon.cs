using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_Weapon : MonoBehaviour, I_Weapon
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject downPosition;
    [SerializeField] GameObject upPosition;
    [SerializeField] GameObject rightPosition;
    [SerializeField] GameObject leftPosition;
    [SerializeField] GameObject character;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    CharacterControls characterControls;
    bool flipped = false;
    public bool Active{
        protected set; get;
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        characterControls = character.GetComponent<CharacterControls>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
    
    protected void UpdatePosition(){
        // switch(characterControls.CharacterDirection){
        //     case CharacterDirection.Right:
        //         transform.position = rightPosition.transform.position;
        //         break;
        //     case CharacterDirection.Down:
        //         transform.position = downPosition.transform.position;
        //         break;
        //     case CharacterDirection.Up:
        //         transform.position = upPosition.transform.position;
        //         break;
        //     case CharacterDirection.Left:
        //         transform.position = leftPosition.transform.position;
        //         break;
        // }
        switch(characterControls.CharacterDirection){
            case CharacterDirection.Right:
                transform.localPosition = rightPosition.transform.localPosition;
                break;
            case CharacterDirection.Down:
                transform.localPosition = downPosition.transform.localPosition;
                break;
            case CharacterDirection.Up:
                transform.localPosition = upPosition.transform.localPosition;
                break;
            case CharacterDirection.Left:
                transform.localPosition = leftPosition.transform.localPosition;
                break;
        }
    }

    public void Flip(bool flip){
        flipped = flip;
    }
    
    public void Appear(){
        GetComponent<SpriteRenderer>().enabled = false;
        animator.SetBool("Idle", false);
        animator.SetTrigger("Appear");
    }
    protected void EnableRenderer(){
        GetComponent<SpriteRenderer>().enabled = true;
    }
    protected void FinishAppear(){
        animator.SetBool("Idle", true);
        character.GetComponent<CharacterControls>().CompleteWeaponSwitch();
    }   
    public void Disappear(){
        animator.SetBool("Idle", false);
        animator.SetTrigger("Disappear");
    }
    protected void FinishDisappear(){
        gameObject.SetActive(false);
    }

    protected virtual void UpdateLayerOrder(){
        switch(characterControls.CharacterDirection){
            case CharacterDirection.Right:
                spriteRenderer.sortingOrder = character.GetComponent<SpriteRenderer>().sortingOrder - 1;
                break;
            case CharacterDirection.Down:
                spriteRenderer.sortingOrder = character.GetComponent<SpriteRenderer>().sortingOrder + 1;
                break;
            case CharacterDirection.Up:
                spriteRenderer.sortingOrder = character.GetComponent<SpriteRenderer>().sortingOrder - 1;
                break;
            case CharacterDirection.Left:
                spriteRenderer.sortingOrder = character.GetComponent<SpriteRenderer>().sortingOrder + 1;
                break;
        }
    }
}
