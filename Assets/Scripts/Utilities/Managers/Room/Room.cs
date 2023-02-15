using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FunkyCode;

public class Room : MonoBehaviour
{
    public Collider2D leftCameraBound;
    public Collider2D rightCameraBound;
    public Collider2D topCameraBound;
    public Collider2D bottomCameraBound;
    public Door[] doors;

    public GameObject roomBounds;
    [SerializeField] GameObject enemies;

    public float RoomTransitionTime{
        get; private set;
    }

    public string Name{
        get; private set;
    }
    [SerializeField] GameObject allRooms;
    [SerializeField] List<Room> adjacentRooms;
    bool objectsUnlocked = false;
    [SerializeField] GameObject[] unlockableObjects;
    [SerializeField] GameObject[] unlockingEnemies;
    [SerializeField] GameObject[] unlockingPuzzleObjects;
    [SerializeField] GameObject[] unlockObjectsToActivate;
    public bool active = false;
    [SerializeField] bool lockDoorsOnFirstEntry = false;
    bool firstEntry = true;
    [Header("Death Room Reset")]
    [Tooltip("If player dies in room before completeing macro objective, room reverts to original state.")]
    [SerializeField] bool roomCompleteResetOnDeath = false;
    bool roomResetObjectiveComplete = false;
    [Tooltip("Room reset objective is the same as item/door unlocking objective.")]
    [SerializeField] bool roomResetSharesUnlockables = true;
    [SerializeField] GameObject[] roomResetRespawnableItems;
    [SerializeField] GameObject[] roomResetObjects;
    [SerializeField] GameObject[] roomResetEnemies;
    [SerializeField] GameObject[] roomResetPuzzleObjects;
    [SerializeField] AudioClip unlockSound;
    [Header("Lighting")]
    [SerializeField] bool useGlobalDarkness = true;
    [SerializeField] Color darknessColor;
    [SerializeField] Color darknessColor_Pass2 = new Color(0f, 0f, 0f, .933f);
    [SerializeField] protected bool lightsAffectDarknessColor = false;
    public bool LightsAffectDarknessColor{
        get { return lightsAffectDarknessColor; }
    }
    [Header("Lightning Lights")]
    [SerializeField] Light2D[] lightsToFlash;
    float lightningTime;
    float lightningDT;
    [SerializeField] float lightningTimeMax = 1.5f;
    [SerializeField] float lightningTimeMin = .5f;
    bool lightningFlashing = false;
    [SerializeField] float lightningFlashingTime = .5f;
    float lightningFlashingDT = 0f;
    Dictionary<Light2D, float> lightsToFlashAlphaDict;
    float randomLightningFlashingTime;
    float randomLightningFlashingDT = 0f;
    [SerializeField] bool lightningBrightensRoom = false;
    [Range(0f, 255f)][SerializeField] int lightningBrighteningAlpha = 245;
    [SerializeField] bool playRainSound = false;
    [SerializeField] bool playThunderSound = false;
    [SerializeField] AudioClip thunderSound;
    [SerializeField] AudioClip rainLoop;
    [SerializeField] AudioSource roomAudioSource;
    float thunderSoundDT = 0f;
    [SerializeField] bool unlockDebug = false;
    [SerializeField] bool useDefaultLastOnGroundPosition = false;
    public bool UseDefaultLastOnGroundPosition{
        get => useDefaultLastOnGroundPosition;
        protected set => useDefaultLastOnGroundPosition = value;
    }
    [SerializeField] Vector3 defaultLastOnGroundPosition;
    public Vector3 DefaultLastOnGroundPosition{
        get => defaultLastOnGroundPosition;
        protected set => defaultLastOnGroundPosition = value; 
    }
    [Header("Camera")]
    [SerializeField] bool useCameraRoomOffset = false;
    [SerializeField] float cameraOffsetX = 0f;
    [SerializeField] float cameraOffsetY = 0f;
    [SerializeField] bool roomShakes = false;
    [SerializeField] bool periodShake = true;
    [SerializeField] float timeBetweenShakes = 3f;
    [SerializeField] float shakeDuration = 1f;
    float shakeDT = 0f;
    float timeLastRespawned;
    [SerializeField] float respawnItemsDuration = 60f;
    [SerializeField] GameObject[] respawnableItems;
    bool firstRoomActivation = true;
    bool canResetLastTimeRespawned = true;
    List<GameObject> objsToDespawn;
    [Header("Music")]
    [SerializeField] protected bool pauseDungeonMusicOnEntrance = false;
    [SerializeField] protected bool playRoomMusicOnEntrance = false;
    [SerializeField] protected bool playRoomMusicOnEnemyEngage = false;
    [SerializeField] protected AudioClip roomMusic;
    [Range(0, 1f)] [SerializeField] protected float musicVolume = .5f;
    [SerializeField] protected Enemy[] musicEngageEnemies;
    [SerializeField] protected bool stopRoomMusicAfterEnemyDeath = false;
    [SerializeField] protected float dungeonMusicRestartDelay = 1f;
    [SerializeField] protected AudioClip enemyDefeatSound;
    [SerializeField] protected bool playEnemyDefeatSound = false;
    protected bool musicEnemiesKilled = false;
    protected bool musicPlaying = false;
    [Header("Character Respawn")]
    [SerializeField] GameObject respawnPosition;
    public Vector3 RespawnPosition{
        protected set => respawnPosition.transform.position = value;
        get => respawnPosition.transform.position;
    }
    [SerializeField] Room respawnRoom;
    public Room RespawnRoom{
        protected set => respawnRoom = value;
        get => respawnRoom;
    }
    [SerializeField] int respawnFloorLevel = 1;
    public int RespawnFloorLevel{
        protected set => respawnFloorLevel = value;
        get => respawnFloorLevel;
    }
    [SerializeField] CharacterDirection respawnPlayerDirection = CharacterDirection.Down;
    public CharacterDirection RespawnPlayerDirection{
        protected set => respawnPlayerDirection = value;
        get => respawnPlayerDirection;
    }
    [Header("Room Entrance Settings")]
    [SerializeField] protected Enemy[] enemiesToSetDirection;
    [SerializeField] protected CharacterDirection[] enemyDirections;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        Initialize();
    }

    void Initialize(){
        Name = gameObject.name.Split('_')[1];
        RoomTransitionTime = GameMaster.Instance.dungeon.roomTransitionTime;

        lightningTime = Random.Range(lightningTimeMin, lightningTimeMax);
        lightningDT = 1.5f;

        lightsToFlashAlphaDict = new Dictionary<Light2D, float>();
        
        foreach(Light2D light in lightsToFlash){
            lightsToFlashAlphaDict.Add(light, light.color.a);
        }

        randomLightningFlashingTime = lightningFlashingTime / Mathf.Round(Random.Range(3, 4));
        objsToDespawn = new List<GameObject>();

        InitializeRoomResetOnDeath();
    }

    protected virtual void InitializeRoomResetOnDeath(){
        if(roomResetSharesUnlockables){
            roomResetObjects = unlockableObjects;
            roomResetEnemies = unlockingEnemies;
            roomResetPuzzleObjects = unlockingPuzzleObjects;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CheckUnlockingEnemies();
        CheckUnlockingPuzzleObjects();
        UpdateScreenShake();
        UpdateRoomMusic();
    }

    protected virtual void FixedUpdate()
    {
        UpdateLightning();
        UpdateUnlockDebug();
    }

    void UpdateUnlockDebug(){
        if(objectsUnlocked  || !unlockDebug){
            return;
        }

        UnlockObjects();
    }

    void UpdateLightning(){
        if(lightsToFlash.Length == 0 || !active){
            return;
        }

        if(lightningFlashing){
            lightningFlashingDT += Time.deltaTime;
            randomLightningFlashingDT += Time.deltaTime;

            if(lightningFlashingDT >= lightningFlashingTime){
                lightningFlashing = false;
                lightningFlashingDT = 0f;

                foreach(Light2D light in lightsToFlash){
                    light.color.a = lightsToFlashAlphaDict[light];
                }
                if(lightningBrightensRoom){
                    FunkyCode.Lighting2D.DarknessColor = useGlobalDarkness ? GameMaster.Instance.dungeon.darknessColor : darknessColor;
                }
                return;
            }

            // thunderSoundDT += Time.deltaTime;
            // if(thunderSoundDT >= thunderSound.length){
            //     thunderSoundDT = 0f;
            //     GameMaster.Instance.audioSource.PlayOneShot(thunderSound);
            // }


            if(randomLightningFlashingDT > randomLightningFlashingTime){
                SetLightningLights();
                if(lightningBrightensRoom){
                    Lighting2D.DarknessColor = useGlobalDarkness ? GameMaster.Instance.dungeon.darknessColor : darknessColor;
                }

                DOVirtual.DelayedCall(.05f, ()=>{
                    if(lightningBrightensRoom){
                        Color brighterColor = useGlobalDarkness ? GameMaster.Instance.dungeon.darknessColor : darknessColor;
                        brighterColor.a = lightningBrighteningAlpha / 255f;
                        Lighting2D.DarknessColor = brighterColor;
                    }
                    SetLightningLights(false);
                });

                randomLightningFlashingDT = 0f;
            }
            return;
        }

        lightningDT += Time.deltaTime;

        if(lightningDT >= lightningTime){
            lightningDT = 0f;
            lightningTime = Random.Range(lightningTimeMin, lightningTimeMax);
            lightningFlashing = true;
            randomLightningFlashingTime = lightningFlashingTime / Mathf.Round(Random.Range(3, 4));
            randomLightningFlashingDT = 0f;
            thunderSoundDT = 0f;
            if(playThunderSound){
                roomAudioSource.PlayOneShot(thunderSound, GameMaster.Instance.MasterVolume);
            }

            foreach(Light2D light in lightsToFlash){
                light.color.a = 1f;
            }

            
        }
    }

    void UpdateScreenShake(){
        if(!roomShakes || !active){
            return;
        }

        shakeDT += Time.deltaTime;
        if(shakeDT >= timeBetweenShakes){
            // Debug.Log($"Shaking camera: {GameMaster.Instance.dungeon.ActiveCamera.name}");
            GameMaster.Instance.dungeon.ActiveCamera.DOShakePosition(shakeDuration, .25f);
            shakeDT = 0f;
        }
    }

    void UpdateRoomMusic(){
        if(musicPlaying && stopRoomMusicAfterEnemyDeath){
            for(int i = 0 ; i < musicEngageEnemies.Length; i++){
                Enemy enemy = musicEngageEnemies[i];
                if(enemy.Alive){
                    break;
                }
                if(i == musicEngageEnemies.Length - 1){
                    GameMaster.Instance.dungeon.PlayDungeonMusic(dungeonMusicRestartDelay, GameMaster.Instance.MasterVolume);
                    musicPlaying = false;
                    musicEnemiesKilled = true;
                    playRoomMusicOnEntrance = false;
                    pauseDungeonMusicOnEntrance = false;
                    if(enemyDefeatSound != null && playEnemyDefeatSound){
                        GameMaster.Instance.audioSource.PlayOneShot(enemyDefeatSound, GameMaster.Instance.MasterVolume);
                    }
                }
            }
        }
        if(!musicPlaying && playRoomMusicOnEnemyEngage && !musicEnemiesKilled){
            foreach(Enemy enemy in musicEngageEnemies){
                if(enemy.Engaged){
                    musicPlaying = true;
                    GameMaster.Instance.dungeon.PlayRoomMusic(roomMusic, musicVolume * GameMaster.Instance.MasterVolume);
                    break;
                }
            }
        }
    }

    void SetLightningLights(bool originalValue = true, float customValue = 1f){
        foreach(Light2D light in lightsToFlash){
            if(originalValue){
                light.color.a = lightsToFlashAlphaDict[light];
            } else {
                light.color.a = customValue;
            }
        }
    }

    void CheckUnlockingEnemies(){
        if(objectsUnlocked || (unlockableObjects == null && !lockDoorsOnFirstEntry) || unlockingEnemies.Length == 0){
            return;
        }

        for(int i = 0; i < unlockingEnemies.Length; i++){
            if(!unlockingEnemies[i].GetComponent<IDefeatable>().Defeated){
                return;
            }
        }

        UnlockObjects();
    }

    void CheckUnlockingPuzzleObjects(){
        if(objectsUnlocked || (unlockableObjects == null && !lockDoorsOnFirstEntry) || unlockingPuzzleObjects.Length == 0){
            return;
        }

        foreach(GameObject gObj in unlockingPuzzleObjects){
            switch(gObj.tag){
                case "Lightable":
                    if(!gObj.GetComponent<LightableObject>().On){
                        return;
                    }
                break;
            }
        }
        
        UnlockObjects();
    }

    protected void UnlockObjects(){
        if(lockDoorsOnFirstEntry){
            foreach(Door door in doors){
                door.UnlockDoor();
            }

            GameMaster.Instance.audioSource.PlayOneShot(unlockSound, GameMaster.Instance.MasterVolume);
        }
        
        if(unlockableObjects != null){
            foreach(GameObject unlockableObjects in unlockableObjects){
                unlockableObjects.GetComponent<IUnlocklableObject>().Unlock();
            }
        }

        foreach(GameObject gObj in unlockObjectsToActivate){
            switch(gObj.tag){
                case "Lightable":
                    gObj.GetComponent<LightableObject>().SetOn(true, false);
                break;
            }
        }

        objectsUnlocked = true;
        if(roomResetSharesUnlockables){
            roomResetObjectiveComplete = true;
        }
    }

    public void ActivateRoom(bool initialRoomSet = false, float lockDoorsDelay = 0f){
        GameMaster.Instance.dungeon.SetRoom(this, initialRoomSet, cameraOffsetX, cameraOffsetY);
        gameObject.SetActive(true);
        active = true;
        ActivateChildrenEnemies(enemies);

        DOVirtual.DelayedCall(lockDoorsDelay, LockDoors);
        SetDarknessColor(initialRoomSet);

        float adjacentRoomDelay = initialRoomSet ? .5f : 0f;
        DOVirtual.DelayedCall(adjacentRoomDelay, ActivateAdjacentRooms);

        if(useCameraRoomOffset){
            // GameMaster.Instance.dungeon.Camera
        }

        if(playRainSound){
            roomAudioSource.DOFade(GameMaster.Instance.MasterVolume, 1f);
            roomAudioSource.Play();
            // roomAudioSource.loop = true;
        }

        if(playRoomMusicOnEntrance){
            GameMaster.Instance.dungeon.PlayRoomMusic(roomMusic, musicVolume * GameMaster.Instance.MasterVolume);
            musicPlaying = true;
        } else if(pauseDungeonMusicOnEntrance){
            GameMaster.Instance.dungeon.PauseDungeonMusic();
        } else{
            GameMaster.Instance.dungeon.PlayDungeonMusic(0f, GameMaster.Instance.MasterVolume);
        }

        if(firstRoomActivation){
            firstRoomActivation = false;
            timeLastRespawned = Time.time;
        } else {
            RespawnItems();
        }
        // if(timeLastVisited - Time.time >= respawnItemsDuration){
        //     Debug.Log("30 seconds have passed since last respawned items");
        // }

        
    }

    void ActivateAdjacentRooms(){
        foreach(Transform roomTransform in allRooms.transform){
            Room currRoom = roomTransform.gameObject.GetComponent<Room>();

            if(adjacentRooms.Contains(currRoom)){
                roomTransform.gameObject.SetActive(true);
            } else if(currRoom != this){
                roomTransform.gameObject.SetActive(false);
            }
        }
    }

    void RespawnItems(){
        // Debug.Log($"Time since last respawn: {Time.time - timeLastRespawned}");
        if(Time.time - timeLastRespawned < respawnItemsDuration){
            return;
        }

        // timeLastRespawned = Time.time;
        foreach(GameObject respawnableObj in respawnableItems){
            respawnableObj.GetComponent<I_Respawnable>().Respawn();
        }
        canResetLastTimeRespawned = true;
    }

    public virtual void DeathRoomReset(){
        if(active){
            DeactivateRoom();
        }
        if(!roomCompleteResetOnDeath || roomResetObjectiveComplete){
            return;
        }

        if(lockDoorsOnFirstEntry){
            firstEntry = true;

            foreach(Door door in doors){
                door.UnlockDoor(false);
            }
        }

        foreach(GameObject respawnableItem in roomResetRespawnableItems){
            I_Respawnable respawnable = respawnableItem.GetComponent<I_Respawnable>();

            if(respawnable != null){
                respawnable.Respawn();
            }
        }

        foreach(GameObject respawnableEnemy in roomResetEnemies){
            Enemy enemy = respawnableEnemy.GetComponent<Enemy>();
            if(enemy != null){
                enemy.Revive();
                continue;
            }

        ClusterEnemy clusterEnemy = respawnableEnemy.GetComponent<ClusterEnemy>();
            if(clusterEnemy != null){
                clusterEnemy.Revive();
            }
        }

        foreach(GameObject respawnableObj in roomResetObjects){
            I_Respawnable respawnable = respawnableObj.GetComponent<I_Respawnable>();

            if(respawnable != null){
                respawnable.Respawn();
            }
        }
    }

    public void AddObjToDesapwn(GameObject obj){
        objsToDespawn.Add(obj);
    }

    void DespawnObjs(){
        if(objsToDespawn == null){
            return;
        }
        for(int i = objsToDespawn.Count - 1; i >= 0; i--){
            GameObject obj = objsToDespawn[i];
            objsToDespawn.Remove(obj);
            Destroy(obj);
        }

        // foreach(GameObject obj in objsToDespawn){
        //     Destroy(obj);
        // } 
    }

    void LockDoors(){
        if(lockDoorsOnFirstEntry && firstEntry){
            foreach(Door door in doors){
                door.LockDoor(true);
            }

            firstEntry = false;
        }
    }

    public virtual void DeactivateRoom(bool initialRoomSet = false){
        active = false;
        DOVirtual.DelayedCall(RoomTransitionTime * .5f, ()=>DeactivateChildrenEnemies(enemies, initialRoomSet));
        // DeactivateChildrenEnemies(enemies, initialRoomSet);
        roomAudioSource.DOFade(0f, 1f);

        if(canResetLastTimeRespawned){
            timeLastRespawned = Time.time;
            canResetLastTimeRespawned = false;
        }

        musicPlaying = false;
        DespawnObjs();
    }

    public void DeactivateAdjacentRooms(){
        foreach(Room adjacentRoom in adjacentRooms){
            adjacentRoom.gameObject.SetActive(false);
        }
    }

    public void DeactivateChildrenEnemies(GameObject parentObject, bool initialRoomSet = false){
        foreach(Transform childTransform in parentObject.transform){
            if(childTransform.gameObject.tag != "Enemy"){
                if(childTransform.gameObject.tag == "ClusterEnemy"){
                    childTransform.gameObject.GetComponent<ClusterEnemy>().ResetOnRoomLeave();
                    continue;
                }
                DeactivateChildrenEnemies(childTransform.gameObject, initialRoomSet);
            } else if(childTransform.gameObject.tag == "Enemy"){
                if(initialRoomSet){
                    childTransform.gameObject.SetActive(false);
                    continue;
                }
                childTransform.gameObject.GetComponent<Enemy>().ResetOnRoomLeave();
            }
        }
    }

    public void ActivateChildrenEnemies(GameObject parentObject){
        foreach(Transform childTransform in parentObject.transform){
            if(childTransform.childCount > 0 && childTransform.gameObject.tag != "Enemy"){
                if(childTransform.gameObject.tag == "ClusterEnemy"){
                    childTransform.gameObject.GetComponent<ClusterEnemy>().ActivateOnRoomEnter();
                    continue;
                }
                ActivateChildrenEnemies(childTransform.gameObject);
            } else if(childTransform.gameObject.tag == "Enemy"){
                childTransform.gameObject.SetActive(true);
            }
        }

        for(int i = 0; i < enemiesToSetDirection.Length; i++){
            if(!enemiesToSetDirection[i].Alive){
                continue;
            }
            enemiesToSetDirection[i].SetCharacterDirection(enemyDirections[i]);
        }

        foreach(Transform childTransform in parentObject.transform){
            if(childTransform.childCount > 0 && childTransform.gameObject.tag != "Enemy"){
            } else if(childTransform.gameObject.tag == "Enemy" ){
                childTransform.GetComponent<Enemy>().suspendAnimationUpdate = false;
            }
        }
    }

    public void ModifyDarknessColor(float amount){
        if(!lightsAffectDarknessColor){
            return;
        }

        darknessColor.a += amount;
        SetDarknessColor(true);
    }

    void SetDarknessColor(bool immediateChange = false){
        // float adjustDarknessAmount = darknessColor.a - GameMaster.Instance.DarknessSliderRange / 2f + GameMaster.Instance.DarknessSliderRange * ;

        Color targetColor = useGlobalDarkness ? GameMaster.Instance.dungeon.darknessColor : darknessColor;
        Color targetColor_Pass2 = useGlobalDarkness ? GameMaster.Instance.dungeon.darknessColor_Pass2 : darknessColor_Pass2;

        if(!GameMaster.Instance.dungeon.UseCustomDarknessColor){
            targetColor = Lighting2D.DarknessColor;
            targetColor_Pass2 = Lighting2D.LightmapPresets[1].darknessColor;
        }
        
        if(immediateChange){
            Lighting2D.DarknessColor = targetColor;
            Lighting2D.LightmapPresets[1].darknessColor = targetColor_Pass2;
            return;
        }

        DOTween.To(()=> Lighting2D.DarknessColor, x=> Lighting2D.DarknessColor = x, targetColor, RoomTransitionTime);
        DOTween.To(()=> Lighting2D.LightmapPresets[1].darknessColor, x=> Lighting2D.LightmapPresets[1].darknessColor = x, targetColor_Pass2, RoomTransitionTime);
    }

    public void SetAudioVolume(float volume){
        roomAudioSource.volume = musicVolume * volume;
    }
}
