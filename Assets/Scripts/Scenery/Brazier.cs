using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;

public class Brazier : LightableObject, ISwitchable
{
    [SerializeField] Light2D light2D;
    [SerializeField] GameObject flameSprite;

    [SerializeField] GameObject innerFlameSprite;
    [SerializeField] GameObject outerFlameSprite;
    [SerializeField] GameObject flameBase;

    [Header("Flame Base Shading")]
    [SerializeField] bool useFlameBaseShades = true;
    [Range(-1f, 1f)] [SerializeField] float innerflameBrightening = .4f;
    [Range(-1f, 1f)] [SerializeField] float outerflameBrightening = -.3f;
    [Range(-1f, 1f)] [SerializeField] float flameLightBrightening = 0f;
    
    [Header("Brazier Connections")]
    [SerializeField] List<Brazier> connectedBraziers;
    [SerializeField] List<GameObject> connectedSwitchables;
    [Header("Heat Distortion")]
    [SerializeField] GameObject distortionObject;
    [SerializeField] bool useDistortionEffect = true;
    [SerializeField] Material distortionMaterial;

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
        SetOn(on, false);
        initialLoad = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Initialize(){
        if(!useFlameBaseShades){
            return;
        }

        Color targetColor = flameBase.GetComponent<SpriteRenderer>().color;
        innerFlameSprite.GetComponent<SpriteRenderer>().color = Utility.GetColorShade(targetColor, innerflameBrightening);
        outerFlameSprite.GetComponent<SpriteRenderer>().color = Utility.GetColorShade(targetColor, outerflameBrightening); 
        light2D.color = Utility.GetColorShade(targetColor, flameLightBrightening);
    }

    public override void SetOn(bool on, bool flipOthers = true){
        if(this.on == on && !initialLoad){
            return;
        }

        light2D.gameObject.SetActive(on);
        flameSprite.SetActive(on);

        if(flipOthers){
            foreach(Brazier brazier in connectedBraziers){
                brazier.FlipSwitch(false);
            }

        }
        
        foreach(GameObject switchableObj in connectedSwitchables){
            ISwitchable switchable = switchableObj.GetComponent<ISwitchable>();
            if(on){
                switchable.Activate();
            } else {
                switchable.Deactivate();
            }
        }
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

    public override void FlipSwitch(bool flipOthers = true){
        on = !on;

        light2D.gameObject.SetActive(on);
        flameSprite.SetActive(on);

        if(useDistortionEffect){
            distortionObject.SetActive(true);
        }

        if(flipOthers){
            foreach(Brazier brazier in connectedBraziers){
                brazier.FlipSwitch(false);
            }
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

    public override void SetOff()
    {
        SwitchOffPlusConnected();
    }

    public virtual void SwitchOffPlusConnected(){
        on = false;

        light2D.gameObject.SetActive(on);
        flameSprite.SetActive(on);

        // foreach(Brazier brazier in connectedBraziers){
        //     brazier.FlipSwitch(false);
        // }

        foreach(GameObject switchableObj in connectedSwitchables){
            ISwitchable switchable = switchableObj.GetComponent<ISwitchable>();
            switchable.Deactivate();
        }

        if(useDistortionEffect){
            distortionObject.SetActive(false);
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

    public void Activate(){
        SetOn(true, false);
    }

    public void Deactivate(){
        SetOn(false, false);
    }
}
