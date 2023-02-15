using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.InputSystem;

public class StaffLightSprite : MonoBehaviour
{
    protected PlayerControls controls;
    private Color flameColor;
    public int numOfOrbitingParticles;
    public float orbitingParticlesAngleSeparation;
    public float orbitingParticlesRadius;
    public float orbitingParticleAngularSpeed;
    public OrbitingStaffParticle orbitingParticlePrefab;
    public bool orbitingParticlesShouldRotate;
    public float orbitingParticlesAngleLerpSpeed;
    OrbitingStaffParticle[] orbitingParticles;
    private bool orientingParticles;
    private float particleOrientationDT;
    private float particleOrientationTime;

    StaffBeamMain[] staffMainBeamParticles;
    int staffMainBeamParticleIndex;
    public StaffBeamMain staffMainBeamParticlePrefab;
    
    private bool onCooldown;
    private float cooldownDT;
    [Header("Beam Preferences")]
    public int mainBeamDamage = 5;
    [SerializeField] public int numOfMainBeamParticles = 10;
    [SerializeField] protected float cooldownTime = .5f;

    private bool clicked;
    private Vector3 targetPosition;
    private bool orientParticles;

    private Vector3 mouseDifferenceUnitVector;

    [SerializeField] private float mainBeamSpeed = 20f;
    [SerializeField] float baseCharacterScale = 1.39f;
    float scale;
    [SerializeField] int beamStaminaCost = 10;

    [SerializeField] ParticleEmitter particleEmitter;
    [SerializeField] AudioClip beamAudioClip;
    [SerializeField] VariableJoystick movementJoystick;
    [SerializeField] VariableJoystick shootingJoystick;

    [SerializeField] float joystickAimingRadius = 3f;
    [SerializeField] CharacterControls characterControls;
    [SerializeField] PlayableCharacter playableCharacter;
    public bool SuspendShooting = false;
    [SerializeField] GameObject downPosition;
    [SerializeField] GameObject upPosition;
    [SerializeField] GameObject rightPosition;
    [SerializeField] GameObject leftPosition;
    bool flipped = false;
    [Header("Detached From Staff")]
    [SerializeField] bool detached = false;
    [SerializeField] GameObject detachedPositionDown;
    [SerializeField] GameObject detachedPositionRight;
    [SerializeField] GameObject detachedPositionUp;
    [SerializeField] GameObject detachedPositionLeft;
    [Range(0, 1f)] public float smoothingTimeDetach = .25f;
    [Range(0, 1f)] public float smoothingTimeAttach = .125f;
    private Vector2 velocity;
    [SerializeField] SortingOrderByY sortingOrderByY;
    bool returningToStaff = false;
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] Rigidbody2D rigidbody;
    [Header("Heat Distortion")]
    [SerializeField] GameObject distortionObject;
    [SerializeField] bool useDistortionEffect = true;
    [Header("Targets Raycast Settings")]
    [Tooltip("How wide of an angle to scan.")]
    [SerializeField] float rayCast_angleRange = 15f;
    [Tooltip("How many angle checks for raycasting")]
    [SerializeField] int rayCast_totalAngleChecks = 4;

    void Awake()
    {
        // controls = new PlayerControls();
        // controls.Gameplay.PrimaryAttack.performed += ctx => ClickTriggered();
    }
    // Start is called before the first frame update
    void Start()
    {
        scale = transform.parent.transform.localScale.x / baseCharacterScale;
        flameColor = GameObject.Find("OldWizard").GetComponent<WizardControls>().flameColor;
        flameColor.a = 1;
        GetComponent<SpriteRenderer>().color = flameColor;

        orbitingParticles = new OrbitingStaffParticle[numOfOrbitingParticles];

        for(int i = 0; i < numOfOrbitingParticles; i++){
            orbitingParticles[i] = Instantiate(orbitingParticlePrefab);
            float angle = 0f - orbitingParticlesAngleSeparation / 2 + orbitingParticlesAngleSeparation * i;
            orbitingParticles[i].initializeValues(angle, orbitingParticlesRadius * scale, orbitingParticleAngularSpeed, gameObject, orbitingParticlesShouldRotate, orbitingParticlesAngleLerpSpeed, i, orbitingParticlesAngleSeparation);
            orbitingParticles[i].gameObject.GetComponent<SpriteRenderer>().sortingLayerName = gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
            orbitingParticles[i].gameObject.GetComponent<SpriteRenderer>().sortingOrder = 3;
            // Debug.Log($"orbitingParticlesShouldRotate: {orbitingParticlesShouldRotate}");
        }

        orientParticles = false;
        orientingParticles = false;


        staffMainBeamParticleIndex = 0;
        staffMainBeamParticles = new StaffBeamMain[numOfMainBeamParticles];

        for(var i = 0; i < numOfMainBeamParticles; i++){
            staffMainBeamParticles[i] = Instantiate(staffMainBeamParticlePrefab);
            staffMainBeamParticles[i].initializeValues(flameColor, gameObject);
            staffMainBeamParticles[i].gameObject.GetComponent<SpriteRenderer>().sortingLayerName = gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
            // staffMainBeamParticles[i].GetComponent<Rigidbody2D>().simulated = false;
            // Debug.Log(staffMainBeamParticles[i]);
        }

        clicked = false;
        onCooldown = false;
        cooldownDT = 0;
        // cooldownTime = .5f;

        particleOrientationDT = 0;
        particleOrientationTime = .5f;

        particleEmitter.color = flameColor;
        particleEmitter.SetSpawnRange(.175f * scale, .22f * scale);

        characterControls = transform.parent.GetComponent<CharacterControls>();

        if(useDistortionEffect){
            distortionObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateSortingLayer();
    }

    void FixedUpdate()
    {
        UpdateCooldown();
        UpdatePosition();
        UpdateDetachedPosition();
        UpdateReturnToStaff();
        // CheckForInput();
        UpdateOrbitingParticleAngles();
        UpdateParticleOrientation();
        // UpdateMainStaffBeam();
        // ResetTouch();
    }


    void UpdatePosition(){
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
            SuspendShooting = false;
            // rigidbody.simulated = false;
        }
    }

    void UpdateCooldown(){
        if(!onCooldown){
            return;
        }

        cooldownDT += Time.deltaTime;
        if(cooldownDT >= cooldownTime){
            onCooldown = false;
            cooldownDT = 0;
        }
    }

    public void ShootBeam()
    {
        if(onCooldown || characterControls.Falling || SuspendShooting || GameMaster.Instance.GameOver || playableCharacter.Stamina < beamStaminaCost || MenuScript.GamePaused){
            if(playableCharacter.Stamina <= 0){
                characterControls.PlayStaminaOutAudio();
            }
            return;
        } else {
            // Debug.Log("NOT ON COOLDOWN");
        }
        
        bool shootJoystickInput = (Math.Abs(shootingJoystick.Horizontal) > 0 || Math.Abs(shootingJoystick.Vertical) > 0);
        // if(controls.Gameplay.PrimaryAttack.triggered){
        //     Debug.Log("Primary attack triggered!");
        // }
        // if ((controls.Gameplay.PrimaryAttack.triggered && Application.platform != RuntimePlatform.Android) || shootJoystickInput)
        // // if ((Input.GetMouseButton(0) && Application.platform != RuntimePlatform.Android) || shootJoystickInput)
        // {
            // Debug.Log("CHANGING CLICK POSITION");
        if(shootJoystickInput){
            targetPosition = new Vector3(shootingJoystick.Horizontal, shootingJoystick.Vertical, 0f);
            targetPosition = targetPosition.normalized * joystickAimingRadius;
            targetPosition += transform.position;
        } else {
            // targetPosition = Camera.main.ScreenToWorldPoint(controls.Gameplay.MousePosition.ReadValue<Vector2>());
            // targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = GameMaster.Instance.crosshair.Position;
        }
        // Debug.Log($"clickPosition: {clickPosition}");
        // if(movementJoystick != null && (Math.Abs(movementJoystick.Horizontal) > 0 || Math.Abs(movementJoystick.Vertical) > 0)){
        //     return;
        // }
        // Debug.Log("Attempting to create beam, removing stamina");
        Vector2 diffVector = targetPosition - transform.position;
        mouseDifferenceUnitVector = diffVector.normalized;
        // CreateStaffBeam();
        clicked = true;
        orientParticles = true;
        if(CreateStaffBeam()){
            playableCharacter.ChangeStamina(-beamStaminaCost);
        }
        // }

    }

    public GameObject GetNearestRaycastObject(Vector3 directionPosition){
        Vector2 directionVector = (directionPosition - transform.position).normalized;
        float originalAngle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
        float startingAngle = originalAngle - rayCast_angleRange / 2f;
        float angleIncrement = rayCast_angleRange * Mathf.Deg2Rad / rayCast_totalAngleChecks;
        float currAngle = startingAngle * Mathf.Deg2Rad;

        LayerMask layerMask = LayerMask.GetMask(new string[]{"Enemy"});
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, directionVector, 20f, layerMask);
        if(raycastHit2D.transform != null && raycastHit2D.transform.gameObject.GetComponent<Enemy>().FloorLevel == characterControls.FloorLevel && Camera.main.GetComponent<CameraBehavior>().IsInCamera(raycastHit2D.collider.bounds)){
            return raycastHit2D.transform.gameObject;
        }

        for(int i = 0; i < rayCast_totalAngleChecks; i++){
            directionVector = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));

            raycastHit2D = Physics2D.Raycast(transform.position, directionVector, 20f, layerMask);
            if(raycastHit2D.transform != null && raycastHit2D.transform.gameObject.GetComponent<Enemy>().FloorLevel == characterControls.FloorLevel && Camera.main.GetComponent<CameraBehavior>().IsInCamera(raycastHit2D.collider.bounds)){
                return raycastHit2D.transform.gameObject;
            }

            currAngle += angleIncrement;
        }

        return null;
    }

    // void UpdateMainStaffBeam(){
    bool CreateStaffBeam(){
        if(!clicked || onCooldown){
        // if(onCooldown){
            // Debug.Log($"Could not create staff beam, cooldown: {onCooldown}, clicked{clicked}");
            return false;
        }

        while(staffMainBeamParticles[staffMainBeamParticleIndex].Active){
            if(++staffMainBeamParticleIndex >= numOfMainBeamParticles){
                staffMainBeamParticleIndex = 0;
            }
        }
        

        staffMainBeamParticles[staffMainBeamParticleIndex].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        StaffBeamMain beamParticle = staffMainBeamParticles[staffMainBeamParticleIndex];
        beamParticle.Activate(transform.position.x, transform.position.y, 10f, mouseDifferenceUnitVector * mainBeamSpeed, flameColor, scale, characterControls.FloorLevel);
        beamParticle.Speed = mainBeamSpeed;
        beamParticle.DistBetweenOrbitingParticles = Vector3.Distance(orbitingParticles[0].transform.position, orbitingParticles[1].transform.position);
        beamParticle.damagePts = mainBeamDamage;
        onCooldown = true;

        GameMaster.Instance.audioSource.PlayOneShot(beamAudioClip, GameMaster.Instance.MasterVolume);
        // Debug.Log("Staff beam created");
        return true;
    }

    void UpdateOrbitingParticleAngles(){
        if(!orientParticles){
            return;
        }
        orientParticles = false;
        orientingParticles = true;

        // Debug.Log(Camera.main.transform.position - transform.parent.gameObject.transform.position);
        // alteredClickPosition.z = 0;
        // Vector3 diffVector = alteredClickPosition - transform.position;
        Vector2 diffVector = targetPosition - transform.position;
        float angleToCrosshair = Mathf.Rad2Deg * Mathf.Atan2(diffVector.y, diffVector.x);
        // mouseDifferenceUnitVector = Vector3.Normalize(diffVector);
        // Vector3 referenceVector = wp + diffVector;
        // float angleToCrosshair = Vector3.Angle(Vector3.zero, referenceVector);
        // float angleToCrosshair = Vector3.Angle(transform.position, wp);
        if(angleToCrosshair < 0){
            angleToCrosshair += 360;
        }
        float targetAngle = -orbitingParticlesAngleSeparation / 2 + angleToCrosshair;
        // if(targetAngle < 0){
        //     targetAngle += 360;
        // }
        
        // if(Input.GetMouseButtonDown(0)){
            // Debug.Log($"referenceVector: {referenceVector}");
            // Debug.Log($"mp: {wp}, gop: {transform.position}");
            // Debug.Log($"angleToCrosshair: {angleToCrosshair}");
        // }
        // var touchPosition = new Vector2(wp.x, wp.y);

        // if(targetAngle < 0){
        //         targetAngle += 360;
        //     }

        
        // bool subtract = false;
        // if(orbitingParticles[0].GetComponent<OrbitingStaffParticleBehavior>().Angle > targetAngle){
        //     subtract = true;
        // }
        for(int i = 0; i < numOfOrbitingParticles; i++){
            OrbitingStaffParticle particleScript = orbitingParticles[i];
            targetAngle += orbitingParticlesAngleSeparation * i;
            float angleDiff = targetAngle - particleScript.Angle;
            if(Mathf.Abs(angleDiff) >= 350){
                // Debug.Log("BIG DIFF");
                // angleDiff += (angleDiff < 0) ? 360 : -360;
                // if(angleToCrosshair >= 0 && angleToCrosshair <= 20){
                //     angleDiff -= 360;
                // }
                // particleScript.setToNewAngle(particleScript.A - )
            }
            // if(Mathf.Abs(angleDiff) >= 180){
            //     Debug.Log("BIG DIFF");
            //     // angleDiff += (angleDiff < 0) ? 360 : -360;
            //     angleDiff += (-(Mathf.Abs(angleDiff) / angleDiff) * 180); 
            //     // if(angleToCrosshair >= 0 && angleToCrosshair <= 20){
            //     //     angleDiff -= 360;
            //     // }
            //     // particleScript.setToNewAngle(particleScript.A - )
            // }
            float angleIterator = (angleDiff) * orbitingParticlesAngleLerpSpeed;
            
            // if(Mathf.Abs(targetAngle - particleScript.Angle) > 180){
            //     // angleIterator = -angleIterator;
            // }
            // targetAngle = particleScript.Angle + angleIterator;
            
            particleScript.SetToNewAngle(targetAngle);
            particleScript.Aligning = true;
            // Debug.Log($"targetAngle: {targetAngle}");
        }
    } 
    // else if(orientingParticles && !(Input.touchCount > 0) || Input.GetMouseButton(0)){
    //     orientingParticles = false;
    //         for(int i = 0; i < numOfOrbitingParticles; i++){
    //         OrbitingStaffParticleBehavior particleScript = orbitingParticles[i].GetComponent<OrbitingStaffParticleBehavior>();
    //         particleScript.Aligning = false;
    //     }
    // }

    // void CheckForInput()

    public void ResetTouch(){
        // if ((!controls.Gameplay.PrimaryAttack.triggered && Application.platform != RuntimePlatform.Android) || 
        //     (shootingJoystick.Horizontal == 0 && shootingJoystick.Vertical == 0))
        // // if ((!Input.GetMouseButton(0) && Application.platform != RuntimePlatform.Android) || 
        // //     (shootingJoystick.Horizontal == 0 && shootingJoystick.Vertical == 0))
        // {
        //     clicked = false;
        //     // Debug.Log("Clicked false");
        // } 

        
            clicked = false;
    }

    void UpdateParticleOrientation(){
        if(orientingParticles){
            particleOrientationDT += Time.deltaTime;

            if(particleOrientationDT >= particleOrientationTime){
                orientingParticles = false;
                particleOrientationDT = 0;

                for(int i = 0; i < numOfOrbitingParticles; i++){
                    OrbitingStaffParticle particleScript = orbitingParticles[i];
                    particleScript.Aligning = false;
                }
            }
        }
    }

    public void TransitionStaffActive(bool active){
        float radius = active ? orbitingParticlesRadius * scale : 0f;
        bool deactivate = !active;
        float delay = 0f;

        foreach(OrbitingStaffParticle orbitingStaffParticle in orbitingParticles){
            if(active){
                orbitingStaffParticle.gameObject.SetActive(true);
                // delay = .25f;
            }

            orbitingStaffParticle.TransitionToNewRadius(radius, .25f, delay, deactivate);
            // delay += .1f;
        }
    }

    public void ScaleTweenOrbitingParticles(float scale, float time, bool shouldResetOnFinish = true){
        for(var i = 0; i < orbitingParticles.Length; i++){
            orbitingParticles[i].ScaleTween(scale, time, shouldResetOnFinish);
        }
    }

    public void DetachFromStaff(){
        detached = true;
        sortingOrderByY.enabled = true;
        SuspendShooting = true;
        // rigidbody.simulated = true;
    }

    public void AttachToStaff(){
        detached = false;
        sortingOrderByY.enabled = false;
        returningToStaff = true;
    }

   void UpdateSortingLayer(){
       if(detached){
           return;
       }

       gameObject.GetComponent<SpriteRenderer>().sortingOrder = GameObject.Find("OldWizard").GetComponent<SpriteRenderer>().sortingOrder + 1;
   }
   
    public void Flip(bool flip){
        flipped = flip;
    }
}
