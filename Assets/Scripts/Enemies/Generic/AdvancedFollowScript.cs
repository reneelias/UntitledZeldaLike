using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedFollowScript : FollowScript
{
    bool previouslyRerouted;
    Vector3 previousPosition;
    float previousDirectionAngle;
    float previousAngleOffset;
    [SerializeField] float angleCheckIncrement;
    [SerializeField] float angleCorrectionIncrement;
    [SerializeField] float thresholdDistance;
    [SerializeField] float minimumMovementThreshold;
    [SerializeField] float scanAngleRange;
    [SerializeField] float maxScanDistance;
    [SerializeField] Collider2D collider;
    [SerializeField] GameObject alternateCollider;
    [SerializeField] Collider2D damageCollider;
    [SerializeField] bool useAlternateCollider;
    [SerializeField] CapsuleCollider2D capsuleCollider;
    [SerializeField] bool useCapsuleCollider = false;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        previouslyRerouted = false;
        previousPosition = Vector3.zero;
        previousDirectionAngle = 0f;
        previousAngleOffset = 0f;

        if(alternateCollider != null){
            Physics2D.IgnoreCollision(damageCollider, alternateCollider.GetComponent<Collider2D>());
        }
        if(useAlternateCollider){
            if(!useCapsuleCollider){
                collider = alternateCollider.GetComponent<Collider2D>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
/*
* Original UpdateVelocity() implementation
*/
    protected override void UpdateVelocity(Vector3 diffVector)
    {
        if(diffVector == null){
            return;
        }
        Vector3 velocityVector = diffVector;

        // int layerMask = 1 << 2;
        // layerMask = ~layerMask;
        RaycastHit2D[] raycastResults = new RaycastHit2D[2];
        Vector2 raycastDirection;
        if(shouldReturnToOrigin && !shouldFollow){
            raycastDirection = (originPosition - transform.position).normalized;
        } else {
            raycastDirection = (objectToFollow.transform.position - transform.position).normalized;
        }
        LayerMask layerMask = LayerMask.GetMask(new string[]{"Default", "Player", "EnemyBarrier", "Enemy", "Geometry Colliders", "Scenery"});
        if(useCapsuleCollider){
            capsuleCollider.Raycast(raycastDirection, raycastResults, Mathf.Infinity, layerMask);
        } else {
            collider.Raycast(raycastDirection, raycastResults, Mathf.Infinity, layerMask);
        }
        RaycastHit2D raycastObject = raycastResults[0].transform.gameObject.name == gameObject.name ? raycastResults[1] : raycastResults[0];
        hasLOS = true;
        float previousPositionDifference = Vector3.Distance(transform.position, previousPosition);

        if(raycastObject){
            // Debug.Log($"Raycast object tag: {raycastObject.transform.gameObject.tag}");
            string originalRaycastObjectTag = raycastObject.transform.gameObject.tag;
            if((originalRaycastObjectTag != objectToFollow.tag && (raycastObject.distance <= thresholdDistance || previousPositionDifference <= minimumMovementThreshold)) 
            || (originalRaycastObjectTag == objectToFollow.tag && previousPositionDifference <= minimumMovementThreshold)
            || (shouldReturnToOrigin && (raycastObject.distance <= thresholdDistance || previousPositionDifference <= minimumMovementThreshold))){

                // if(raycastObject.distance <= thresholdDistance){
                //     Debug.Log("raycastObject.distance <= thresholdDistance");
                // }
                Vector2 newVelocityDirection = Vector2.zero;
                bool newDirectionFound = false;
                float angleIncrement = angleCheckIncrement * Mathf.Deg2Rad;
                Vector2 tempRaycastDirection;
                float currDirectionAngle = Mathf.Atan2(raycastDirection.y, raycastDirection.x);
                float angleToFollowObject = Mathf.Atan2(diffVector.y, diffVector.x);
                
                // Debug.Log("rerouting");
                if(previouslyRerouted && previousPositionDifference <= minimumMovementThreshold){
                // if(previouslyRerouted && (raycastObject.distance <= thresholdDistance || previousPositionDifference <= minimumMovementThreshold)){
                    float angleIncrementSign = previousAngleOffset / Mathf.Abs(previousAngleOffset);
                    float newAngle = previousDirectionAngle + angleCorrectionIncrement * Mathf.Deg2Rad * angleIncrementSign;
                    tempRaycastDirection = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
                    newVelocityDirection = tempRaycastDirection;
                    newDirectionFound = true;
                    previousDirectionAngle = newAngle;
                    previousAngleOffset += (angleCorrectionIncrement * Mathf.Deg2Rad * angleIncrementSign);
                } else {
                    float currAngleOffset = 0f;
                    int loopNum = 1;
                    float tempAngle = 0f;

                    while(angleIncrement * loopNum < 2 * Mathf.PI){
                        currAngleOffset = angleIncrement * loopNum++;
                        tempAngle = currDirectionAngle + currAngleOffset;

                        tempRaycastDirection = new Vector2(Mathf.Cos(tempAngle), Mathf.Sin(tempAngle));
                        if(useCapsuleCollider){
                            capsuleCollider.Raycast(raycastDirection, raycastResults, Mathf.Infinity);
                        } else {
                            collider.Raycast(raycastDirection, raycastResults, Mathf.Infinity);
                        }
                        raycastObject = raycastResults[0];
                        
                        if((raycastObject.transform.gameObject.tag == objectToFollow.tag || raycastObject.transform.gameObject.tag == "Wall") || (raycastObject.transform.gameObject.tag != objectToFollow.tag && raycastObject.distance > thresholdDistance)){
                            newVelocityDirection = tempRaycastDirection;
                            newDirectionFound = true;
                            previousDirectionAngle = tempAngle;
                            previousAngleOffset = currAngleOffset;
                            break;
                        }

                        currAngleOffset *= -1;
                        tempAngle = currDirectionAngle + currAngleOffset;
                        
                        tempRaycastDirection = new Vector2(Mathf.Cos(tempAngle), Mathf.Sin(tempAngle));
                        if(useCapsuleCollider){
                            capsuleCollider.Raycast(raycastDirection, raycastResults, Mathf.Infinity);
                        } else {
                            collider.Raycast(raycastDirection, raycastResults, Mathf.Infinity);
                        }
                        raycastObject = raycastResults[0];


                        // if((raycastObject.transform.gameObject.tag == objectToFollow.tag)
                        // || (!newDirectionFound && raycastObject.transform.gameObject.tag != objectToFollow.tag && raycastObject.distance > thresholdDistance)
                        // || (newDirectionFound && Mathf.Abs(tempAngle - angleToFollowObject) < Mathf.Abs(previousDirectionAngle  - angleToFollowObject))){
                        if((raycastObject.transform.gameObject.tag == objectToFollow.tag || raycastObject.transform.gameObject.tag == "Wall") || (raycastObject.transform.gameObject.tag != objectToFollow.tag && raycastObject.distance > thresholdDistance)){
                            if(newDirectionFound){
                                Debug.Log("Going other direction instead");
                            }

                            newVelocityDirection = tempRaycastDirection;
                            newDirectionFound = true;
                            previousDirectionAngle = tempAngle;
                            previousAngleOffset = currAngleOffset;
                            break;
                        }

                        if(newDirectionFound){
                            break;
                        }
                    }
                    // Debug.Log($"loopNum: {loopNum-1}");
                }

                if(newDirectionFound){
                    velocityVector = newVelocityDirection;
                    previouslyRerouted = true;
                    // Debug.Log("Velocity Vector Changed");
                }
                
                if(originalRaycastObjectTag != objectToFollow.tag){
                    hasLOS = false;
                }
            } 
            // else if(raycastObject.transform.gameObject.tag != objectToFollow.tag && previousPositionDifference <= .05f){
            //     previouslyRerouted = true;
                
            // } 
            else {
                previouslyRerouted = false;
            }
        }
        /*
        If hasLOS and new position is close to old position, sweep 45 degrees each way and decide to turn left or right
        */
        // Debug.Log($"Position difference: {previousPositionDifference}");
        // Debug.Log($"hasLOS: {hasLOS}");
        previousPosition = transform.position;
        base.UpdateVelocity(velocityVector);
    }

    // protected override void UpdateVelocity(Vector3 diffVector)
    // {
    //     if(diffVector == null){
    //         return;
    //     }
    //     Vector3 velocityVector = diffVector;

    //     int layerMask = 1 << 2;
    //     layerMask = ~layerMask;
    //     Vector2 raycastDirection = (objectToFollow.transform.position - transform.position).normalized;
    //     RaycastHit2D raycastObject = Physics2D.Raycast(transform.position, raycastDirection, Mathf.Infinity, layerMask);
    //     hasLOS = true;
    //     float previousPositionDifference = Vector3.Distance(transform.position, previousPosition);

    //     if(raycastObject){
    //         // Debug.Log($"Raycast object tag: {raycastObject.transform.gameObject.tag}");
    //         string originalRaycastObjectTag = raycastObject.transform.gameObject.tag;
    //         if((originalRaycastObjectTag != objectToFollow.tag && (raycastObject.distance <= thresholdDistance || previousPositionDifference <= minimumMovementThreshold)) 
    //         || (originalRaycastObjectTag == objectToFollow.tag && previousPositionDifference <= minimumMovementThreshold)
    //         || (shouldReturnToOrigin && previousPositionDifference <= minimumMovementThreshold)){
    //         // if((originalRaycastObjectTag != objectToFollow.tag && (raycastObject.distance <= thresholdDistance || previousPositionDifference <= minimumMovementThreshold)) 
    //         // || (originalRaycastObjectTag == objectToFollow.tag && previousPositionDifference <= minimumMovementThreshold)
    //         // || (shouldFollow && previousPositionDifference <= minimumMovementThreshold)
    //         // ){
    //             Vector2 newVelocityDirection = Vector2.zero;
    //             bool newDirectionFound = false;
    //             float angleIncrement = angleCheckIncrement * Mathf.Deg2Rad;
    //             Vector2 tempRaycastDirection;
    //             float currDirectionAngle = Mathf.Atan2(raycastDirection.y, raycastDirection.x);
    //             float leftSideScore = 0f, rightSideScore = 0f;
                
    //             Debug.Log("rerouting");
    //             if(previouslyRerouted && previousPositionDifference <= minimumMovementThreshold){
    //                 Debug.Log("previously rerouted");
    //                 float angleIncrementSign = previousAngleOffset / Mathf.Abs(previousAngleOffset);
    //                 float newAngle = previousDirectionAngle + angleCorrectionIncrement * Mathf.Deg2Rad * angleIncrementSign;
    //                 tempRaycastDirection = new Vector2(Mathf.Cos(newAngle), Mathf.Sin(newAngle));
    //                 newVelocityDirection = tempRaycastDirection;
    //                 newDirectionFound = true;
    //                 previousDirectionAngle = newAngle;
    //                 previousAngleOffset += (angleCorrectionIncrement * Mathf.Deg2Rad * angleIncrementSign);
    //             } else {
    //                 float currAngleOffset = 0f;
    //                 int loopNum = 1;
    //                 float tempAngle = 0f;
    //                 float scanDist = maxScanDistance == -1 ? Mathf.Infinity : maxScanDistance;
    //                 float largestDeduction = 5f;
    //                 float smallestDeduction = 1f;
    //                 float deductionScaler = 1f;
    //                 bool firstValidAngleFound = false;
    //                 float firstValidLeftAngle = 0f;
    //                 float firstValidRightAngle = 0f;
    //                 float leftAngleOffset = 0f;
    //                 float rightAngleOffset = 0f;

    //                 while(angleIncrement * loopNum <= scanAngleRange * Mathf.Deg2Rad){
    //                     currAngleOffset = angleIncrement * loopNum++;
    //                     tempAngle = currDirectionAngle + currAngleOffset;

    //                     tempRaycastDirection = new Vector2(Mathf.Cos(tempAngle), Mathf.Sin(tempAngle));
    //                     raycastObject = Physics2D.Raycast(transform.position, tempRaycastDirection, scanDist, layerMask);
                        
    //                     if(raycastObject.transform.gameObject.tag != objectToFollow.tag && raycastObject.distance <= thresholdDistance * 1.5f){
    //                         if(raycastObject.distance != 0f){
    //                             leftSideScore -= (deductionScaler * Mathf.Clamp(thresholdDistance / raycastObject.distance, smallestDeduction, largestDeduction));
    //                         } else {
    //                             leftSideScore -= largestDeduction;
    //                         }
    //                     }
    //                     if(!firstValidAngleFound && (raycastObject.transform.gameObject.tag == objectToFollow.tag || raycastObject.distance > thresholdDistance)){
    //                         firstValidAngleFound = true;
    //                         firstValidLeftAngle = tempAngle;
    //                         leftAngleOffset = currAngleOffset;
    //                     }
    //                 }
                    
    //                 currAngleOffset = 0f;
    //                 loopNum = 1;
    //                 tempAngle = 0f;
    //                 firstValidAngleFound = false;

    //                 while(Mathf.Abs(angleIncrement * loopNum) <= scanAngleRange * Mathf.Deg2Rad){
    //                     currAngleOffset = angleIncrement * loopNum++;
    //                     tempAngle = currDirectionAngle - currAngleOffset;

    //                     tempRaycastDirection = new Vector2(Mathf.Cos(tempAngle), Mathf.Sin(tempAngle));
    //                     raycastObject = Physics2D.Raycast(transform.position, tempRaycastDirection, scanDist, layerMask);
                        
    //                     if(raycastObject.transform.gameObject.tag != objectToFollow.tag && raycastObject.distance <= thresholdDistance * 1.5f){
    //                         if(raycastObject.distance != 0f){
    //                             leftSideScore -= (deductionScaler * Mathf.Clamp(thresholdDistance / raycastObject.distance, smallestDeduction, largestDeduction));
    //                         } else {
    //                             leftSideScore -= largestDeduction;
    //                         }
    //                     }
    //                     if(!firstValidAngleFound && (raycastObject.transform.gameObject.tag == objectToFollow.tag || raycastObject.distance > thresholdDistance)){
    //                         firstValidAngleFound = true;
    //                         firstValidRightAngle = tempAngle;
    //                         rightAngleOffset = currAngleOffset;
    //                     }
    //                 }

                    
    //                 tempAngle = rightSideScore > leftSideScore ? firstValidRightAngle : firstValidLeftAngle;
    //                 tempRaycastDirection = new Vector2(Mathf.Cos(tempAngle), Mathf.Sin(tempAngle));
    //                 newVelocityDirection = tempRaycastDirection;
    //                 newDirectionFound = true;
    //                 previousDirectionAngle = tempAngle;
    //                 previousAngleOffset = rightSideScore > leftSideScore ? rightAngleOffset : leftAngleOffset;
    //                 // Debug.Log($"loopNum: {loopNum-1}");
    //             }
    //             if(newDirectionFound){
    //                 velocityVector = newVelocityDirection;
    //                 previouslyRerouted = true;
    //                 // Debug.Log("Velocity Vector Changed");
    //             }
                
    //             if(originalRaycastObjectTag != objectToFollow.tag){
    //                 hasLOS = false;
    //             }
    //         } 
    //         // else if(raycastObject.transform.gameObject.tag != objectToFollow.tag && previousPositionDifference <= .05f){
    //         //     previouslyRerouted = true;
                
    //         // } 
    //         else {
    //             previouslyRerouted = false;
    //         }
    //     }
    //     /*
    //     If hasLOS and new position is close to old position, sweep 45 degrees each way and decide to turn left or right
    //     */
    //     // Debug.Log($"Position difference: {previousPositionDifference}");
    //     // Debug.Log($"hasLOS: {hasLOS}");
    //     previousPosition = transform.position;
    //     base.UpdateVelocity(velocityVector);
    // }
}
