using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class Crosshair : MonoBehaviour
{
    [SerializeField] float aimingRadius = 3f;
    [SerializeField] VariableJoystick shootingJoystick;
    [SerializeField] GameObject originObject;
    Vector3 targetPosition;
    public PlayerControls controls;
    public Vector3 Position{
        get => transform.position;
    }
    Vector2 aimingInput;
    [SerializeField] CharacterControls characterControls;
    Vector3 previousMousePosition;
    Vector3 relativePositionToObject;
    Gamepad gamepad;
    string currentInputScheme = "Keyboard_Mouse";
    Color originalColor;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] bool useControllerTargetSnapping = true;

    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        // Cursor.lockState = CursorLockMode.locked;
        Cursor.visible = false;
        controls = characterControls.Controls;
        // controls.Gameplay.Aiming.performed += ctx => aimingInput = ctx.ReadValue<Vector2>();
        // controls.Gameplay.Aiming.performed += ctx => AimTriggered();
        // controls.Gameplay.Aiming.canceled += ctx => aimingInput = Vector2.zero;
        originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // UpdateAimingInput();
        UpdatePosition();
        // gamepad = Gamepad.current;

        // if(gamepad != null){
        //     // if(gamepad.allControls.){
        //     //     Debug.Log("Controller has been actuated.");
        //     // }

        //     InputControl[] inputArray = gamepad.allControls.ToArray();
        //     for(int i = 0; i < inputArray.Length; i++){
        //         // Debug.Log(inputArray[i].GetType());

        //         if(inputArray[i].IsPressed()){
        //             Debug.Log("Button pressed");
        //             break;
        //         }
        //     }
        // }
    }
    
    void FixedUpdate()
    {
    }

    void UpdateAimingInput(){
        aimingInput = characterControls.AimingInput;
    }

    public void SetAimingInput(){
        aimingInput = characterControls.AimingInput;
    }

    void UpdatePosition(){
        // transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool shootJoystickInput = (Math.Abs(shootingJoystick.Horizontal) > 0 || Math.Abs(shootingJoystick.Vertical) > 0)
        || aimingInput.magnitude > 0f;

        bool inputDetected = false;
        
        if (shootJoystickInput) {

            if(aimingInput.magnitude > 0f){
                targetPosition = aimingInput;
            } else {
                targetPosition = new Vector3(shootingJoystick.Horizontal, shootingJoystick.Vertical, 0f);
            }
            targetPosition = targetPosition.normalized * aimingRadius;
            targetPosition += originObject.transform.position;
            inputDetected = true;
        } else {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Vector3 ogMouseInput = Input.mousePosition;
            // Vector2 newMouseInput = controls.Gameplay.MousePosition.ReadValue<Vector2>();
            // targetPosition = Camera.main.ScreenToWorldPoint(controls.Gameplay.MousePosition.ReadValue<Vector2>());
            targetPosition.z = 0f;
        }
        
        if(currentInputScheme == "Gamepad" && !GameMaster.Instance.GameOver && !GameMaster.Instance.UI_FadedOut){

            if(useControllerTargetSnapping){
                GameObject raycastObj = characterControls.StaffLightSprite.GetNearestRaycastObject(targetPosition);

                if(raycastObj != null){
                    float newMagnitude = (raycastObj.transform.position - originObject.transform.position).magnitude;
                    targetPosition = originObject.transform.position + (targetPosition - originObject.transform.position).normalized * newMagnitude;
                    spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
                    // spriteRenderer.color = Color.red;
                    // Debug.Log($"Enemy Target Name: {raycastObj.transform.name}");
                }  else {
                    spriteRenderer.color = originalColor;
                }
            }

            if(inputDetected){
                    transform.position = targetPosition;
                    relativePositionToObject = transform.position - originObject.transform.position;
                } else {
                    transform.position = originObject.transform.position + relativePositionToObject;
                }
        } else {
                transform.position = targetPosition;
        }
        // Debug.Log(Position);

        previousMousePosition = Input.mousePosition;
    }

    public void SetInputeScheme(string inputScheme){
        currentInputScheme = inputScheme;
    }

    public void SetCursorVisible(bool visible = true){
        Cursor.visible = visible;
    }

    public void SetCrosshairVisible(bool visible = true, bool overrideScheme = false){
        if(!overrideScheme && characterControls.CurrentWeaponType == CharacterControls.WeaponType.Sword){
            return;
        }
        
        gameObject.GetComponent<Renderer>().enabled = visible;
    }

    public void SetControllerCrosshairSnapping(bool snapping){
        useControllerTargetSnapping = snapping;
    }
}
