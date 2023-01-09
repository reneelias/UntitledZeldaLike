using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    [SerializeField] Door connectingDoor;
    [SerializeField] Room room;
    [SerializeField] Camera activeCamera;
    // CameraBehavior cameraBehavior;
    [SerializeField] GameObject transitionPosition;
    public GameObject TransitionPosition{
        get { return transitionPosition; }
    }
    bool transitioning = false;
    bool activating = false;
    [SerializeField] GameObject player;
    [SerializeField] bool isLocked = false;

    public GameObject lockedDoorObject;
    public Sprite alternateLockSprite;
    [SerializeField] AudioClip unlockDoorAudioClip;
    [SerializeField] AudioClip lockDoorAudioClip;
    [SerializeField] DoorDirection doorDirection = DoorDirection.Down;
    public DoorDirection DoorDirection{
        get { return doorDirection; }
    }


    private bool initialSetup = true;
    // Start is called before the first frame update
    void Start()
    {
        if(!activeCamera){
            activeCamera = Camera.main;
        }

        // cameraBehavior = activeCamera.GetComponent<CameraBehavior>();
        if(!player){
            player = GameObject.FindWithTag("Player");
        }

        if(isLocked){
            LockDoor();
        } else {
            UnlockDoor();
        }

        if(alternateLockSprite != null){
            lockedDoorObject.GetComponent<SpriteRenderer>().sprite = alternateLockSprite;
        }

        initialSetup = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateRoom(float roomTransitionTime = 0f){
        transitioning = true;
        activating = true;
        room.ActivateRoom(false, roomTransitionTime);
    }

    private void OnTriggerEnter2D(Collider2D collider2D){
        if(collider2D.tag == "Player" && !transitioning && room.active)
        {
            // Debug.Log("We got you old man.");
            // Debug.Log("Transitioning to new room");
            transitioning = true;
            connectingDoor.ActivateRoom(room.RoomTransitionTime);
            room.DeactivateRoom();
            Vector3 nextTransitionPosition = connectingDoor.TransitionPosition.transform.position;
            // float xPositionModifier = .5f;
            float xPositionModifier = 0;
            if(connectingDoor.DoorDirection == DoorDirection.Left || connectingDoor.DoorDirection == DoorDirection.Right){
                nextTransitionPosition.y += -xPositionModifier;
            } 
            // else if(connectingDoor.DoorDirection == DoorDirection.Right){
            //     nextTransitionPosition.y += -xPositionModifier;
            // }

            player.GetComponent<CharacterControls>().TranslateToPosition(nextTransitionPosition, room.RoomTransitionTime, false);
            // cameraBehavior.TransitionToNewRoom(connectingDoor.room, room.RoomTransitionTime);
            // float placeholderX = 0;
            // DOTween.To(()=> placeholderX, x=> placeholderX = x, 1, room.RoomTransitionTime).OnComplete(()=>{
            //     transitioning = false;
            //     activating = false;
            //     Debug.Log("Just left old room");
            // });
        }
    }
    private void OnTriggerExit2D(Collider2D collider2D){
        if(collider2D.tag == "Player" && transitioning)
        {
            if(activating){
                transitioning = false;
                activating = false;
                // Debug.Log("transition about to complete");
            } else {
                transitioning = false;
                // room.active = false;
                // connectingDoorController.ActivateRoom();
                // Debug.Log("Exiting old room");
            }
        }
    }

    public void LockDoor(){
        lockedDoorObject.SetActive(true);
        
        if(!initialSetup){
            GameMaster.Instance.audioSource.PlayOneShot(lockDoorAudioClip);
        }
    }

    public void UnlockDoor(bool playAudio = true){
        lockedDoorObject.SetActive(false);

        if(!initialSetup && playAudio){
            GameMaster.Instance.audioSource.PlayOneShot(unlockDoorAudioClip);
        }
    }
}

public enum DoorDirection{
    Down,
    Up,
    Left,
    Right
}
