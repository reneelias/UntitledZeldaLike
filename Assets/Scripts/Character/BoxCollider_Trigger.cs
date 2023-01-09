using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCollider_Trigger : MonoBehaviour
{
    [SerializeField] CharacterControls characterControls;
    public CharacterControls CharacterControls{
        get => characterControls;
        protected set => characterControls = value;
    }
    [SerializeField] Collider2D collider;
    public bool characterFalling = false;
    bool onGround = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetCharacterFalling(){

    }

    public void DodgeEnded(){
        Collider2D[] collider2Ds = new Collider2D[10];
        collider.OverlapCollider(new ContactFilter2D().NoFilter(), collider2Ds);
        Collider2D belowGroundCollider = new Collider2D();
        bool touchedBelowGround = false;
        bool touchedGround = false;
        
        for(int i = 0; i < collider2Ds.Length; i++){
            Collider2D currentCollider2D = collider2Ds[i];
            if(currentCollider2D == null){
            //     if(!characterFalling){
            //         characterControls.ResetLastOnGroundPosition();
            //     }
                break;
            }
            
            if(currentCollider2D.gameObject.layer == LayerMask.NameToLayer("Ground")){
                touchedGround = true;
            }

            if(currentCollider2D.tag == "BelowGround"){
                touchedBelowGround = true;
                belowGroundCollider = currentCollider2D;

                // LayerMask groundLayerMask = LayerMask.GetMask(new string[]{"Ground"});
                // Collider2D[] groundColliders = new Collider2D[1];
                // ContactFilter2D groundFilter = new ContactFilter2D();
                // groundFilter.SetLayerMask(groundLayerMask);

                // collider.OverlapCollider(groundFilter, groundColliders);
 
                // if(!characterFalling){
                //     characterControls.InitiateFall(currentCollider2D);
                // }
                // break;
            }
        }

        if(!characterFalling){
            if(touchedBelowGround && !touchedGround){
                characterControls.InitiateFall(belowGroundCollider);
            } else {
                characterControls.ResetLastOnGroundPosition();
            }
        }
        // collider.OverlapCollider()
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        LayerMask groundLayerMask = LayerMask.GetMask(new string[]{"Ground"});

        switch(other.tag){
            case "BelowGround":
                if(!characterFalling && !collider.IsTouchingLayers(groundLayerMask)){
                    characterControls.InitiateFall(other);
                }
                break;
            case "SlipperyFloor":
                // Collider2D[] collider2Ds = new Collider2D[10];
                // collider.OverlapCollider(new ContactFilter2D().NoFilter(), collider2Ds);
                
                // for(int i = 0; i < collider2Ds.Length; i++){
                //     Collider2D currentCollider2D = collider2Ds[i];
                //     if(currentCollider2D == null){
                //         if(!characterFalling){
                //             characterControls.SlipperyFloorContact();
                //         }
                //         break;
                //     }
                //     if(currentCollider2D.tag == "BelowGround"){
                //         break;
                //     }
                // }

                if(!characterFalling){
                    characterControls.SlipperyFloorContact();
                }
                break;
            // case "Ground":
            //     onGround = true;
            // break;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        LayerMask groundLayerMask = LayerMask.GetMask(new string[]{"Ground"});

        switch(other.tag){
            case "BelowGround":
                if(!characterFalling && !collider.IsTouchingLayers(groundLayerMask)){
                    characterControls.InitiateFall(other);
                }
                break;
            case "SlipperyFloor":
                // Collider2D[] collider2Ds = new Collider2D[10];
                // collider.OverlapCollider(new ContactFilter2D().NoFilter(), collider2Ds);
                
                // for(int i = 0; i < collider2Ds.Length; i++){
                //     Collider2D currentCollider2D = collider2Ds[i];
                //     if(currentCollider2D == null){
                //         if(!characterFalling){
                //             characterControls.SlipperyFloorContact();
                //         }
                //         break;
                //     }
                //     if(currentCollider2D.tag == "BelowGround"){
                //         break;
                //     }
                // }
                if(!characterFalling){
                    characterControls.SlipperyFloorContact();
                }
                break;
            // case "Ground":
            //     onGround = true;
            // break;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        switch(other.tag){
            case "SlipperyFloor":
                // Debug.Log("Exiting Slippery Floor");
                characterControls.ExitSlipperyFloor();
                break;
            // case "Ground":
                // if(other.IsTouchingLayers())
                // onGround = false;
            // break;
        }
    }
}
