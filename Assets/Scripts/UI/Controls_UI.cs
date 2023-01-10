using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Controls_UI : MonoBehaviour, I_ControlsSwapUI
{
    [SerializeField] GameObject KeyboardMouse_UI;
    [SerializeField] GameObject Playstation_UI;
    [SerializeField] GameObject Xbox_UI;
    [SerializeField] GameObject Nintendo_UI;
    [SerializeField] GameObject Staff_UI;
    [SerializeField] GameObject Sword_UI;
    [SerializeField] WeaponSwitch_UI WeaponSwitch_UI;
    WeaponTypeUI activeWeaponType = WeaponTypeUI.Staff;
    Dictionary<ControlType, GameObject> controlTypeDictionary;
    ControlType currentControlType = ControlType.KEYBOARD_MOUSE;

    // Start is called before the first frame update
    void Start() {
        controlTypeDictionary = new Dictionary<ControlType, GameObject>();
        controlTypeDictionary.Add(ControlType.KEYBOARD_MOUSE, KeyboardMouse_UI);
        controlTypeDictionary.Add(ControlType.PLAYSTATION, Playstation_UI);
        controlTypeDictionary.Add(ControlType.XBOX, Xbox_UI);
        controlTypeDictionary.Add(ControlType.NINTENDO, Nintendo_UI);
        // WeaponSwitch_UI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() {

    }

    public void UpdateControlsUI(){
        controlTypeDictionary[currentControlType].SetActive(false);
        currentControlType = ControlsManager.Instance.CurrentControlType;
        controlTypeDictionary[currentControlType].SetActive(true);
    }

    public void SetSwordUiActive(){
        Staff_UI.SetActive(false);
        Sword_UI.SetActive(true);
        activeWeaponType = WeaponTypeUI.Sword;
    }
    
    public void SetStaffUiActive(){
        Staff_UI.SetActive(true);
        Sword_UI.SetActive(false);
        activeWeaponType = WeaponTypeUI.Staff;
    }

    public void FadeUiElements(float duration, float alpha = 0f, float delay = 0f){
        GameObject activeUiElement = activeWeaponType == WeaponTypeUI.Staff ? Staff_UI : Sword_UI;

        this.GetComponent<CanvasGroup>().DOFade(alpha, duration).SetDelay(delay);
    }

    public void ActivateWeaponSwitchUI(){
        WeaponSwitch_UI.gameObject.SetActive(true);
        WeaponSwitch_UI.UpdateControlsUI();
    }
}

public enum WeaponTypeUI{
    Staff,
    Sword
}