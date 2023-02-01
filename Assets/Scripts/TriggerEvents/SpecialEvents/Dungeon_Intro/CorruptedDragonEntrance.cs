using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class CorruptedDragonEntrance : EventTrigger
{
    [Header("Corrupted Dragon Entrance")]
    [SerializeField] CorruptedDragonPuppet corruptedDragonPuppet;
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    [SerializeField] GameObject leftArmParentObj;
    [SerializeField] GameObject rightArmParentObj;
    [SerializeField] GameObject headAndBody;
    [SerializeField] GameObject head;
    [SerializeField] GameObject body;
    [SerializeField] GameObject neckParent;
    [SerializeField] GameObject[] firstLightsShown;
    [SerializeField] float leftArmDelay = 2f;
    [SerializeField] float rightArmDelay = 2f;
    [SerializeField] float firstLightsDelay = 2f;
    [SerializeField] float headAndBodyDelay = 2f;
    [SerializeField] float fadeInDuration = 1f;
    [SerializeField] float moveUpDuration = 3f;
    [SerializeField] AudioClip shakeSound;
    [SerializeField] float shakeDuration = 1f;
    [SerializeField] float shakeStrength = .2f;
    [SerializeField] AudioClip dragonGrumble;
    Sequence fadeInSequence;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // headAndBody.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        head.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        body.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        // leftHand.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        // rightHand.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        foreach(GameObject lightObject in firstLightsShown){
            lightObject.GetComponent<Light2D>().color.a = 0f;
        }

        foreach(Transform armSubObj in leftArmParentObj.transform){
            armSubObj.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }

        foreach(Transform armSubObj in rightArmParentObj.transform){
            armSubObj.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }

        foreach(Transform neckSubObj in neckParent.transform){
            neckSubObj.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Trigger()
    {
        base.Trigger();

        // Vector3 headAndBodyOgPosition = headAndBody.transform.position;
        // headAndBody.transform.position = new Vector3(headAndBodyOgPosition.x, headAndBodyOgPosition.y - 1.25f, headAndBodyOgPosition.z);
        // fadeInSequence = DOTween.Sequence()
        //     .Append(leftHand.GetComponent<SpriteRenderer>().DOFade(1f, fadeInDuration).SetDelay(leftArmDelay))
        //         .InsertCallback(leftArmDelay, ()=>{
        //                 GameMaster.Instance.audioSource.PlayOneShot(shakeSound);
        //                 GameMaster.Instance.dungeon.ActiveCamera.DOShakePosition(shakeDuration, shakeStrength);
        //             })
        //     .Append(rightHand.GetComponent<SpriteRenderer>().DOFade(1f, fadeInDuration).SetDelay(rightArmDelay))
        //         .InsertCallback(leftArmDelay + rightArmDelay + fadeInDuration, ()=>{
        //                 GameMaster.Instance.audioSource.PlayOneShot(shakeSound);
        //                 GameMaster.Instance.dungeon.ActiveCamera.DOShakePosition(shakeDuration, shakeStrength);
        //             })
        //     .Append(DOVirtual.DelayedCall(firstLightsDelay, FadeInFirstLights))
        //     .Append(headAndBody.GetComponent<SpriteRenderer>().DOFade(1f, moveUpDuration).SetDelay(headAndBodyDelay))
        //     .Join(headAndBody.transform.DOMoveY(headAndBodyOgPosition.y, moveUpDuration)).SetEase(Ease.InOutSine)
        //         .InsertCallback(leftArmDelay + rightArmDelay + firstLightsDelay + fadeInDuration * 3f, ()=>{
        //                 GameMaster.Instance.audioSource.PlayOneShot(dragonGrumble, .75f);
        //             })
        //     .OnComplete(Finish);

        // float bodyTranslateY = 1.25f;
        // Vector3 headAndBodyOgPosition = headAndBody.transform.position;
        // headAndBody.transform.position = new Vector3(headAndBodyOgPosition.x, headAndBodyOgPosition.y - bodyTranslateY, headAndBodyOgPosition.z);
        // fadeInSequence = DOTween.Sequence()
        //     .Append(DOVirtual.DelayedCall(leftArmDelay, ()=>{
        //         FadeInArmObjects(leftArmParentObj, leftArmDelay);
        //     }))
        //         .InsertCallback(leftArmDelay, ()=>{
        //                 GameMaster.Instance.audioSource.PlayOneShot(shakeSound);
        //                 GameMaster.Instance.dungeon.ActiveCamera.DOShakePosition(shakeDuration, shakeStrength);
        //             })
        //     .Append(DOVirtual.DelayedCall(leftArmDelay + fadeInDuration, ()=>{
        //         FadeInArmObjects(rightArmParentObj, rightArmDelay);
        //     }))
        //         .InsertCallback(leftArmDelay + rightArmDelay + fadeInDuration, ()=>{
        //                 GameMaster.Instance.audioSource.PlayOneShot(shakeSound);
        //                 GameMaster.Instance.dungeon.ActiveCamera.DOShakePosition(shakeDuration, shakeStrength);
        //             })
        //     .Append(DOVirtual.DelayedCall(firstLightsDelay, FadeInFirstLights))
        //     .Append(headAndBody.GetComponent<SpriteRenderer>().DOFade(1f, moveUpDuration).SetDelay(headAndBodyDelay))
        //     .Join(headAndBody.transform.DOMoveY(headAndBodyOgPosition.y, moveUpDuration)).SetEase(Ease.InOutSine)
        //         .InsertCallback(leftArmDelay + rightArmDelay + firstLightsDelay + fadeInDuration * 3f, ()=>{
        //                 GameMaster.Instance.audioSource.PlayOneShot(dragonGrumble, .75f);
        //             })
        //     .OnComplete(Finish);

        float bodyTranslateY = 1f;
        // Vector3 bodyOgPosition = body.transform.position;
        // body.transform.position = new Vector3(bodyOgPosition.x, bodyOgPosition.y - bodyTranslateY, bodyOgPosition.z);
        // Vector3 headOgPosition = head.transform.position;
        // head.transform.position = new Vector3(headOgPosition.x, headOgPosition.y - bodyTranslateY, headOgPosition.z);

        fadeInSequence = DOTween.Sequence()
            .Append(DOVirtual.DelayedCall(leftArmDelay, ()=>{
                FadeInSubObjects(leftArmParentObj, fadeInDuration, leftArmDelay);
            }))
                .InsertCallback(leftArmDelay, ()=>{
                        GameMaster.Instance.audioSource.PlayOneShot(shakeSound);
                        GameMaster.Instance.dungeon.ActiveCamera.DOShakePosition(shakeDuration, shakeStrength);
                        ControlsManager.Instance.PlayControllerHaptics(.25f, .75f, .5f);
                    })
            .Append(DOVirtual.DelayedCall(leftArmDelay + fadeInDuration, ()=>{
                FadeInSubObjects(rightArmParentObj, fadeInDuration, rightArmDelay);
            }))
                .InsertCallback(leftArmDelay + rightArmDelay + fadeInDuration, ()=>{
                        GameMaster.Instance.audioSource.PlayOneShot(shakeSound);
                        GameMaster.Instance.dungeon.ActiveCamera.DOShakePosition(shakeDuration, shakeStrength);
                        ControlsManager.Instance.PlayControllerHaptics(.25f, .75f, .5f);
                    })
            .Append(DOVirtual.DelayedCall(firstLightsDelay, FadeInFirstLights))
            .Append(head.GetComponent<SpriteRenderer>().DOFade(1f, moveUpDuration).SetDelay(headAndBodyDelay))
            .Join(body.GetComponent<SpriteRenderer>().DOFade(1f, moveUpDuration).SetDelay(headAndBodyDelay))
            .Join(DOVirtual.DelayedCall(headAndBodyDelay, ()=>{
                FadeInSubObjects(neckParent, fadeInDuration, 0f);
            }))
            .Join(DOVirtual.DelayedCall(0f, ()=>{
                corruptedDragonPuppet.AnimateBody(new Vector3(0f, bodyTranslateY, 0f), moveUpDuration, 0f, true);
            }))
            .Join(DOVirtual.DelayedCall(0f, ()=>{
                corruptedDragonPuppet.AnimateHead(new Vector3(0f, bodyTranslateY, 0f), moveUpDuration, 0f);
            }))
                .InsertCallback(leftArmDelay + rightArmDelay + firstLightsDelay + fadeInDuration * 3f, ()=>{
                        GameMaster.Instance.audioSource.PlayOneShot(dragonGrumble, .75f);
                        ControlsManager.Instance.PlayControllerHaptics(.1f, .3f, 5f);
                    })
            .OnComplete(Finish);

        // corruptedDragonPuppet.AnimateArmMovement(ArmSide.Right, 180, corruptedDragonPuppet.MaxHandDistance * .6f, 2f, 0f);
        // corruptedDragonPuppet.AnimateArmMovement(ArmSide.Left, 0, corruptedDragonPuppet.MaxHandDistance * .6f, 2f, 0f);
    
        // bodyTranslateY = 1f;
        // corruptedDragonPuppet.AnimateBody(new Vector3(0f, bodyTranslateY, 0f), moveUpDuration, 0f, true);
        // corruptedDragonPuppet.AnimateHead(new Vector3(0f, bodyTranslateY, 0f), moveUpDuration, 0f);
    }

    void FadeInSubObjects(GameObject parentObj, float fadeDuration, float delay){
        for(int i = 0; i < parentObj.transform.childCount; i++){
            GameObject subObj = parentObj.transform.GetChild(i).gameObject;
            subObj.GetComponent<SpriteRenderer>().DOFade(1f, fadeInDuration);
        }
    }

    void FadeInFirstLights(){
        for(int i = 0; i < firstLightsShown.Length; i++){
            Light2D light = firstLightsShown[i].GetComponent<Light2D>();
            DOTween.To(()=> light.color.a, x=> light.color.a = x, 1f, fadeInDuration);
        }
        headAndBody.GetComponent<SpriteRenderer>().DOFade(.25f, fadeInDuration);
    }
}
