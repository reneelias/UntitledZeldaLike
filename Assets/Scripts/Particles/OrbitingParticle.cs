using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class OrbitingParticle : MonoBehaviour
{
    protected float angle;
    public float Angle{
        get{return angle;}
    }
    float radius;
    float radianAngle;
    public BoxCollider2D boxCollider;
    public bool shouldRotate;
    public GameObject orbitObject;
    private Vector3 orbitPosition;
    public float angularSpeed = 1f;
    protected bool calculateOrbitAtStart = true;
    Tween radiusChangeTween;
    [SerializeField] SpriteRenderer spriteRenderer;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        Debug.Log($"Rotating block start: {gameObject.name}");
        if(calculateOrbitAtStart){
            if(orbitObject){
                orbitPosition = new Vector3(orbitObject.transform.position.x, orbitObject.transform.position.y, orbitObject.transform.position.z);
            } else {
                orbitPosition = Vector3.zero;
            }

            radianAngle = (float)Math.Atan2(this.transform.position.y - orbitPosition.y, this.transform.position.x - orbitPosition.x);
            angle = (float) ((180/Math.PI) * radianAngle);

            
        }
        
        transform.Rotate(0.0f, 0.0f, angle, Space.Self);

        radius = (float)Math.Sqrt(Math.Pow(this.transform.position.x - orbitPosition.x, 2) + Math.Pow(this.transform.position.y - orbitPosition.y, 2));
        // int[,] gameGrid = new int[,]{{0,0}, {2,2}};
    }

    public void initializeValues(float angle, float radius, float angularSpeed, GameObject orbitObject, bool shouldRotate){
        // Debug.Log("initializing values");
        this.angle = angle;
        this.radianAngle = angle * Mathf.Deg2Rad;
        this.radius = radius;
        this.angularSpeed = angularSpeed;
        this.orbitObject = orbitObject;
        this.shouldRotate = shouldRotate;
        if(!shouldRotate){
            transform.position = new Vector3((float)Math.Cos(radianAngle), (float)Math.Sin(radianAngle), 0) * radius + orbitObject.transform.position;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // CheckForTouch();
        UpdatePosition();
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        UpdateAngle();
    }

    void UpdateAngle()
    {
        if(shouldRotate){
            angle += angularSpeed;
        }
    }
    void UpdatePosition()
    {
        GameObject oldWizard = GameObject.Find("OldWizard");
        Vector3 orbitObjectVelocity = new Vector3(oldWizard.GetComponent<Rigidbody2D>().velocity.x, oldWizard.GetComponent<Rigidbody2D>().velocity.y, 0);
        orbitObjectVelocity = Vector3.zero;

        if(shouldRotate){ 
            if(orbitObject){
                orbitPosition = new Vector3(orbitObject.transform.position.x, orbitObject.transform.position.y, orbitObject.transform.position.z);
            } else {
                orbitPosition = Vector3.zero;
            }
            radianAngle = angle * Mathf.Deg2Rad;
            transform.position = new Vector3((float)Math.Cos(radianAngle), (float)Math.Sin(radianAngle), 0) * radius + orbitPosition + orbitObjectVelocity;
            // this.transform.Rotate(0.0f, 0.0f, angle, Space.World);
            // this.transform.rotation.Set(0f, 0f, angle, 0f);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        } else {
            if(orbitObject){
                radianAngle = angle * Mathf.Deg2Rad;
                transform.position = new Vector3((float)Math.Cos(radianAngle), (float)Math.Sin(radianAngle), 0) * radius + orbitObject.transform.position + orbitObjectVelocity;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    public virtual void TransitionToNewRadius(float newRadius, float transitionDuration, float delay = 0f, bool deactivateAfterCompletion = false){
        if(radiusChangeTween != null && radiusChangeTween.IsPlaying()){
            radiusChangeTween.Kill();
        }
        
        Ease ease = deactivateAfterCompletion ? Ease.InBack : Ease.InOutQuad;
        if(!deactivateAfterCompletion && spriteRenderer.enabled == false){
            spriteRenderer.enabled = true;
        }

        radiusChangeTween = DOTween.To(()=> radius, x=> radius = x, newRadius, transitionDuration)
            .SetEase(ease)
            .SetDelay(delay)
            .OnComplete(()=>{
                if(deactivateAfterCompletion){
                    // gameObject.SetActive(false);
                    spriteRenderer.enabled = false;
                }
            });
    }

    bool CheckForTouch()
    {
        if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0))
        {
            Vector3 wp;
            if(Input.GetMouseButtonDown(0)){
                wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            } else {
                wp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }

            var touchPosition = new Vector2(wp.x, wp.y);

            if (boxCollider == Physics2D.OverlapPoint(touchPosition)){
                // Debug.Log("HIT!");
                shouldRotate = !shouldRotate;
                return true;
            }
            else{
                // Debug.Log("MISS");
            }
        }
        return false;
    }
}
