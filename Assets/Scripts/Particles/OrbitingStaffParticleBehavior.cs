using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class OrbitingStaffParticleBehavior : OrbitingParticle
{
    private UnityEngine.Rendering.Universal.Light2D lightComp;
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

    // Start is called before the first frame update
    protected override void Start()
    {
        // calculateOrbitAtStart = false;
        // base.Start();
        lightComp = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        flameColor = GameObject.Find("OldWizard").GetComponent<WizardControls>().flameColor;
        lightComp.color = flameColor;
        flameColor.a = 1;
        GetComponent<SpriteRenderer>().color = flameColor;
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
       gameObject.GetComponent<SpriteRenderer>().sortingOrder = GameObject.Find("OldWizard").GetComponent<SpriteRenderer>().sortingOrder + 1;
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
