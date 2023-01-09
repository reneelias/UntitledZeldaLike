using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlickeringLightBehavior : MonoBehaviour
{
   float baseIntensity;
    UnityEngine.Rendering.Universal.Light2D lightComponent;
    bool lightChanged = false;
    private float elapsedDT = 0f;
    private float intensityChangeTime;
    private float intensityChangeTimeRange;
    private float originalLightScale;
    private float intensityModifierRange;
    private Color color;
    // Random rand;

    // Start is called before the first frame update
    void Start()
    {
        lightComponent = GetComponent<UnityEngine.Rendering.Universal.Light2D>(); 
        baseIntensity = lightComponent.intensity;
        color = lightComponent.color;
        // lightComponent.color = color;
        lightChanged = false;
        // GetComponent<SpriteRenderer>().color = flameColor;
        intensityChangeTime = .125f;
        intensityChangeTimeRange = 1f;
        intensityModifierRange = .25f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Debug.Log(Random.value);
        if(!lightChanged && Random.value > .5){
            var randomModifier = Random.value;
            lightComponent.intensity = 1f + randomModifier * intensityModifierRange;
            lightChanged = true;
            intensityChangeTime = Random.value * intensityChangeTimeRange;

            var scaleRange = .125f;
            var newScale = originalLightScale + scaleRange * (randomModifier - .5f);
        } else {
            elapsedDT += Time.deltaTime;

            if(elapsedDT >= intensityChangeTime){
                elapsedDT = 0;
                lightChanged = false;
                lightComponent.intensity = baseIntensity;
            }
        }
    }
}
