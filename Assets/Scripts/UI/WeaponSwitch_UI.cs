using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitch_UI : MonoBehaviour, I_ControlsSwapUI
{
    [SerializeField] GameObject KeyboardMouse_UI;
    [SerializeField] GameObject Playstation_UI;
    [SerializeField] GameObject Xbox_UI;
    [SerializeField] GameObject Nintendo_UI;
    Dictionary<ControlType, GameObject> controlTypeDictionary;
    ControlType currentControlType = ControlType.KEYBOARD_MOUSE;

    void Awake()
    {
        controlTypeDictionary = new Dictionary<ControlType, GameObject>();
        controlTypeDictionary.Add(ControlType.KEYBOARD_MOUSE, KeyboardMouse_UI);
        controlTypeDictionary.Add(ControlType.PLAYSTATION, Playstation_UI);
        controlTypeDictionary.Add(ControlType.XBOX, Xbox_UI);
        controlTypeDictionary.Add(ControlType.NINTENDO, Nintendo_UI);
        gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateControlsUI(){
        controlTypeDictionary[currentControlType].SetActive(false);
        currentControlType = ControlsManager.Instance.CurrentControlType;
        controlTypeDictionary[currentControlType].SetActive(true);
    }
}
