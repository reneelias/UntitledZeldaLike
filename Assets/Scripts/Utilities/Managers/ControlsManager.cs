using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : Singleton<ControlsManager>
{
    [SerializeField] Sprite keyboardMouse_InteractSprite;
    [SerializeField] Sprite playstation_InteractSprite;
    [SerializeField] Sprite xbox_InteractSprite;
    [SerializeField] Sprite nintendo_InteractSprite;
    Dictionary<ControlType, Sprite> interactSpriteDictionary;
    ControlType currentControlType = ControlType.KEYBOARD_MOUSE;
    public ControlType CurrentControlType{
        protected set => currentControlType = value;
        get => currentControlType;
    }
    [SerializeField] GameObject[] controlSwapUI_Array;

    // Start is called before the first frame update
    void Start()
    {
        interactSpriteDictionary = new Dictionary<ControlType, Sprite>();
        interactSpriteDictionary.Add(ControlType.KEYBOARD_MOUSE, keyboardMouse_InteractSprite);
        interactSpriteDictionary.Add(ControlType.PLAYSTATION, playstation_InteractSprite);
        interactSpriteDictionary.Add(ControlType.XBOX, xbox_InteractSprite);
        interactSpriteDictionary.Add(ControlType.NINTENDO, nintendo_InteractSprite);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrentControlType(string controllerName){
        switch(controllerName){
            case "DualShock4GamepadHID":
            case "DualSenseGamepadHID":
                currentControlType = ControlType.PLAYSTATION;
                break;
            case "SwitchProControllerHID":
                currentControlType = ControlType.NINTENDO;
                break;
            case "XInputControllerWindows":
                currentControlType = ControlType.XBOX;
                break;
            case "Keyboard_Mouse":
            default:
                currentControlType = ControlType.KEYBOARD_MOUSE;
                break;
        }

        foreach(GameObject controlsSwapUI in controlSwapUI_Array){
            controlsSwapUI.GetComponent<I_ControlsSwapUI>().UpdateControlsUI();
        }
    }

    public Sprite GetCurrentInteractSprite(){
        return interactSpriteDictionary[currentControlType];
    }
}

public enum ControlType{
    KEYBOARD_MOUSE,
    PLAYSTATION,
    XBOX,
    NINTENDO
}