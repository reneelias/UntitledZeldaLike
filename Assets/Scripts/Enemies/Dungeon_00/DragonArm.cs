using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class DragonArm : MonoBehaviour
{
    [SerializeField] ArmSide armSide = ArmSide.Right;
    [SerializeField] GameObject hand;
    [SerializeField] GameObject lowerArm;
    [SerializeField] GameObject upperArm;
    [SerializeField] float pivotCompensation = .1f;
    [SerializeField] float armPieceLength = 96f;
    float armPieceLengthEffective;
    [SerializeField] bool automaticallyCalculateArmLength = true;
    [SerializeField][Range(0f, .5f)] float handMinDistToCenter = .1f;
    float maxArmAngle = 90f;
    float handDistFromCenter;
    float handMaxDistFromCenter;
    public float HandMaxDistFromCenter{
        protected set => handMaxDistFromCenter = value;
        get => handMaxDistFromCenter;
    }
    [SerializeField] bool attachLowerArmToBody = true;
    [SerializeField] GameObject lowerArmAttachmentObj;
    [SerializeField] bool useStartPositionOnLoad = true;
    [SerializeField] bool freezeHands = false;
    Vector3 handFreezePosition;
    float handFreezeAngle;
    float handAngleFromCenter = 0f;
    [SerializeField] float handMaxAngleFromCenter = 90f;

    [Header("Debug")]
    [SerializeField] bool debugMode = false;
    [SerializeField] float handStartAngleFromCenter = 0f;
    [SerializeField] float angleChangeSpeed = 1f;
    [SerializeField] float handDistanceChangeSpeed = .05f;
    
    // Start is called before the first frame update
    void Start()
    {
        if(automaticallyCalculateArmLength){
            // Vector3 originalAngles = lowerArm.transform.eulerAngles;
            // lowerArm.transform.localEulerAngles = Vector3.zero;
            armPieceLength = lowerArm.GetComponent<SpriteRenderer>().bounds.size.x;
            // lowerArm.transform.localEulerAngles = originalAngles;
        }
        armPieceLengthEffective = armPieceLength - 2f * armPieceLength * pivotCompensation;

        handMaxDistFromCenter = 2f * armPieceLengthEffective * .9999f;
        UpdateLowerArmPosition();
        if(useStartPositionOnLoad){
            handAngleFromCenter = Mathf.Atan2(hand.transform.position.y - lowerArm.transform.position.y, hand.transform.position.x - lowerArm.transform.position.x) * Mathf.Rad2Deg;
            handDistFromCenter = (hand.transform.position - lowerArm.transform.position).magnitude;
            if(handDistFromCenter > handMaxDistFromCenter){
                handDistFromCenter = handMaxDistFromCenter;
                UpdateHandPosition();
            }
        } else {
            handAngleFromCenter = handStartAngleFromCenter;
            handDistFromCenter = handMaxDistFromCenter;
        }

        if(debugMode){
            hand.transform.position = lowerArm.transform.position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * handAngleFromCenter) * handDistFromCenter, Mathf.Sin(Mathf.Deg2Rad * handAngleFromCenter) * handDistFromCenter, 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLowerArmPosition();
        UpdateHandFreeze();
        UpdateHandPositionDebug();
        UpdateHand();
        UpdateArmPieceAngles();
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate()
    {
    }

    void UpdateHandPositionDebug(){
        if(!debugMode){
            return;
        }

        handDistFromCenter += handDistanceChangeSpeed;
        if(handDistFromCenter >= handMaxDistFromCenter){
            handDistanceChangeSpeed *= -1f;
            handDistFromCenter = handMaxDistFromCenter;
        }
        if(handDistFromCenter <= handMinDistToCenter){
            handDistanceChangeSpeed *= -1f;
            handDistFromCenter = handMinDistToCenter;
        }

        // handDistFromCenter = handMaxDistFromCenter * .98f;
        hand.transform.position = lowerArm.transform.position + new Vector3(Mathf.Cos(handAngleFromCenter * Mathf.Deg2Rad) * handDistFromCenter, Mathf.Sin(handAngleFromCenter * Mathf.Deg2Rad) * handDistFromCenter, 0f);
        // hand.transform.localEulerAngles = new Vector3(0f, 0f, handAngleFromCenter + 90f);
        // UpdateArmPieceAngles();
    }

    void UpdateHand(){
        if(freezeHands){
            return;
        }

        handAngleFromCenter = Mathf.Atan2(hand.transform.position.y - lowerArm.transform.position.y, hand.transform.position.x - lowerArm.transform.position.x) * Mathf.Rad2Deg;
        handDistFromCenter = (hand.transform.position - lowerArm.transform.position).magnitude;
    }

    void UpdateLowerArmPosition(){
        if(!attachLowerArmToBody){
            return;
        }

        lowerArm.transform.position = lowerArmAttachmentObj.transform.position;
    }

    float prevAngle = 0f;
    void UpdateArmPieceAngles(){
        float lowerArmAngle = 0f;
        float upperArmAngle = 0f;

        // if(handAngleFromCenter % 180f == 0f){
        //     lowerArmAngle = handAngleFromCenter;
        // } else {
            lowerArmAngle = Mathf.Acos((Mathf.Pow(armPieceLengthEffective, 2f) + Mathf.Pow(handDistFromCenter, 2f) - Mathf.Pow(armPieceLengthEffective, 2f)) / (2f * armPieceLengthEffective * handDistFromCenter)) * Mathf.Rad2Deg;
        // }
        // Debug.Log($"lowerArmAngle: {lowerArmAngle}");
        // lowerArmAngle = Mathf.Acos(Mathf.Clamp((Mathf.Pow(armPieceLengthEffective, 2f) + Mathf.Pow(handDistFromCenter, 2f) - Mathf.Pow(armPieceLengthEffective, 2f) / (2f * armPieceLengthEffective * handDistFromCenter)) * Mathf.Rad2Deg, .001f, .99f));
        upperArmAngle = -lowerArmAngle;

        if(armSide == ArmSide.Right){
            lowerArmAngle *= -1f;
            upperArmAngle *= -1f;
            lowerArmAngle += 180f;
            upperArmAngle += 180f;
        }

        // if(armSide == ArmSide.Right){
        //     lowerArmAngle = maxArmAngle + maxArmAngle * (handDistFromCenter / handMaxDistFromCenter);
        //     // upperArmAngle = maxArmAngle * 3f - maxArmAngle * (handDistFromCenter / handMaxDistFromCenter);
        //     upperArmAngle = -maxArmAngle - maxArmAngle * (handDistFromCenter / handMaxDistFromCenter);
        // } else {
        //     lowerArmAngle = maxArmAngle - maxArmAngle * (handDistFromCenter / handMaxDistFromCenter);
        //     upperArmAngle = -(maxArmAngle - maxArmAngle * (handDistFromCenter / handMaxDistFromCenter));
        // }

        lowerArmAngle += handAngleFromCenter;
        upperArmAngle += handAngleFromCenter;

        // if(armSide == ArmSide.Left){
        //     Debug.Log($"Current angle: {lowerArmAngle} / Delta arm angle: {prevAngle - lowerArmAngle}");
        // }
        prevAngle = lowerArmAngle;
        
        lowerArm.transform.localEulerAngles = new Vector3(0f, 0f, lowerArmAngle);
        upperArm.transform.localEulerAngles = new Vector3(0f, 0f, upperArmAngle);

        if(armSide == ArmSide.Right){
            upperArm.transform.position = new Vector3(lowerArm.transform.position.x - Mathf.Cos(lowerArmAngle * Mathf.Deg2Rad) * (armPieceLength - armPieceLength * pivotCompensation * 2f), lowerArm.transform.position.y - Mathf.Sin(lowerArmAngle * Mathf.Deg2Rad) * (armPieceLength - armPieceLength * pivotCompensation * 2f), 0f);
        } else {
            upperArm.transform.position = lowerArm.transform.position + new Vector3(Mathf.Cos(lowerArmAngle * Mathf.Deg2Rad) * (armPieceLength - armPieceLength * pivotCompensation * 2f), Mathf.Sin(lowerArmAngle * Mathf.Deg2Rad) * (armPieceLength - armPieceLength * pivotCompensation * 2f), 0f);
        }
    }

    void UpdateHandFreeze(){
        if(!freezeHands){
            return;
        }

        float distFromCenter = (handFreezePosition - lowerArm.transform.position).magnitude;
        if(distFromCenter > handMaxDistFromCenter && Mathf.Abs(distFromCenter - handMaxDistFromCenter) > .0005f){
            handFreezePosition = lowerArm.transform.position + new Vector3(Mathf.Cos(handFreezeAngle * Mathf.Deg2Rad) * handMaxDistFromCenter, Mathf.Sin(handFreezeAngle * Mathf.Deg2Rad) * handMaxDistFromCenter, 0f);
            SetHandPosition(handFreezePosition);
        } else {
            SetHandPosition(handFreezePosition);
            handAngleFromCenter = Mathf.Atan2(hand.transform.position.y - lowerArm.transform.position.y, hand.transform.position.x - lowerArm.transform.position.x) * Mathf.Rad2Deg;
            handFreezeAngle = handAngleFromCenter;
            handDistFromCenter = (hand.transform.position - lowerArm.transform.position).magnitude;
        }

    }

    public void AnimateArmMovement(float handAngleFromCenter, float handDistFromCenter, float translationDuration, float delay, Ease ease = Ease.InOutQuad){
        float handDistTarget = handDistFromCenter > handMaxDistFromCenter ? handMaxDistFromCenter : handDistFromCenter;
        DOTween.To(()=> this.handDistFromCenter, x => this.handDistFromCenter = x, handDistTarget, translationDuration).SetDelay(delay).SetEase(ease);
        DOTween.To(()=> this.handAngleFromCenter, x => this.handAngleFromCenter = x, handAngleFromCenter, translationDuration).SetDelay(delay).SetEase(ease)
            .OnUpdate(UpdateHandPosition);
    }

    void UpdateHandPosition(){
        hand.transform.position = lowerArm.transform.position + new Vector3(Mathf.Cos(handAngleFromCenter * Mathf.Deg2Rad) * handDistFromCenter, Mathf.Sin(handAngleFromCenter * Mathf.Deg2Rad) * handDistFromCenter, 0f);
    }

    public void SetHandPosition(Vector3 position){
        hand.transform.position = position;
    }

    public void SetHandFreeze(bool freeze = true){
        freezeHands = freeze;

        if(freezeHands){
            handFreezePosition = hand.transform.position;
        }
    }

    public void SetArmLayer(string layerName, int layerOrder){
        upperArm.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
        upperArm.GetComponent<SpriteRenderer>().sortingOrder = layerOrder;

        lowerArm.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
        lowerArm.GetComponent<SpriteRenderer>().sortingOrder = layerOrder - 1;
    }

    public void SetArmLayerRelativeHand(){
        lowerArm.GetComponent<SpriteRenderer>().sortingLayerName = hand.GetComponent<SpriteRenderer>().sortingLayerName;
        lowerArm.GetComponent<SpriteRenderer>().sortingOrder = hand.GetComponent<SpriteRenderer>().sortingOrder - 2;

        upperArm.GetComponent<SpriteRenderer>().sortingLayerName = hand.GetComponent<SpriteRenderer>().sortingLayerName;
        upperArm.GetComponent<SpriteRenderer>().sortingOrder = hand.GetComponent<SpriteRenderer>().sortingOrder - 1;
    }

    // /// <summary>
    // /// Callback to draw gizmos that are pickable and always drawn.
    // /// </summary>
    // void OnDrawGizmos()
    // {

    // }
}

public enum ArmSide{
    Right,
    Left
}