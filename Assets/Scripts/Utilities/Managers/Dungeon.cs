using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FunkyCode;
using DG.Tweening;

// public class Dungeon : StaticInstance<Dungeon>
public class Dungeon : MonoBehaviour
{
    [SerializeField] Camera activeCamera;
    public Camera ActiveCamera{
        protected set => activeCamera = value;
        get => activeCamera;
    }
    CameraBehavior cameraBehavior;
    public CameraBehavior Camera{
        get => cameraBehavior;
        protected set => cameraBehavior = value;
    }
    [SerializeField] Room initialRoom;
    public Room CurrentRoom{
        private set; get;
    }
    public float roomTransitionTime;
    [SerializeField] GameObject rooms;
    [SerializeField] int keyCount = 0;

    public int KeyCount{
        get{return keyCount;}
    }
    [SerializeField] TextMeshProUGUI keyCountText;
    public Color darknessColor;
    public Color darknessColor_Pass2;
    [SerializeField] bool useCustomDungeonDarkness = true;
    public bool UseCustomDarknessColor{
        get { return useCustomDungeonDarkness; }
    }
    float originalDarknessAmount;
    float originalDarknessAmount_Pass2;
    [SerializeField] int currentFloor = 1;
    public int CurrentFloor{
        get{ return currentFloor; } 
    }
    [Header("Dungeon Audio")]
    [SerializeField] bool playAmbientSound = true;
    [Range(0, 1f)] [SerializeField] float ambientVolume = 1f;
    [SerializeField] AudioClip ambientBackgroundSound;
    [SerializeField] AudioSource audioSource_Ambient;
    [SerializeField] bool playDungeonMusic = true;
    [Range(0, 1f)] [SerializeField] float musicVolume = 1f;
    [SerializeField] AudioClip dungeonMusic;
    [SerializeField] AudioSource audioSource_Music;
    [SerializeField] bool masterKeyAcquired = false;
    public bool MasterKeyAcquired{
        protected set => masterKeyAcquired = value;
        get => masterKeyAcquired;
    }
    [SerializeField] GameObject masterKeyUIObj;
    Room deathRoom;

    // Start is called before the first frame update
    void Start()
    {
        cameraBehavior = activeCamera.GetComponent<CameraBehavior>();
        ModifyKeyCount(0);
        InitialLoad();
    }

    public void InitialLoad(){
        CurrentRoom = initialRoom;
        CurrentRoom.ActivateRoom(true);

        foreach(Transform childRoom in rooms.transform){
            Room tempRoom = childRoom.gameObject.GetComponent<Room>();
            if(tempRoom == CurrentRoom){
                continue;
            }

            tempRoom.DeactivateRoom(true);
        }

        if(!useCustomDungeonDarkness){
            darknessColor = Lighting2D.DarknessColor;
            darknessColor_Pass2 = Lighting2D.LightmapPresets[1].darknessColor;
        }

        originalDarknessAmount = darknessColor.a;
        originalDarknessAmount_Pass2 = darknessColor.a;

        PlayDungeonMusic();

        if(playAmbientSound){
            audioSource_Ambient.clip = ambientBackgroundSound;
            audioSource_Ambient.loop = true;
            audioSource_Ambient.volume = ambientVolume;
            audioSource_Ambient.Play();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }

    public void SetRoom(Room room, bool initialRoomSet = false, float roomCameraOffsetX = 0, float roomCameraOffsetY = 0){
        CurrentRoom = room;

        cameraBehavior.leftBound = CurrentRoom.leftCameraBound;
        cameraBehavior.rightBound = CurrentRoom.rightCameraBound;
        cameraBehavior.topBound = CurrentRoom.topCameraBound;
        cameraBehavior.bottomBound = CurrentRoom.bottomCameraBound;

        cameraBehavior.UpdateBounds();
        if(!initialRoomSet){
            cameraBehavior.TransitionToNewRoom(room, room.RoomTransitionTime, roomCameraOffsetX, roomCameraOffsetY);
        } else {
            cameraBehavior.SetCameraOnPlayerPosition();
            cameraBehavior.SetCameraOffset(roomCameraOffsetX, roomCameraOffsetY);
        }
    }

    public void ModifyKeyCount(int amount, bool isMasterKey = false){
        if(isMasterKey){
            MasterKeyAcquired = true;
            masterKeyUIObj.SetActive(true);
        } else {
            keyCount += amount;
            keyCountText.text = $":{keyCount}";
        }
    }

    public void BossDoorUnlocked(){
        MasterKeyAcquired = false;
        masterKeyUIObj.SetActive(false);
    }

    public void PlayRoomMusic(AudioClip roomMusic, float volume = .5f){
        if(!playDungeonMusic){
            return;
        }

        audioSource_Music.Stop();
        audioSource_Music.clip = roomMusic;
        audioSource_Music.loop = true;
        audioSource_Music.volume = volume;
        audioSource_Music.Play();
    }

    public void PlayDungeonMusic(float delay = 0f){
        if(!playDungeonMusic || (audioSource_Music.clip == dungeonMusic && audioSource_Music.isPlaying)){
            return;
        }  

        audioSource_Music.Stop();
        DOVirtual.DelayedCall(delay, ()=>{
            audioSource_Music.clip = dungeonMusic;
            audioSource_Music.loop = true;
            audioSource_Music.volume = musicVolume;
            audioSource_Music.Play();
        });
    }

    public void PauseDungeonMusic(bool fadeout = true){
        if(fadeout){
            audioSource_Music.DOFade(0f, 1f)
                .OnComplete(()=>{audioSource_Music.Stop();});
        } else {
            audioSource_Music.Stop();
        }
    }

    public void PlayerDied(){
        deathRoom = CurrentRoom;
    }

    public void ResetAfterDeath(float resetDelay = 1f){
        // CharacterControls characterControls = GameMaster.Instance.Player.GetComponent<CharacterControls>();
        PlayableCharacter playableCharacter = GameMaster.Instance.Player.GetComponent<PlayableCharacter>();
        playableCharacter.Respawn(deathRoom.RespawnPosition, deathRoom.RespawnFloorLevel, deathRoom.RespawnPlayerDirection);
        
        CurrentRoom.DeactivateRoom();
        CurrentRoom.DeactivateAdjacentRooms();
        CurrentRoom.gameObject.SetActive(false);
        deathRoom.RespawnRoom.gameObject.SetActive(true);
        PauseDungeonMusic();
        DOVirtual.DelayedCall(resetDelay, ()=>{
            CurrentRoom.DeathRoomReset();
        });
    
        DOVirtual.DelayedCall(resetDelay * 1.5f, ()=>{
            CurrentRoom = deathRoom.RespawnRoom;
            // characterControls.SetPlayerDirection(deathRoom.RespawnPlayerDirection);
            CurrentRoom.ActivateRoom();
        });
    }

    public void SetDarknessValue(float darknessMultiplier){
        // darknessColor.a = originalDarknessAmount - GameMaster.Instance.DarknessSliderRange / 2f + GameMaster.Instance.DarknessSliderRange * darknessMultiplier;
        // CurrentRoom
    }
}
