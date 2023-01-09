using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CorruptedDragonPuppet : MonoBehaviour
{
    [SerializeField] DragonArm leftArm;
    public DragonArm LeftArm{
        protected set => leftArm = value;
        get => leftArm;
    }
    [SerializeField] DragonArm rightArm;
    public DragonArm RightArm{
        protected set => rightArm = value;
        get => rightArm;
    }
    public float MaxHandDistance{
        get => leftArm.HandMaxDistFromCenter;
    }
    [SerializeField] GameObject head;
    public GameObject Head{
        protected set => head = value;
        get => head;
    }
    [SerializeField] GameObject body;
    public GameObject Body{
        protected set => body = value;
        get => body;
    }
    [SerializeField] GameObject neckAnchor_Position;
    [SerializeField] GameObject neckPiece_Anchor;
    public GameObject NeckPiece_Anchor{
        protected set => neckPiece_Anchor = value;
        get => neckPiece_Anchor;
    }
    [SerializeField] GameObject neckPiece_Long;
    public GameObject NeckPiece_Long{
        protected set => neckPiece_Long = value;
        get => neckPiece_Long;
    }
    float neckRawLength;
    [SerializeField] GameObject eyes_Normal;
    public GameObject Eyes_Normal{
        protected set => eyes_Normal = value;
        get => eyes_Normal;
    }
    [SerializeField] GameObject eyes_Biting;
    public GameObject Eyes_Biting{
        protected set => eyes_Biting = value;
        get => eyes_Biting;
    }
    [SerializeField] Sprite normalSprite;
    [SerializeField] Sprite bitingSprite;
    [SerializeField] HeadState headState;
    // Start is called before the first frame update
    void Start()
    {
        CalcualteNeckLength();
        SetHeadState(headState);
    }

    void CalcualteNeckLength(){
        Vector3 originalScale = neckPiece_Long.transform.localScale;
        neckPiece_Long.transform.localScale = Vector3.one;
        neckRawLength = neckPiece_Long.GetComponent<SpriteRenderer>().bounds.size.y;
        neckPiece_Long.transform.localScale = originalScale;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNeckPieces();
    }

    void UpdateNeckPieces(){
        neckPiece_Anchor.transform.position = neckAnchor_Position.transform.position;
        neckPiece_Long.transform.position = neckPiece_Anchor.transform.position;
        float neckAngle = Mathf.Atan2(head.transform.position.y - neckPiece_Anchor.transform.position.y, head.transform.position.x - neckPiece_Anchor.transform.position.x);
        neckAngle = neckAngle * Mathf.Rad2Deg + 90f;
        neckPiece_Anchor.transform.localEulerAngles = new Vector3(0f, 0f, neckAngle);
        neckPiece_Long.transform.localEulerAngles = new Vector3(0f, 0f, neckAngle);
        float lengthToHead = (head.transform.position - neckPiece_Anchor.transform.position).magnitude;
        neckPiece_Long.transform.localScale = new Vector3(1f, lengthToHead / neckRawLength, 1f);
    }

    public void AnimateArmMovement(ArmSide armSide, float handAngleFromCenter, float handDistFromCenter, float translationDuration, float delay, Ease ease = Ease.InOutQuad){
        DragonArm arm = armSide == ArmSide.Right ? rightArm : leftArm;

        arm.AnimateArmMovement(handAngleFromCenter, handDistFromCenter, translationDuration, delay);
    }

    public Tween AnimateBody(Vector3 moveVector, float duration, float delay, bool keepHandsFrozen = false, bool moveByDifference = true, Ease ease = Ease.InOutQuad){
        Vector3 targetPosition = moveByDifference ? body.transform.position + moveVector : moveVector;
        if(keepHandsFrozen){
            rightArm.SetHandFreeze();
            leftArm.SetHandFreeze();
        }

        return body.transform.DOMove(targetPosition, duration).SetDelay(delay).SetEase(ease)
            .OnComplete(()=>{
                if(keepHandsFrozen){
                    rightArm.SetHandFreeze(false);
                    leftArm.SetHandFreeze(false);
                }
            });


            
    }

    public Tween AnimateHead(Vector3 moveVector, float duration, float delay, bool moveByDifference = true, Ease ease = Ease.InOutQuad){
        Vector3 targetPosition = moveByDifference ? head.transform.position + moveVector : moveVector;

        return head.transform.DOMove(targetPosition, duration).SetDelay(delay).SetEase(ease);
    }

    public void FadeArms(float finalAlpha, float duration, float delay, Ease ease = Ease.Linear){

    }

    public void SetArmLayer(string layerName, int layerOrder){
        // leftArm.SetArmLayer(layerName, layerOrder);
    }

    public void SetHeadLayer(string layerName, int layerOrder){
        head.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
        head.GetComponent<SpriteRenderer>().sortingOrder = layerOrder;
    }

    public void SetHeadState(HeadState headState){
        this.headState = headState;
        switch(headState){
            case HeadState.Normal:
                head.GetComponent<SpriteRenderer>().sprite = normalSprite;
                eyes_Normal.SetActive(true);
                eyes_Biting.SetActive(false);
                break;
            case HeadState.Biting:
                head.GetComponent<SpriteRenderer>().sprite = bitingSprite;
                eyes_Normal.SetActive(false);
                eyes_Biting.SetActive(true);
                break;
        }
    }
}

public enum HeadState{
    Normal,
    Biting
}
