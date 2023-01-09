using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaffSpriteBehavior : MonoBehaviour
{
    private Color flameColor;
    public int numOfOrbitingParticles;
    public float orbitingParticlesAngleSeparation;
    public float orbitingParticlesRadius;
    public float orbitingParticleAngularSpeed;
    public GameObject orbitingParticlePrefab;
    public bool orbitingParticlesShouldRotate;
    public float orbitingParticlesAngleLerpSpeed;
    GameObject[] orbitingParticles;

    /*
    Need to implement particles staying at their rotation for a second or so after clicking before beginning
    their orbit again.
    */
    private bool orientingParticles;
    private float particleOrientationDT;
    private float particleOrientationTime;

    GameObject[] staffMainBeamParticles;
    int numOfMainBeamParticles;
    int staffMainBeamParticleIndex;
    public GameObject staffMainBeamParticlePrefab;
    
    private bool onCooldown;
    private float cooldownDT;
    private float cooldownTime;

    private bool clicked;
    private Vector3 clickPosition;
    private bool clickReset;

    private Vector3 mouseDifferenceUnitVector;

    [SerializeField] private float mainBeamSpeed = .25f;

    // Start is called before the first frame update
    void Start()
    {
        flameColor = GameObject.Find("OldWizard").GetComponent<WizardControls>().flameColor;
        flameColor.a = 1;
        GetComponent<SpriteRenderer>().color = flameColor;

        orbitingParticles = new GameObject[numOfOrbitingParticles];

        for(int i = 0; i < numOfOrbitingParticles; i++){
            orbitingParticles[i] = Instantiate(orbitingParticlePrefab);
            float angle = 0f - orbitingParticlesAngleSeparation / 2 + orbitingParticlesAngleSeparation * i;
            orbitingParticles[i].GetComponent<OrbitingStaffParticleBehavior>().initializeValues(angle, orbitingParticlesRadius, orbitingParticleAngularSpeed, gameObject, orbitingParticlesShouldRotate, orbitingParticlesAngleLerpSpeed, i, orbitingParticlesAngleSeparation);
            orbitingParticles[i].GetComponent<SpriteRenderer>().sortingLayerName = gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
            orbitingParticles[i].GetComponent<SpriteRenderer>().sortingOrder = 3;
            // Debug.Log($"orbitingParticlesShouldRotate: {orbitingParticlesShouldRotate}");
        }

        orientingParticles = false;


        staffMainBeamParticleIndex = 0;
        numOfMainBeamParticles = 25;
        staffMainBeamParticles = new GameObject[numOfMainBeamParticles];

        for(var i = 0; i < numOfMainBeamParticles; i++){
            staffMainBeamParticles[i] = Instantiate(staffMainBeamParticlePrefab);
            staffMainBeamParticles[i].GetComponent<StaffBeamMainBehavior>().initializeValues(flameColor, gameObject);
            staffMainBeamParticles[i].GetComponent<SpriteRenderer>().sortingLayerName = gameObject.GetComponent<SpriteRenderer>().sortingLayerName;
            staffMainBeamParticles[i].GetComponent<SpriteRenderer>().sortingOrder = 3;
            // staffMainBeamParticles[i].GetComponent<Rigidbody2D>().simulated = false;
            // Debug.Log(staffMainBeamParticles[i]);
        }

        clicked = false;
        onCooldown = false;
        cooldownDT = 0;
        cooldownTime = .5f;

        particleOrientationDT = 0;
        particleOrientationTime = .5f;

        clickReset = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForTouch();
        UpdateOrbitingParticleAngles();
        UpdateMainStaffBeam();
        UpdateCooldown();
        ResetTouch();
        UpdateSortingLayer();
    }

   
    void FixedUpdate()
    {

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

    void CheckForTouch()
    {
        if((clicked && !onCooldown) || onCooldown){
            return;
        }

        if ((Input.touchCount > 0) || Input.GetMouseButton(0))
        {
            // Debug.Log("Clicked");
            if(Input.GetMouseButton(0)){
                clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            } else {
                clickPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }

            clickPosition.z = 0;
            clicked = true;
        }
    }

    void UpdateMainStaffBeam(){
        if(!clicked || onCooldown){
            return;
        }

        while(staffMainBeamParticles[staffMainBeamParticleIndex].GetComponent<StaffBeamMainBehavior>().Active){
            if(++staffMainBeamParticleIndex >= numOfMainBeamParticles){
                staffMainBeamParticleIndex = 0;
            }
        }

        staffMainBeamParticles[staffMainBeamParticleIndex].transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        StaffBeamMainBehavior beamParticleScript = staffMainBeamParticles[staffMainBeamParticleIndex].GetComponent<StaffBeamMainBehavior>();
        beamParticleScript.Activate(transform.position.x, transform.position.y, 10f, mouseDifferenceUnitVector * mainBeamSpeed, flameColor);
        beamParticleScript.Speed = mainBeamSpeed;
        beamParticleScript.DistBetweenOrbitingParticles = Vector3.Distance(orbitingParticles[0].transform.position, orbitingParticles[1].transform.position);
        onCooldown = true;
    }

    void UpdateOrbitingParticleAngles(){
        if(!clicked || (clicked && (orientingParticles || onCooldown)) || !clickReset){
            return;
        }
        orientingParticles = true;

        Debug.Log(Camera.main.transform.position - transform.parent.gameObject.transform.position);
        // alteredClickPosition.z = 0;
        // Vector3 diffVector = alteredClickPosition - transform.position;
        Vector3 diffVector = clickPosition - transform.position;
        float angleToCrosshair = Mathf.Rad2Deg * (float)(Math.Atan2((double) diffVector.y, (double) diffVector.x));
        mouseDifferenceUnitVector = Vector3.Normalize(diffVector);
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
            OrbitingStaffParticleBehavior particleScript = orbitingParticles[i].GetComponent<OrbitingStaffParticleBehavior>();
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
    
        clickReset = false;
    } 
    // else if(orientingParticles && !(Input.touchCount > 0) || Input.GetMouseButton(0)){
    //     orientingParticles = false;
    //         for(int i = 0; i < numOfOrbitingParticles; i++){
    //         OrbitingStaffParticleBehavior particleScript = orbitingParticles[i].GetComponent<OrbitingStaffParticleBehavior>();
    //         particleScript.Aligning = false;
    //     }
    // }

    void ResetTouch(){
        if (!(Input.touchCount > 0) && !Input.GetMouseButton(0))
        {
            clicked = false;
            // Debug.Log("Clicked false");
            clickReset = true;
        } 

        if(orientingParticles){
            particleOrientationDT += Time.deltaTime;

            if(particleOrientationDT >= particleOrientationTime){
                orientingParticles = false;
                particleOrientationDT = 0;

                for(int i = 0; i < numOfOrbitingParticles; i++){
                    OrbitingStaffParticleBehavior particleScript = orbitingParticles[i].GetComponent<OrbitingStaffParticleBehavior>();
                    particleScript.Aligning = false;
                }
            }
        }
    }

   void UpdateSortingLayer(){
       gameObject.GetComponent<SpriteRenderer>().sortingOrder = GameObject.Find("OldWizard").GetComponent<SpriteRenderer>().sortingOrder + 1;
   }
}
