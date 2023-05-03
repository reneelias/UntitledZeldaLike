using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Tilemaps;

public class CameraBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    float minX_Bound;
    float maxX_Bound;
    float minY_Bound;
    float maxY_Bound;

    public Collider2D leftBound;
    public Collider2D rightBound;
    public Collider2D topBound;
    public Collider2D bottomBound;

    [SerializeField] Camera activeCamera;
    
    bool transitioning = false;

    private Bounds cameraBounds;
    public bool followObject = true;
    public GameObject objectToFollow;
    private Vector3 previousObjectPosition;
    [Header("Centered Follow")]
    public float cameraSpeed = .025f;
    [Header("Leading Follow")]
    public bool cameraLeads = true;
    [Range(0, 5f)] [SerializeField] float cameraLeadAmount = 3.5f;
    [Range(0, 1f)] public float smoothingTime = .25f;
    private Vector2 velocity;
    float originalOrthographicSize;
    Tween transitionPositionTween;
    public bool skipTransitionPositionTween;
    [SerializeField] float cameraOffsetX;
    [SerializeField] float cameraOffsetY;

    void Start()
    {
        // GetComponent<Camera>().orthographicSize = 3;
        // GetComponent<Camera>().orthographicSize = 10;
        // DOTween.To(()=> GetComponent<Camera>().orthographicSize, x=> GetComponent<Camera>().orthographicSize = x, 5, 5).SetEase(Ease.InOutSine);
        originalOrthographicSize = activeCamera.orthographicSize;
        UpdateBounds();
        float newX = Mathf.Clamp(objectToFollow.transform.position.x, minX_Bound, maxX_Bound);
        float newY = Mathf.Clamp(objectToFollow.transform.position.y, minY_Bound, maxY_Bound);
        Vector3 newPosition = new Vector3(newX, newY, transform.position.z);
        transform.position = newPosition;
        velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        FollowObject();
        FollowObjectLeading();
    }

    void FollowObject(){
        if(transitioning || cameraLeads || !followObject){
            return;
        }
        // float xAxisValue = Input.GetAxis("Horizontal") * cameraSpeed;
        // float zAxisValue = Input.GetAxis("Vertical") * cameraSpeed;
 
        // transform.position = new Vector3(transform.position.x + xAxisValue, transform.position.y, transform.position.z + zAxisValue);
        float originalZPosition = transform.position.z;
        float finalCameraSpeed = cameraSpeed;
        float cameraObjectDistance = Vector3.Distance(objectToFollow.transform.position, transform.position);
        
        finalCameraSpeed *= Math.Min(cameraObjectDistance, 1);
        

        float newX = Mathf.Clamp(transform.position.x + (objectToFollow.transform.position.x - transform.position.x + cameraOffsetX) * finalCameraSpeed, minX_Bound, maxX_Bound);
        float newY = Mathf.Clamp(transform.position.y + (objectToFollow.transform.position.y - transform.position.y + cameraOffsetY) * finalCameraSpeed, minY_Bound, maxY_Bound);
        Vector3 newPosition = new Vector3(newX, newY, originalZPosition);
        // Vector3 newPosition = transform.position + (objectToFollow.transform.position - transform.position) * finalCameraSpeed;
        // newPosition.z = originalZPosition;
        // transform.position += (objectToFollow.transform.position - transform.position) * cameraSpeed;
        transform.position = newPosition;
        // transform.position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, transform.position.z);
        // previousObjectPosition = objectToFollow.transform.position;
    }

    void FollowObjectLeading(){
        if(transitioning || !cameraLeads || !followObject){
            return;
        }
        
        // float finalCameraSpeed = cameraSpeed;
        // float cameraObjectDistance = Vector3.Distance(objectToFollow.transform.position, transform.position);
        
        // finalCameraSpeed *= Math.Min(cameraObjectDistance + cameraLeadAmount, 1);

        Vector2 objectVelocityAdditionVector = objectToFollow.GetComponent<WizardControls>().Velocity;
        objectVelocityAdditionVector = objectVelocityAdditionVector.normalized * cameraLeadAmount;

        if(objectVelocityAdditionVector.magnitude == 0f){
            return;
        }
        Vector3 objectCameraDiffVector = objectToFollow.transform.position - transform.position;
        
        Vector2 targetPoint = new Vector2(transform.position.x + objectCameraDiffVector.x + objectVelocityAdditionVector.x, 
            transform.position.y + objectCameraDiffVector.y + objectVelocityAdditionVector.y);

            
        Vector3 newPosition = Vector3.zero;
        newPosition.x = Mathf.Clamp(Mathf.SmoothDamp(transform.position.x, targetPoint.x, ref velocity.x, smoothingTime), minX_Bound, maxX_Bound);
        newPosition.y = Mathf.Clamp(Mathf.SmoothDamp(transform.position.y, targetPoint.y, ref velocity.y, smoothingTime), minY_Bound, maxY_Bound);
        newPosition.z = transform.position.z;
    
        // float newX = Mathf.Clamp(transform.position.x + ((objectToFollow.transform.position.x - transform.position.x) + objectVelocityAdditionVector.x) * finalCameraSpeed, minX_Bound, maxX_Bound);
        // float newY = Mathf.Clamp(transform.position.y + ((objectToFollow.transform.position.y - transform.position.y) + objectVelocityAdditionVector.y) * finalCameraSpeed, minY_Bound, maxY_Bound);
        

        // Vector3 newPosition = new Vector3(newX, newY, originalZPosition);
        
        transform.position = newPosition;
    }

    public void UpdateBounds(){
        if(!leftBound){
            return;
        }
        minX_Bound = leftBound.bounds.min.x + activeCamera.orthographicSize * activeCamera.aspect;
        maxX_Bound = rightBound.bounds.max.x - activeCamera.orthographicSize * activeCamera.aspect;
        minY_Bound = bottomBound.bounds.min.y + activeCamera.orthographicSize;
        maxY_Bound = topBound.bounds.max.y - activeCamera.orthographicSize;

        if(minX_Bound > maxX_Bound){
            minX_Bound = maxX_Bound = leftBound.bounds.min.x + (rightBound.bounds.max.x - leftBound.bounds.min.x) / 2f;
        }
        if(minY_Bound > maxY_Bound){
            minY_Bound = maxY_Bound = bottomBound.bounds.min.y + (topBound.bounds.max.y - bottomBound.bounds.min.y) / 2f;
        }

        
        cameraBounds = new Bounds(transform.position, new Vector3(activeCamera.orthographicSize * 2f * activeCamera.aspect, activeCamera.orthographicSize * 2f, 0f));
    }

    public Tween TransitionToNewPosition(Vector3 newPosition, float transitionTime){
        if(skipTransitionPositionTween){
            skipTransitionPositionTween = false;
            return null;
        }

        transitioning = true;
        return transform.DOMove(newPosition, transitionTime)
            // .OnUpdate(()=>{Debug.Log("transition camera to new position");})
            .OnComplete(()=> transitioning = false);
    }
    public void TransitionToNewRoom(Room room, float transitionTime, float roomCameraOffsetX, float roomCameraOffsetY){
        leftBound = room.leftCameraBound;
        rightBound = room.rightCameraBound;
        topBound = room.topCameraBound;
        bottomBound = room.bottomCameraBound;

        UpdateBounds();
        
        float newX = Mathf.Clamp(objectToFollow.transform.position.x, minX_Bound, maxX_Bound);
        float newY = Mathf.Clamp(objectToFollow.transform.position.y, minY_Bound, maxY_Bound);
        Vector3 newPosition = new Vector3(newX, newY, transform.position.z);
        TransitionToNewPosition(newPosition, transitionTime);

        cameraOffsetX = roomCameraOffsetX;
        cameraOffsetY = roomCameraOffsetY;
    }

    public void SetCameraOffset(float offsetX, float offsetY){
        cameraOffsetX = offsetX;
        cameraOffsetY = offsetY;
    }

    public void SetCameraOnPlayerPosition(){

        Vector3 newPosition = Vector3.zero;
        newPosition.x = Mathf.Clamp(objectToFollow.transform.position.x, minX_Bound, maxX_Bound);
        newPosition.y = Mathf.Clamp(objectToFollow.transform.position.y, minY_Bound, maxY_Bound);
        newPosition.z = transform.position.z;

        transform.position = newPosition;
        velocity = Vector2.zero;
    }

    public bool IsInCamera(Bounds otherBounds){
        cameraBounds.center = new Vector3(transform.position.x, transform.position.y, 0f);
        cameraBounds.size = new Vector3(activeCamera.orthographicSize * 2f * activeCamera.aspect, activeCamera.orthographicSize * 2f, 0f);
        otherBounds.center = new Vector3(otherBounds.center.x, otherBounds.center.y, 0f);

        return cameraBounds.Intersects(otherBounds);
    }

    public Tween ZoomInFocusObject(float newSize = 3f, float zoomTime = .5f){
        // activeCamera.orthographicSize = newSize;
        Tween zoomTween = activeCamera.DOOrthoSize(newSize, zoomTime);

        float temp_minX_Bound = leftBound.bounds.min.x + newSize * activeCamera.aspect;
        float temp_maxX_Bound = rightBound.bounds.max.x - newSize * activeCamera.aspect;
        float temp_minY_Bound = bottomBound.bounds.min.y + newSize;
        float temp_maxY_Bound = topBound.bounds.max.y - newSize;
        
        if(followObject){
            Vector3 targetPosition = Vector3.zero;
            targetPosition.x = Mathf.Clamp(objectToFollow.transform.position.x, temp_minX_Bound, temp_maxX_Bound);
            targetPosition.y = Mathf.Clamp(objectToFollow.transform.position.y, temp_minY_Bound, temp_maxY_Bound);
            targetPosition.z = transform.position.z;
            activeCamera.transform.DOMove(targetPosition, zoomTime);
        }
        transitioning = true;

        return zoomTween;
    }
    
    public Tween ZoomOutOriginalSize(float zoomTime = .5f){
        Tween zoomTween = activeCamera.DOOrthoSize(originalOrthographicSize, zoomTime)
            .OnUpdate(UpdateBounds)
            .OnComplete(()=>{transitioning = false;});
        
        transitioning = false;

        return zoomTween;
    }
}
