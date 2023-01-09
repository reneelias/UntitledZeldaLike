using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;

public class FlickeringLightBehaviorSL2D : MonoBehaviour
{
   float baseIntensity;
    Light2D lightComponent;
    bool lightChanged = false;
    private float elapsedDT = 0f;
    private float intensityChangeTime;
    private float intensityChangeTimeRange;
    private float originalLightScale;
    [SerializeField] private float intensityModifierRange = .25f;
    private Color color;
    // Random rand;

    // Start is called before the first frame update
    void Start()
    {
        lightComponent = GetComponent<Light2D>(); 
        color = lightComponent.color;
        baseIntensity = color.a;
        // lightComponent.color = color;
        lightChanged = false;
        // GetComponent<SpriteRenderer>().color = flameColor;
        intensityChangeTime = .25f;
        intensityChangeTimeRange = 1f;
        // intensityModifierRange = .25f;
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
            lightComponent.color.a = baseIntensity - randomModifier * intensityModifierRange;
            lightChanged = true;
            intensityChangeTime = Random.value * intensityChangeTimeRange;

            // intensityChangeTime = .5f + (.025f * Random.value * ((Random.value < 5f) ? 1 : -1));

            var scaleRange = .125f;
            var newScale = originalLightScale + scaleRange * (randomModifier - .5f);
        } else {
            elapsedDT += Time.deltaTime;

            if(elapsedDT >= intensityChangeTime){
                elapsedDT = 0;
                lightChanged = false;
                lightComponent.color.a = baseIntensity;
            }
        }
    }
}
