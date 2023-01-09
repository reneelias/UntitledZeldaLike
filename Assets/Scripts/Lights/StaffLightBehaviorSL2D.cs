using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;

public class StaffLightBehaviorSL2D : MonoBehaviour
{
    float baseIntensity;
    Light2D lightComponent;
    // bool lightChanged = false;
    // private float elapsedDT = 0f;
    private float intensityChangeTime;
    public GameObject lightSprite;
    private float originalLightScale;
    private Color flameColor;
    [SerializeField] float baseCharacterScale = 1.39f;
    // Random rand;

    // Start is called before the first frame update
    void Start()
    {
        lightComponent = GetComponent<Light2D>(); 
        originalLightScale = lightSprite.transform.localScale.x;
        flameColor = GameObject.Find("OldWizard").GetComponent<WizardControls>().flameColor;
        
        flameColor.a = 1f;
        baseIntensity = flameColor.a;
        lightComponent.color = flameColor;

        lightComponent.size *= transform.parent.localScale.x / baseCharacterScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        UpdatePosition();
    }

    void UpdatePosition(){
        transform.position = lightSprite.transform.position;
    }


     // Debug.Log(Random.value);
        // if(!lightChanged && Random.value > .5){
        //     var randomModifier = Random.value;
        //     lightComponent.intensity = 1f + randomModifier * .66f;
        //     lightChanged = true;
        //     intensityChangeTime = Random.value *.35f;

        //     var scaleRange = .125f;
        //     var newScale = originalLightScale + scaleRange * (randomModifier - .5f);
        //     lightSprite.transform.localScale = new Vector3(newScale, newScale, 1); 
        // } else {
        //     elapsedDT += Time.deltaTime;

        //     if(elapsedDT >= intensityChangeTime){
        //         elapsedDT = 0;
        //         lightChanged = false;
        //         lightComponent.intensity = baseIntensity;
        //         lightSprite.transform.localScale = new Vector3(originalLightScale, originalLightScale, originalLightScale);
        //     }
        // }
}
