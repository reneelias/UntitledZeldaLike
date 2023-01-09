using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;
using System;
using DG.Tweening;

public class OrbitingStaffParticle : OrbitingParticle
{
    private Light2D light2D;
    private Color flameColor;
    private float angleLerpSpeed;
    private bool aligning;
    public bool Aligning{
        get{return aligning;} 
        set{aligning = value;
            shouldRotate = !aligning;
            }
    }
    int index;
    float angleSeperation;
    GameObject player;
    float originalScale;
    float originalLightAlpha;
    CharacterControls characterControls;

    // Start is called before the first frame update
    protected override void Start()
    {
        // calculateOrbitAtStart = false;
        // base.Start();
        light2D = GetComponent<Light2D>();
        player = GameObject.FindWithTag("Player");
        flameColor = player.GetComponent<WizardControls>().flameColor;
        light2D.color = flameColor;
        flameColor.a = 1;
        GetComponent<SpriteRenderer>().color = flameColor;
        originalScale = transform.localScale.x;
        originalLightAlpha = light2D.color.a;
        characterControls = player.GetComponent<CharacterControls>();
    }

    public void initializeValues(float angle, float radius, float angularSpeed, GameObject orbitObject, bool shouldRotate, float angleLerpSpeed, int index, float angleSeperation){
        base.initializeValues(angle, radius, angularSpeed, orbitObject, shouldRotate);
       
        // Debug.Log("initializing values orbitingstaff");
        // this.shouldRotate = shouldRotate;
        this.angleLerpSpeed = angleLerpSpeed;
        this.index = index;
        this.angleSeperation = angleSeperation;
    }

    // Update is called once per frames
    protected override void Update()
    {
        base.Update();
        // UpdateLines();
        UpdateSortingLayer();
    }

    public void SetToNewAngle(float angle){
        //degrees
        this.angle = angle;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

   void UpdateSortingLayer(){
       if(characterControls.CharacterDirection == CharacterDirection.Down || characterControls.CharacterDirection == CharacterDirection.Right){
           gameObject.GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder + 2;
       } else {
           gameObject.GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder - 1;
       }       
   }

   public void ScaleTween(float scale, float time, bool shouldResetOnFinish = true){
       transform.DOScale(scale, time)
                .OnComplete(()=>{
                    if(shouldResetOnFinish){
                        transform.localScale = new Vector3(originalScale, originalScale, 0f);
                    }
                });
   }

    public override void TransitionToNewRadius(float newRadius, float transitionDuration, float delay = 0, bool deactivateAfterCompletion = false)
    {
        base.TransitionToNewRadius(newRadius, transitionDuration, delay, deactivateAfterCompletion);

        float targetAlpha = deactivateAfterCompletion ? 0f : originalLightAlpha;
        if(deactivateAfterCompletion){
            DOTween.To(()=> light2D.color.a, x=>light2D.color.a = x, targetAlpha, transitionDuration);
        }
    }
    // bool CheckForTouch(){
    //     if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
    //     {
    //         Vector3 wp;
    //         if(Input.GetMouseButtonDown(0)){
    //             wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         } else {
    //             wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
    //         }

    //         var touchPosition = new Vector2(wp.x, wp.y);
    //     }
    //     return false;
    // }
}
