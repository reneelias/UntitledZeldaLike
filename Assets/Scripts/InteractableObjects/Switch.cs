using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour, ISwitchable
{
    public bool Activated{
        protected set; get;
    } = false;

    [SerializeField] GameObject unpressedSprite;
    [SerializeField] GameObject pressedSprite;
    [SerializeField] GameObject[] objectsToSwtichOn;
    [SerializeField] GameObject[] objectsToSwtichOff;
    [SerializeField] LightableObject[] lightsToSwitchOn;
    [SerializeField] LightableObject[] lightsToSwitchOff;
    [SerializeField] bool resetsWithTime = false;
    [SerializeField] float activatedTime = 5f;
    float activatedDT = 0f;
    // Start is called before the first frame update
    bool activatePermanently = false;
    [SerializeField] bool pressableByObjects = false;

    [SerializeField] AudioClip switchSound;
    public bool ActivatePermanently{
        get{
            return activatePermanently;
        }
        set{
            activatePermanently = value;
            if(activatePermanently){
                Activate();
            } else {
                Deactivate();
            }
        }
    }
    void Start()
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.Find("WizardBoxCollider").GetComponent<Collider2D>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        UpdateActivated();        
    }

    void UpdateActivated(){
        if(!Activated || ActivatePermanently || !resetsWithTime){
            return;
        }

        activatedDT += Time.deltaTime;

        if(activatedDT >= activatedTime){
            Deactivate();
        }
    }

    public void Activate(){
        Activated = true;
        GenericActivationActions();
        activatedDT = 0f;
        

        foreach(GameObject switchableObject in objectsToSwtichOn){
            switchableObject.GetComponent<ISwitchable>().Activate();

            if(ActivatePermanently){
                switchableObject.GetComponent<ISwitchable>().ActivatePermanently = true;
            } else {
                switchableObject.GetComponent<ISwitchable>().Activate();
            }
        }

        foreach(LightableObject lightableObject in lightsToSwitchOff){
            lightableObject.SetOn(false, false);
        }
    }

    public void Deactivate(){
        Activated = false;
        GenericActivationActions();

        foreach(GameObject switchableObject in objectsToSwtichOn){
            switchableObject.GetComponent<ISwitchable>().Deactivate();
        }
    }

    public void GenericActivationActions(bool playSound = true){
        unpressedSprite.SetActive(!Activated);
        pressedSprite.SetActive(Activated);
        if(playSound){
            GameMaster.Instance.audioSource.PlayOneShot(switchSound, GameMaster.Instance.MasterVolume);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(Activated){
            return;
        }

        switch(other.tag){
            case "PlayerBoxCollider":
            case "Breakable":
                Activate();
            break;
        }
        // if(other.tag == "PlayerBoxCollider"){
        //     Activate();
        // }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "Breakable"){
            Skull skull = other.gameObject.GetComponent<Skull>();
            
        }

        if(Activated){
            return;
        }
        
        switch(other.tag){
            case "PlayerBoxCollider":
            case "Breakable":
                if(resetsWithTime){
                    Activate();
                }
            break;
        }
        // if(other.tag == "PlayerBoxCollider" && resetsWithTime){
        //     Activate();
        // }        
    } 
    void OnTriggerExit2D(Collider2D other)
    {
        // if(Activated){
        //     return;
        // }

        switch(other.tag){
            case "PlayerBoxCollider":
            case "Breakable":
                if(!resetsWithTime){
                    Activated = false;
                    GenericActivationActions(false);
                }
            break;
        }
        
        // if(other.tag == "PlayerBoxCollider" && !resetsWithTime){
        //     Activated = false;
        //     GenericActivationActions(false);
        // }        
    }
}
