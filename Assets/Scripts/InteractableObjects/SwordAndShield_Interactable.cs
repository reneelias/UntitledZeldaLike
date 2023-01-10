using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using FunkyCode;

public class SwordAndShield_Interactable : A_Interactable, I_DiscoverableItem
{
    [SerializeField] ParticleEmitter particleEmitter;

    [TextArea]
    [SerializeField] protected string[] discoverMessages;
    public string[] DiscoverMessages{
        protected set{ discoverMessages = value;}
        get{return discoverMessages;}
    }
    Sequence bounceSequence;
    Vector3 originPosition;
    [SerializeField] float fontScaling = 1f;
    public float FontScaling{
        protected set{fontScaling = value;}
        get {return fontScaling;}
    }
    [SerializeField] GameObject spriteObject;
    public GameObject SpriteObject{
        protected set {spriteObject = value;}
        get {return spriteObject;}
    }
    [SerializeField] GameObject swordSprite;
    [SerializeField] GameObject sheildSprite;
    [SerializeField] LightSprite2D radialLight;
    LightCollider2D lightCollider2D;
    [SerializeField] AudioClip pickupSound;
    // Start is called before the first frame update
    protected override void Start()
    {
        InteractableType = Interactable_Type.Sword;
        FontScaling = fontScaling;
        // lightCollider2D = spriteObject.GetComponent<LightCollider2D>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        UpdateLight();   
    }
    
    void UpdateLight(){
        if(!radialLight.enabled){
            return;
        }

        radialLight.lightSpriteTransform.rotation += 1f;
    }

    public override bool Interact(){
        if(!Interactable){
            return false;
        }

        // radialLight.enabled = true;
        HideInteractionPrompt(true);
        Interactable = false;
        swordSprite.SetActive(false);
        sheildSprite.SetActive(false);
        // lightCollider2D.enabled = false;
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
                Interactable = true;
                })
            .Pause();
    }

    public void Discover(){
        particleEmitter.activelySpawning = true;
        radialLight.enabled = true;
        GameMaster.Instance.audioSource.PlayOneShot(pickupSound);
        spriteObject.SetActive(true);
        // lightCollider2D.enabled = false;
    }

    public void Finish(){
        particleEmitter.activelySpawning = false;
    }
}
