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
    [SerializeField] Sprite keyboardMouse_BackgroundSprite;
    [SerializeField] Sprite controller_BackgroundSprite;
    Dictionary<ControlType, Sprite> interactSpriteDictionary;
    ControlType currentControlType = ControlType.KEYBOARD_MOUSE;
    public ControlType CurrentControlType{
        protected set => currentControlType = value;
        get => currentControlType;
    }
    [SerializeField] GameObject[] controlSwapUI_Array;
    float hapticsDT = 0f;
    float hapticsDuration;
    bool hapticsActive = false;
    bool rumbleEnabled = true;

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
        UpdateHapticsTimer();
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

    public void UpdateHapticsTimer(){
        if(!hapticsActive){
            return;
        }

        hapticsDT += Time.deltaTime;

        if(hapticsDT >= hapticsDuration){
            hapticsActive = false;
            Gamepad.current.ResetHaptics();
        }
    }

    public Sprite GetCurrentInteractSprite(){
        return interactSpriteDictionary[currentControlType];
    }

    public Sprite GetCurrentBackgroundSprite(){
        return currentControlType == ControlType.KEYBOARD_MOUSE ? keyboardMouse_BackgroundSprite : controller_BackgroundSprite;
    }

    public void PlayControllerHaptics(float leftMotorIntensity, float rightMotorIntensity, float duration){
        if(!rumbleEnabled || Gamepad.current == null){
            return;
        }

        float rumbleModifier = CurrentControlType == ControlType.PLAYSTATION ? 1f : 1.25f;
        Gamepad.current.SetMotorSpeeds(leftMotorIntensity * rumbleModifier, rightMotorIntensity * rumbleModifier);
        // Debug.Log("Playing controller haptics");

        hapticsActive = true;
        hapticsDuration = duration;
        hapticsDT = 0f;
    }

    public void EnableRumble(bool enable){
        rumbleEnabled = enable;
    }
}

public enum ControlType{
    KEYBOARD_MOUSE,
    PLAYSTATION,
    XBOX,
    NINTENDO
}