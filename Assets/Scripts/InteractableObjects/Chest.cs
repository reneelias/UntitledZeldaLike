using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Chest : A_Interactable, IUnlocklableObject
{
    [SerializeField] bool unlocked = false;
    Sequence bounceSequence;
    Sequence scaleSequence;
    [SerializeField] bool openable = false;
    bool opened = false;
    [SerializeField] GameObject containedItem;
    public GameObject ContainedItem{
        get{ return containedItem;}
    }
    [SerializeField] Collider2D collider2D;
    Vector3 originPosition;
    [SerializeField] bool unlockDebug = false;
    [SerializeField] float unlockDebugTime = 1f;
    [SerializeField] GameObject closedChestObject;
    [SerializeField] GameObject openChestObject;
    float unlockDebugDT = 0f;
    [SerializeField] AudioClip chestOpenSound;
    [SerializeField] AudioClip itemGetSound;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        InitializeBounceAnim();

        if(!unlocked && !unlockDebug){
            gameObject.SetActive(false);
        }

        if(unlockDebug){
            transform.localScale = Vector3.zero;
        }

        InteractableType = Interactable_Type.Chest;
        Interactable = openable;
        
        // interactText.alpha = 0;
        // interactTextTween = interactText.transform.DOLocalMoveY(interactText.transform.localPosition.y + .2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        // interactTextTween.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        UpdateUnlockDebug();
    }

    void UpdateUnlockDebug(){
        if(!unlockDebug || unlocked){
            return;
        }

        unlockDebugDT += Time.deltaTime;

        if(unlockDebugDT >= unlockDebugTime){
            transform.localScale = new Vector3(1f, 1f, 1f);
            Unlock();
        }
    }

    public void Unlock(){
        gameObject.SetActive(true);
        unlocked = true;
        
        float originalScale = transform.localScale.x;
        // transform.localScale = new Vector3(originalScale * 1.5f, originalScale * 1.5f, 1f);
        bounceSequence.Restart();
        // scaleSequence.Restart();
    }

    public void Open(){
        closedChestObject.SetActive(false);
        openChestObject.SetActive(true);
        Interactable = false;
        HideInteractionText(true);
        opened = true;
        GameMaster.Instance.audioSource.PlayOneShot(chestOpenSound);
        DOVirtual.DelayedCall(.25f, ()=>{GameMaster.Instance.audioSource.PlayOneShot(itemGetSound);});
    }

    public override bool Interact(){
        if(!openable || opened){
            return false;
        }

        Open();
        return true;
    }

    void InitializeBounceAnim(){
        float originalScale = transform.localScale.x;
        originPosition = transform.position;
        Vector3 bounceStrengthVector = new Vector3(0f, .5f, 0f);
        float bounceDiffVectorAngle = Mathf.Atan2(bounceStrengthVector.y, bounceStrengthVector.x);
        bounceDiffVectorAngle += Mathf.Deg2Rad * transform.localEulerAngles.z;
        bounceStrengthVector = new Vector3(Mathf.Cos(bounceDiffVectorAngle), Mathf.Sin(bounceDiffVectorAngle), 0f) * bounceStrengthVector.magnitude;

        Vector3 rotationStrengthVector = new Vector3(0f, 0f, 10f);
        // transform.position = originPosition + bounceDiffVector;

        float bounceTime = .5f;
        bounceSequence = DOTween.Sequence()
            .Append(transform.DOPunchPosition(bounceStrengthVector, bounceTime, 10, 0f))
            .Join(transform.DOShakeRotation(bounceTime, rotationStrengthVector, 10, 45f, false))
            .OnComplete(()=>{
                openable = true;
                Interactable = true;
                })
            .Pause();
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // if(other.gameObject.tag == "Player" && openable){
        //     Open();
        // }
    }
}
