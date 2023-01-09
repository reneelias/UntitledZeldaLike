using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatformGrid : MonoBehaviour
{
    [SerializeField] FloatingPlatform floatingPlatformParent;
    [SerializeField] Rigidbody2D rigidbody;
    bool playerOnPlatform = false;
    CharacterControls characterControls;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
        UpdatePlayerCollisionCheck();
    }

    void UpdatePlayerCollisionCheck(){
        if(!playerOnPlatform){
            return;
        }

        Collider2D[] contacts = new Collider2D[1];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask(new string[]{"Player"}));
        rigidbody.OverlapCollider(contactFilter, contacts);

        if(contacts[0] == null){
            floatingPlatformParent.PlayerExitedPlatform(characterControls);
            playerOnPlatform = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.tag){
            case "Player":
                characterControls = other.gameObject.GetComponent<CharacterControls>();
                
                if(characterControls != null){
                    floatingPlatformParent.PlayerEnteredPlatform(characterControls);
                    playerOnPlatform = true;
                }
            break;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        switch(other.tag){
            case "Player":
                floatingPlatformParent.PlayerExitedPlatform(characterControls);
                playerOnPlatform = false;
            break;
        }
    }
}
