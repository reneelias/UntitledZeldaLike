using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;

public class TorchFlame : MonoBehaviour
{
    [SerializeField] Light2D light2D;
    [SerializeField] GameObject innerFlameSprite;
    [SerializeField] GameObject outerFlameSprite;
    [Range(-1f, 1f)] [SerializeField] float innerflameBrightening = .4f;
    [Range(-1f, 1f)] [SerializeField] float outerflameBrightening = -.3f;
    [Header("Heat Distortion")]
    [SerializeField] GameObject distortionObject;
    [SerializeField] bool useDistortionEffect = true;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Initialize(){

        Color targetColor = light2D.color;
        Color newColor = Utility.GetColorShade(targetColor, innerflameBrightening);
        newColor.a = 1f;
        innerFlameSprite.GetComponent<SpriteRenderer>().color = newColor;
        newColor = Utility.GetColorShade(targetColor, outerflameBrightening);
        newColor.a = 1f;
        outerFlameSprite.GetComponent<SpriteRenderer>().color = newColor;

        if(useDistortionEffect){
            distortionObject.SetActive(true);
        }
    }
}
