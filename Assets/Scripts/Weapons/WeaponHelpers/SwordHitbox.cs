using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    [SerializeField] CharacterControls characterControls;
    [SerializeField] PlayableCharacter playableCharacter;
    Vector3 originalLocalPosition;
    // Start is called before the first frame update
    void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        SetLocalPosition();
    }

    public void SetLocalPosition(){
        transform.localPosition = originalLocalPosition;
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
         switch(other.tag){
            case "Enemy":
            case "PickUp":
            case "Breakable":
            case "Scenery":
                characterControls.WeaponHitTarget(other.gameObject);
                if(other.tag == "Enemy"){
                    playableCharacter.ChangeStamina(-other.gameObject.GetComponent<Enemy>().PlayerSwordHitStaminaCost);
                }
            break;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
         switch(other.gameObject.tag){
            case "Scenery":
                characterControls.WeaponHitTarget(other.gameObject);                
            break;
        }
    }
}
