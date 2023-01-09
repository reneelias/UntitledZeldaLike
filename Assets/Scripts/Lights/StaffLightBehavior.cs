using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StaffLightBehavior : MonoBehaviour
{
    float baseIntensity;
    UnityEngine.Rendering.Universal.Light2D lightComponent;
    // bool lightChanged = false;
    // private float elapsedDT = 0f;
    private float intensityChangeTime;
    public GameObject lightSprite;
    private float originalLightScale;
    private Color flameColor;
    // Random rand;

    // Start is called before the first frame update
    void Start()
    {
        lightComponent = GetComponent<UnityEngine.Rendering.Universal.Light2D>(); 
        baseIntensity = lightComponent.intensity;
        originalLightScale = lightSprite.transform.localScale.x;
        flameColor = GameObject.Find("OldWizard").GetComponent<WizardControls>().flameColor;
        flameColor.a = 1;
        lightComponent.color = flameColor;
        // GetComponent<SpriteRenderer>().color = flameColor;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
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
}
