using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;

public class TorchFlame : LightableObject, ISwitchable
{
    [SerializeField] Light2D light2D;
    [SerializeField] GameObject innerFlameSprite;
    [SerializeField] GameObject outerFlameSprite;
    [Range(-1f, 1f)] [SerializeField] float innerflameBrightening = .4f;
    [Range(-1f, 1f)] [SerializeField] float outerflameBrightening = -.3f;
    [Header("Heat Distortion")]
    [SerializeField] GameObject distortionObject;
    [SerializeField] bool useDistortionEffect = true;
    public bool Activated{
        get;
    }
    
    public bool ActivatePermanently{
        get; set;
    }
    bool initialLoad = true;
    
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        LightableByBeam = false;
        globalDarknessModifier = 0f;
        SetOn(on, false);
        initialLoad = false;
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

    public override void SetOn(bool on, bool flipOthers = true){
        if(this.on == on && !initialLoad){
            return;
        }

        light2D.gameObject.SetActive(on);
        innerFlameSprite.SetActive(on);
        outerFlameSprite.SetActive(on);

        this.on = on;

        if(useDistortionEffect){
            distortionObject.SetActive(on);
        }
        
        if(GameMaster.Instance.dungeon.CurrentRoom == null){
            return;
        }

        float darknessModifier = globalDarknessModifier;
        if(on){
            darknessModifier *= -1;
        }
        GameMaster.Instance.dungeon.CurrentRoom.ModifyDarknessColor(darknessModifier);
    }

    public override void SetOff(){

    }

    public override void FlipSwitch(bool flipOthers = true){

    }
    public void Activate(){
        SetOn(true, false);
    }

    public void Deactivate(){
        SetOn(false, false);
    }
}
