using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EventTrigger : MonoBehaviour, I_Triggerable
{
    [SerializeField] TriggerType triggerType = TriggerType.Move;
    public TriggerType TriggerType{
        get => triggerType;
        protected set => triggerType = value;
    }
    [SerializeField] GameObject[] movePositions;
    public GameObject[] MovePositions{
        get => movePositions;
        protected set => movePositions = value;
    }
    [SerializeField] protected float[] moveSpeeds;
    public List<(GameObject movePosition, float moveSpeed)> MoveActions{
        get;
        protected set;
    }
    public List<(GameObject character, string text)> DialogueTupleList{
        get;
        protected set;
    }
    [SerializeField] CharacterDirection playerDirectionWhenFinished = CharacterDirection.Down;
    public CharacterDirection CharacterDirectionWhenFinished{
        protected set => playerDirectionWhenFinished = value;
        get => playerDirectionWhenFinished;
    }
    [SerializeField] protected List<GameObject> dialogue_characters;
    [SerializeField] protected List<string> dialogue_text;
    [SerializeField] protected float moveSpeed = 6f;
    
    [SerializeField] protected GameObject[] triggerableObjs;
    public bool Triggered{
        get;
        protected set;
    } = false;
    [SerializeField] protected bool useCollider = true;
    [SerializeField] protected Collider2D collider;
    [SerializeField] EventTrigger nextEventTrigger;
    public EventTrigger NextEventTrigger{
        get => nextEventTrigger;
        protected set => nextEventTrigger = value;
    }
    protected bool finished = false;
    [SerializeField] float pauseBeforeNextAction = 0f;
    public float PauseBeforeNextAction{
        protected set => pauseBeforeNextAction = value;
        get => pauseBeforeNextAction;
    }
    [SerializeField] protected bool giveControlBackToPlayer = true;
    public bool GiveControlBackToPlayer{
        protected set => giveControlBackToPlayer = value;
        get => giveControlBackToPlayer;
    }
    [SerializeField] protected bool fadeOutUiStart = true;
    [SerializeField] protected float uiFadeOutDuration = 1f;
    [SerializeField] protected bool fadeInUiEnd = true;
    [Header("Camera Controls")]
    [SerializeField] protected CameraBehavior camera;
    [SerializeField] protected bool initialZoomTweenTriggers = true;
    [SerializeField] protected bool overrideCurrentCameraZoom = false;
    [SerializeField] protected float initialCameraZoom = 6.5f;
    [SerializeField] protected float initialCameraZoomAnimDur = 1f;
    [SerializeField] protected float[] cameraZoomLevels;
    [SerializeField] protected float[] cameraZoomAnimDurs;
    [SerializeField] protected float[] cameraZoomPauseDurs;
    protected int cameraZoomIndex = 0;
    protected bool cameraZoomsFinished = false;
    [SerializeField] protected bool overrideCurrentCameraPosition = false;
    [SerializeField] protected Vector2 initialCameraPosition;
    [SerializeField] protected float initialCameraMoveAnimDur = 1f;
    [SerializeField] protected Vector2[] cameraPositions;
    [SerializeField] protected float[] cameraPositionAnimDurs;
    [SerializeField] protected float[] cameraPositionPauseDurs;
    protected int cameraPositionIndex = 0;
    protected bool cameraMovesFinished = false;
    
 
    // Start is called before the first frame update
    protected virtual void Start()
    {
        switch(TriggerType){
            case TriggerType.Move:
                MoveActions = new List<(GameObject movePosition, float moveSpeed)>();
                for(int i = 0; i < movePositions.Length; i++){
                    MoveActions.Add((movePositions[i], moveSpeeds[i]));
                }
                break;
            case TriggerType.Dialogue:
                DialogueTupleList = new List<(GameObject character, string text)>();
                for(int i = 0; i < dialogue_characters.Count; i++){
                    DialogueTupleList.Add((dialogue_characters[i], dialogue_text[i]));
                }
                break;
            case TriggerType.CameraAnimation:
                if(camera == null){
                    camera = Camera.main.GetComponent<CameraBehavior>();
                }

                break;
        }

        if(collider != null && !useCollider){
            collider.enabled = false;
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateCameraAnimation();
    }

    protected virtual void UpdateCameraAnimation(){
        if(!finished && cameraMovesFinished && cameraZoomsFinished){
            Finish();
        }
    }

    public virtual void Trigger(){
        Triggered = true;

        foreach(GameObject obj in triggerableObjs){
            I_Triggerable triggerable = obj.GetComponent<I_Triggerable>();
            triggerable.Trigger();
        }

        switch(triggerType){
            case TriggerType.Dialogue:
                if(!useCollider){
                    GameMaster.Instance.Player.GetComponent<CharacterControls>().PauseForInteractionEvent(this);
                }
                break;
            case TriggerType.Move:
                if(!useCollider){
                    GameMaster.Instance.Player.GetComponent<CharacterControls>().MovementEventTriggered(this, true);
                }
                break;
            case TriggerType.CameraAnimation:
                if(!useCollider){
                    GameMaster.Instance.Player.GetComponent<CharacterControls>().EventTriggered(this, true);
                }

                if(camera == null) {
                    camera = Camera.main.GetComponent<CameraBehavior>();
                }
                BeginCameraAnimation();
                break;
            default:
                break;
        }

        if(fadeOutUiStart){
            GameMaster.Instance.FadeOutUI(uiFadeOutDuration);
        }
    }

    protected virtual void BeginCameraAnimation(){
        float moveDur = overrideCurrentCameraPosition ? 0 : initialCameraMoveAnimDur;
        float zoomDur = overrideCurrentCameraZoom ? 0 : initialCameraZoomAnimDur;

        Tween positionTween = camera.TransitionToNewPosition(initialCameraPosition, moveDur);
        Tween zoomTween = camera.ZoomInFocusObject(initialCameraZoom, zoomDur);

        positionTween.OnComplete(NextCameraMoveAnim);
        zoomTween.OnComplete(NextCameraZoomAnim);
    }

    protected virtual void NextCameraMoveAnim(){
        if(cameraPositionIndex >= cameraPositions.Length){
            cameraMovesFinished = true;
            return;
        }

        Tween positionTween;
        DOVirtual.DelayedCall(cameraPositionPauseDurs[cameraPositionIndex], ()=>{
            positionTween = camera.TransitionToNewPosition(cameraPositions[cameraPositionIndex], cameraPositionAnimDurs[cameraPositionIndex]);
            positionTween.OnComplete(NextCameraMoveAnim);
            cameraPositionIndex++;
        });
    }

    protected virtual void NextCameraZoomAnim(){
        if(cameraZoomIndex >= cameraZoomLevels.Length){
            cameraZoomsFinished = true;
            return;
        }

        Tween zoomTween;
        DOVirtual.DelayedCall(cameraZoomPauseDurs[cameraZoomIndex], ()=>{
            zoomTween = camera.ZoomInFocusObject(cameraZoomLevels[cameraZoomIndex], cameraZoomAnimDurs[cameraZoomIndex]);
            zoomTween.OnComplete(NextCameraZoomAnim);
            cameraZoomIndex++;
        });
    }

    public virtual void Finish(){
        finished = true;
        if(nextEventTrigger != null){
            DOVirtual.DelayedCall(pauseBeforeNextAction, ()=>{nextEventTrigger.Trigger();});
        }

        if(fadeInUiEnd){
            GameMaster.Instance.FadeInUI((pauseBeforeNextAction == 0 ? 1f : pauseBeforeNextAction));
        }
    }
}

public enum TriggerType{
    Move,
    Dialogue,
    CameraAnimation,
    Animation
}
