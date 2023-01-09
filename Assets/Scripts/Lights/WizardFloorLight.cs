using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;

public class WizardFloorLight : MonoBehaviour
{
    Light2D lightComponent;
    [SerializeField] GameObject player;
    [SerializeField] float baseCharacterScale = 1.39f;
    [SerializeField] GameObject downPosition;
    [SerializeField] GameObject rightPosition;
    [SerializeField] GameObject leftPosition;
    [SerializeField] GameObject upPosition;
    WFL_FlipType flipType = WFL_FlipType.None;
    CharacterControls characterControls;
    [Header("Detached From Staff")]
    [SerializeField] bool detached = false;
    [SerializeField] GameObject detachedPositionDown;
    [SerializeField] GameObject detachedPositionRight;
    [SerializeField] GameObject detachedPositionUp;
    [SerializeField] GameObject detachedPositionLeft;
    [Range(0, 1f)] public float smoothingTimeDetach = .25f;
    [Range(0, 1f)] public float smoothingTimeAttach = .125f;
    private Vector2 velocity;
    bool returningToStaff = false;
    // Start is called before the first frame update
    void Start()
    {
        characterControls = player.GetComponent<CharacterControls>();

        lightComponent = GetComponent<Light2D>(); 
        lightComponent.color = player.GetComponent<WizardControls>().flameColor;
        lightComponent.color.a = 1;

        lightComponent.size *= transform.parent.localScale.x / baseCharacterScale;
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
        // UpdateFlip();
        UpdatePosition();
        UpdateDetachedPosition();
        UpdateReturnToStaff();
    }
    
    // void UpdateFlip(){
    //     if(detached || returningToStaff){
    //         return;
    //     }

    //     switch(flipType){
    //         case WFL_FlipType.None:
    //             transform.position = normalPosition.transform.position;
    //             break;
    //         case WFL_FlipType.Flip:
    //             transform.position = flippedPosition.transform.position;
    //             break;
    //         case WFL_FlipType.UpFlip:
    //             transform.position = upFlipPosition.transform.position;
    //             break;
    //     }
    // }

    
    protected void UpdatePosition(){
        if(detached || returningToStaff){
            return;
        }

        switch(characterControls.CharacterDirection){
            case CharacterDirection.Right:
                transform.position = rightPosition.transform.position;
                break;
            case CharacterDirection.Down:
                transform.position = downPosition.transform.position;
                break;
            case CharacterDirection.Up:
                transform.position = upPosition.transform.position;
                break;
            case CharacterDirection.Left:
                transform.position = leftPosition.transform.position;
                break;
        }
    }
    
    void UpdateDetachedPosition(){
        if(!detached){
            return;
        }

        Vector3 targetPosition = Vector3.zero;
        switch(characterControls.CharacterDirection){
            case CharacterDirection.Down:
                targetPosition = detachedPositionDown.transform.position;
                break;
            case CharacterDirection.Up:
                targetPosition = detachedPositionUp.transform.position;
                break;
            case CharacterDirection.Left:
                targetPosition = detachedPositionLeft.transform.position;
                break;
            case CharacterDirection.Right:
                targetPosition = detachedPositionRight.transform.position;
                break;
        }

        Vector3 newPosition = Vector3.zero;
        newPosition.x = Mathf.SmoothDamp(transform.position.x, targetPosition.x, ref velocity.x, smoothingTimeDetach);
        newPosition.y = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref velocity.y, smoothingTimeDetach);
        newPosition.z = transform.position.z;

        transform.position = newPosition;
    }
    void UpdateReturnToStaff(){
        if(!returningToStaff){
            return;
        }

        Vector3 targetPosition = Vector3.zero;
        
        switch(characterControls.CharacterDirection){
            case CharacterDirection.Right:
                targetPosition = rightPosition.transform.position;
                break;
            case CharacterDirection.Down:
                targetPosition = downPosition.transform.position;
                break;
            case CharacterDirection.Up:
                targetPosition = upPosition.transform.position;
                break;
            case CharacterDirection.Left:
                targetPosition = leftPosition.transform.position;
                break;
        }

        targetPosition.z = transform.position.z;

        Vector3 newPosition = Vector3.zero;
        newPosition.x = Mathf.SmoothDamp(transform.position.x, targetPosition.x, ref velocity.x, smoothingTimeAttach);
        newPosition.y = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref velocity.y, smoothingTimeAttach);
        newPosition.z = transform.position.z;

        transform.position = newPosition;
        
        if((transform.position - targetPosition).magnitude < .05f){
            returningToStaff = false;
        }
    }

    public void DetachFromStaff(){
        detached = true;
    }

    public void AttachToStaff(){
        detached = false;
        returningToStaff = true;
    }

    public void Flip(WFL_FlipType flipType){
        this.flipType = flipType;
    }
}

public enum WFL_FlipType{
    None,
    Flip,
    UpFlip
}