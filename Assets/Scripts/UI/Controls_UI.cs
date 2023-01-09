using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Controls_UI : MonoBehaviour
{
    [SerializeField] GameObject Mouse_UI;
    [SerializeField] GameObject Staff_UI;
    [SerializeField] GameObject Sword_UI;
    [SerializeField] GameObject WeaponSwitch_UI;
    ControlTypeUI activeControlType = ControlTypeUI.Staff;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSwordUiActive(){
        Staff_UI.SetActive(false);
        Sword_UI.SetActive(true);
        activeControlType = ControlTypeUI.Sword;
    }
    
    public void SetStaffUiActive(){
        Staff_UI.SetActive(true);
        Sword_UI.SetActive(false);
        activeControlType = ControlTypeUI.Staff;
    }

    public void FadeUiElements(float duration, float alpha = 0f, float delay = 0f){
        GameObject activeUiElement = activeControlType == ControlTypeUI.Staff ? Staff_UI : Sword_UI;

        this.GetComponent<CanvasGroup>().DOFade(alpha, duration).SetDelay(delay);
    }

    public void ActivateWeaponSwitchUI(){
        WeaponSwitch_UI.SetActive(true);
    }
}

public enum ControlTypeUI{
    Staff,
    Sword
}