using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class MechanicalHatch : MonoBehaviour
{
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;
    Vector3 leftDoorPositionOG;
    Vector3 rightDoorPositionOG;
    [SerializeField] GameObject statusLight;
    [SerializeField] GameObject reflections;
    [SerializeField] GameObject belowGroundCollider;
    [SerializeField] Collider2D collisionCheckCollider;
    Tween leftOpenTween;
    Tween rightOpenTween;
    Tween leftCloseTween;
    Tween rightCloseTween;
    [SerializeField] float tweenDuration = .25f;
    float tweenDT = 0f;
    [SerializeField] float tweenDistance = .5f;
    [SerializeField] float openPauseDuration = 1f;
    [SerializeField] Color  lightIdleColor = Color.red;
    [SerializeField] Color lightAcceptColor = Color.green;
    bool open = false;
    List<GameObject> objectsInContact;
    [SerializeField] List<GameObject> unlockableObjects;

    // Start is called before the first frame update
    void Start()
    {
        leftDoorPositionOG = leftDoor.transform.position;   
        rightDoorPositionOG = rightDoor.transform.position;
        objectsInContact = new List<GameObject>();
        // unlockableObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        // UpdateContactCheck();
    }

    void UpdateContactCheck(){
        if(objectsInContact.Count == 0){
            return;
        }

        foreach(GameObject obj in objectsInContact){
            Skull skull = obj.GetComponent<Skull>();

            if(skull.Falling){
                foreach(GameObject unlockableObj in unlockableObjects){
                    unlockableObj.GetComponent<IUnlocklableObject>().Unlock();
                }
            }
        }
    }

    public void UnlockObjects(){
        foreach(GameObject unlockableObj in unlockableObjects){
            unlockableObj.GetComponent<IUnlocklableObject>().Unlock();
        }
    }

    void StartOpenSequence(){
        tweenDT = 0f;
        leftOpenTween = leftDoor.transform.DOMoveX(leftDoorPositionOG.x - tweenDistance, tweenDuration);
        rightOpenTween = rightDoor.transform.DOMoveX(rightDoorPositionOG.x + tweenDistance, tweenDuration)
            .OnUpdate(()=>{
                tweenDT += Time.deltaTime;
                if(tweenDT >= tweenDuration / 2f && !open){
                    open = true;
                    belowGroundCollider.SetActive(true);
                    collisionCheckCollider.enabled = false;
                }
            })
            .OnComplete(()=>{
                reflections.SetActive(false);
                DOVirtual.DelayedCall(openPauseDuration, StartCloseSequence);
            });

        statusLight.GetComponent<SpriteRenderer>().color = lightAcceptColor;
        statusLight.GetComponent<LightSprite2D>().color = lightAcceptColor;
    }

    void StartCloseSequence(){
        tweenDT = 0f;
        reflections.SetActive(true);
        leftCloseTween = leftDoor.transform.DOMoveX(leftDoorPositionOG.x, tweenDuration);
        rightCloseTween = rightDoor.transform.DOMoveX(rightDoorPositionOG.x, tweenDuration)
            .OnUpdate(()=>{
                tweenDT += Time.deltaTime;
                if(tweenDT >= tweenDuration / 2f && open){
                    open = false;
                    belowGroundCollider.SetActive(false);
                    collisionCheckCollider.enabled = true;
                }
            });

        statusLight.GetComponent<SpriteRenderer>().color = lightIdleColor;
        statusLight.GetComponent<LightSprite2D>().color = lightIdleColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.tag){
            case "Breakable":
                if(!objectsInContact.Contains(other.gameObject)){
                    objectsInContact.Add(other.gameObject);
                }

                if(open){
                    return;
                }

                StartOpenSequence();
            break;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
         
        switch(other.tag){
            case "Breakable":
                // if(objectsInContact.Contains(other.gameObject)){
                //     objectsInContact.Remove(other.gameObject);
                //     Skull skull = other.gameObject.GetComponent<Skull>();
                //     if(skull.Falling){
                //         Debug.Log("Skull falling");
                //     }
                // }
            break;
        }       
    }
}
