using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] ShieldSubObject downShield;
    [SerializeField] ShieldSubObject upShield;
    [SerializeField] ShieldSubObject rightShield;
    [SerializeField] ShieldSubObject leftShield;
    Dictionary<CharacterDirection, ShieldSubObject> directionShieldDictionary;
    ShieldSubObject activeShieldObj;
    bool shieldAnimatingIn = false;
    bool shieldAnimatingOut = false;
    bool shieldEquipped = false;
    bool shieldActive = false;
    [SerializeField] CharacterControls characterControls;
    [SerializeField] PlayableCharacter playableCharacter;

    // Start is called before the first frame update
    void Start()
    {
        directionShieldDictionary = new Dictionary<CharacterDirection, ShieldSubObject>();
        directionShieldDictionary.Add(CharacterDirection.Down, downShield);
        directionShieldDictionary.Add(CharacterDirection.Up, upShield);
        directionShieldDictionary.Add(CharacterDirection.Left, leftShield);
        directionShieldDictionary.Add(CharacterDirection.Right, rightShield);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShieldDrawOrder();
    }

    void FixedUpdate()
    {
        UpdateShieldEquipped();    
    }

    void UpdateShieldEquipped(){
        if(!shieldActive){
            return;
        }

        playableCharacter.ChangeStamina(0);
    }

    void UpdateShieldDrawOrder(){
        if(!shieldActive){
            return;
        }
        int sortingDiff = 1;

        switch(characterControls.CharacterDirection){
            case CharacterDirection.Down:
                sortingDiff = 2;
                break;
            case CharacterDirection.Up:
                sortingDiff = -2;
                break;
            case CharacterDirection.Left:
                sortingDiff = 1;
                break;
            case CharacterDirection.Right:
                sortingDiff = -2;
                break;
        }

        activeShieldObj.GetComponent<SpriteRenderer>().sortingOrder = characterControls.SortingOrder + sortingDiff;
    }

    public void EquipShield(CharacterDirection playerDirection){
        activeShieldObj = directionShieldDictionary[playerDirection];
        activeShieldObj.gameObject.SetActive(true);
        shieldEquipped = true;
        AnimateShieldIn();
    }

    public void UnEquipShield(){
        shieldActive = false;
        AnimateShieldOut();
    }

    public void AnimateShieldIn(){
        shieldActive = true;
    }

    public void AnimateShieldOut(){
        shieldEquipped = false;
        activeShieldObj.gameObject.SetActive(false);
    }

    public void ShieldHit(int staminaCost = 10){
        playableCharacter.ChangeStamina(-staminaCost);
    }
}
