using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovementTile : MonoBehaviour, ISwitchable
{
    [SerializeField] TileDirection tileDirection = TileDirection.Down;
    public TileDirection TileDirection{
        get => tileDirection;
    }
    protected Dictionary<TileDirection, Vector2> directionVectorsDictionary;
    [SerializeField] float movementForceMagnitude = 2f;
    [SerializeField] bool activated = true;
    public bool Activated{
        protected set;
        get;
    } = true;
    [SerializeField] Animator animator;
    [SerializeField] float turnOnOffTime = 1f;
    Tween speedTween;
    [SerializeField] bool cyclesDirection = false;
    [SerializeField] bool cyclesWhileInactive = true;
    [SerializeField] TileDirection[] directionsToCycle;
    [SerializeField] float cycleDuration = 2f;
    float cycleDT = 0f;
    int cycleDirectionIndex = 0;
    [SerializeField] Collider2D collider;
    public Collider2D Collider{
        get => collider;
        protected set => collider = value;
    }

    void Awake()
    {
        directionVectorsDictionary = new Dictionary<TileDirection, Vector2>();
        directionVectorsDictionary.Add(TileDirection.Up, Vector2.up);
        directionVectorsDictionary.Add(TileDirection.Down, Vector2.down);
        directionVectorsDictionary.Add(TileDirection.Left, Vector2.left);
        directionVectorsDictionary.Add(TileDirection.Right, Vector2.right);

        SetDirection(tileDirection);

        if(!activated){
            animator.speed = 0f;
            Activated = false;
        }
    }

    void SetDirection(TileDirection tileDirection){
        this.tileDirection = tileDirection;
        float angle = 0f;

        switch(tileDirection){
            case TileDirection.Down:
                angle = 0f;
                break;
            case TileDirection.Up:
                angle = 180f;
                break;
            case TileDirection.Left:
                angle = -90f;
                break;
            case TileDirection.Right:
                angle = 90f;
                break;
        }

        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateDirectionCycle();
    }

    public void UpdateDirectionCycle(){
        if(!cyclesDirection){
            return;
        }

        cycleDT += Time.deltaTime;

        if(cycleDT >= cycleDuration){
            if(++cycleDirectionIndex >= directionsToCycle.Length){
                cycleDirectionIndex = 0;
            }

            SetDirection(directionsToCycle[cycleDirectionIndex]);
            cycleDT = 0f;
        }
    }

    public void Activate(){
        if(Activated){
            return;
        }
        Activated = true;

        if(speedTween != null){
            speedTween.Kill();
        }
        speedTween = DOTween.To(() => animator.speed, x => animator.speed = x, 1f, turnOnOffTime * (1f - animator.speed));
    }

    public void Deactivate(){
        if(!Activated){
            return;
        }
        Activated = false;

        if(speedTween != null){
            speedTween.Kill();
        }
        speedTween = DOTween.To(() => animator.speed, x => animator.speed = x, 0f, turnOnOffTime * animator.speed);
    }
    
    public bool ActivatePermanently{
        get; set;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        switch(other.tag){
            case "PlayerBoxCollider":
                CharacterControls characterControls = other.GetComponent<BoxCollider_Trigger>().CharacterControls;
                characterControls.ApplyForceFromMovementTile(directionVectorsDictionary[tileDirection] * movementForceMagnitude * animator.speed, this);
                break;

            case "EnemyGeometryCollider":
                Enemy enemy = other.transform.parent.GetComponent<Enemy>();
                enemy.ApplyForceFromMovementTile(directionVectorsDictionary[tileDirection] * movementForceMagnitude * animator.speed, this);
                break;
            case "Interactable":
                Key key = other.GetComponent<Key>();

                if(key == null){
                    break;
                }

                key.ApplyForceFromMovementTile(directionVectorsDictionary[tileDirection] * movementForceMagnitude * animator.speed, this);
                break;
            case "Breakable":
                Skull skull = other.GetComponent<Skull>();

                if(skull == null){
                    break;
                }

                skull.ApplyForceFromMovementTile(directionVectorsDictionary[tileDirection] * movementForceMagnitude * animator.speed, this);
                break;
        }
    }
}

public enum TileDirection{
    Up,
    Down,
    Left,
    Right
}
