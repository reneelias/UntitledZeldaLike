using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FunkyCode;
using DG.Tweening;

public class StaffBeamMain : MonoBehaviour, IParticle
{
    public bool Active{
        get;
        private set;
    }
    private Color color;
    private Light2D lightComp;
    private float lifeDuration;
    private float lifeDT;
    private GameObject parentObject;
    private float xIterator;
    private float yIterator;
    [SerializeField] private GameObject lrObj0, lrObj1;
    private LineRenderer lineRenderer0, lineRenderer1;
    private LineRenderer[] lineRenderers;
    float lineUpdateDT;
    float lineUpdateTime;
    float currLineStepX;
    float lineStepIncrement;
    bool updatingLines;
    public int damagePts;
    float distBetweenOrbitingParticles;
    public float DistBetweenOrbitingParticles{
        get{
            return distBetweenOrbitingParticles;
        }
        set{
            distBetweenOrbitingParticles = value;
            circleCollider.radius = distBetweenOrbitingParticles / 2f;
        }
    }

    public float collisionParticleDisplaceMax_X = -.75f;
    public float collisionParticleDisplaceMax_Y = -.75f;
    public float collisionParticleAngleRange = 30f;

    const float BASE_SPEED = 14f;

    float lineOverallTimerDT;
    float lineOverallTimer;
    private bool hitTarget;

    [SerializeField] ParticleEmitter particleEmitter;

    [SerializeField] CircleCollider2D circleCollider;
    

    // private Particle[] lineParticles;

    // private float speed;
    public float Speed{
        get;
        set;
    }

    float scale;
    float lightBaseSize;
    Vector2 velocity;
    [SerializeField] Rigidbody2D rigidbody2D;
    float collisionDT = 0f;
    float collisionTime = .05f;
    [SerializeField] int floorLevel = 1;
    public int FloorLevel{
        get => floorLevel;
        protected set => floorLevel = value;
    }


    public void initializeValues(Color color, GameObject parentObject){
        this.color = color;
        GetComponent<SpriteRenderer>().color = color;
        lightComp = GetComponent<Light2D>();
        lightComp.color = color;
        lightComp.color.a = 0;
        EnableRenderer(false);
        this.parentObject = parentObject;
        Active = false;
        Speed = 1f;

        lineRenderer0 = lrObj0.GetComponent<LineRenderer>();
        lineRenderer1 = lrObj1.GetComponent<LineRenderer>();

        lineRenderers = new LineRenderer[]{lineRenderer0, lineRenderer1};

        lineUpdateDT = 0f;
        lineUpdateTime = .006f;
        updatingLines = false;
        currLineStepX = 0f;
        lineStepIncrement = 1.5f;
        lineOverallTimerDT = 0f;
        lineOverallTimer = 6f;

        damagePts = 5;
        hitTarget = false;

        GetComponent<Rigidbody2D>().simulated = false;
        particleEmitter.color = color;
    }

    // Start is called before the first frame update
    void Start()
    {
        lightBaseSize = lightComp.size;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Move();      
        UpdateActive();
        UpdateLines();  
    }

    void UpdateActive(){
        if(!Active){
            return;
        }

        collisionDT += Time.deltaTime;
        lifeDT += Time.deltaTime;

        if(lifeDT >= lifeDuration){
            Deactivate();
        }
    }

    public void Activate(float spawnX, float spawnY, float lifeDuration, Vector2 velocity, Color color, float scale, int floorLevel){
        transform.position = new Vector3(spawnX, spawnY, 0);
        transform.localScale = new Vector3(scale, scale, 1f);
        this.scale = scale;
        this.lifeDuration = lifeDuration;
        lifeDT = 0;
        this.velocity = velocity;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
        // Debug.Log($"xIterator: {xIterator}, yIterator: {yIterator}");
        Active = true;
        EnableRenderer(true);
        lightComp.color.a = 1;
        lightComp.size = lightBaseSize * scale;
        
        currLineStepX = 0f;
        lineUpdateDT = 0f;
        updatingLines = true;

        // Debug.Log($"Speed: {Speed}");
        this.color = color;
        hitTarget = false;
        
        rigidbody2D.simulated = true;
        rigidbody2D.WakeUp();
        rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        collisionDT = 0f;
        FloorLevel = floorLevel;
        
        // Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameObject.Find("WizardBoxCollider").GetComponent<Collider2D>());
        // Debug.Log($"collider radius: {GetComponent<CircleCollider2D>().radius}");
    }

    private void Move(){
        if(!Active){
            return;
        }

        rigidbody2D.velocity = velocity;
        // transform.position += (new Vector3(xIterator, yIterator, 0) * Speed);
    }

    public void Deactivate(){
        EnableRenderer(false);
        lightComp.color.a = 0;
        Active = false;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.Sleep();
        updatingLines = false;

        for(int i = 0; i < lineRenderers.Length; i++){
            lineRenderers[i].positionCount = 0;
            // lineRenderers[i].SetPositions(null);
        }
    }

    void EmitCollisionParticles(Collision2D collision){
        Vector2 particleTrajectoryLimit_00 = new Vector2(-collisionParticleDisplaceMax_X, collisionParticleDisplaceMax_Y);
        Vector2 particleTrajectoryLimit_01 = new Vector2(-collisionParticleDisplaceMax_X, -collisionParticleDisplaceMax_Y);
        
        float ogAngle_00 = Mathf.Atan2(particleTrajectoryLimit_00.y, particleTrajectoryLimit_00.x);
        float ogAngle_01 = Mathf.Atan2(particleTrajectoryLimit_01.y, particleTrajectoryLimit_01.x);

        // float currentAngle = transform.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 collisionNormal = collision.contacts[0].normal;
        float currentAngle = Mathf.Atan2(collisionNormal.y, collisionNormal.x) + Mathf.PI;

        float newAngle_00 = ogAngle_00 + currentAngle;
        float newAngle_01 = ogAngle_01 + currentAngle;

        // Vector2 newTrajectoryLimit_00 = new Vector2(Mathf.Cos(newAngle_00) * collisionParticleDisplaceMax_X, Mathf.Sin(newAngle_00) * collisionParticleDisplaceMax_Y);
        // Vector2 newTrajectoryLimit_01 = new Vector2(Mathf.Cos(newAngle_01) * collisionParticleDisplaceMax_X, Mathf.Sin(newAngle_01) * collisionParticleDisplaceMax_Y);

        // particleEmitter.xDisplacementRangeMin = Mathf.Min(newTrajectoryLimit_00.x, newTrajectoryLimit_01.x);
        // particleEmitter.xDisplacementRangeMax = Mathf.Max(newTrajectoryLimit_00.x, newTrajectoryLimit_01.x);
        // particleEmitter.yDisplacementRangeMin = Mathf.Min(newTrajectoryLimit_00.y, newTrajectoryLimit_01.y);
        // particleEmitter.yDisplacementRangeMax = Mathf.Max(newTrajectoryLimit_00.y, newTrajectoryLimit_01.y);

        particleEmitter.angleOrigin = currentAngle * Mathf.Rad2Deg - 180;
        particleEmitter.angleRange = collisionParticleAngleRange;

        particleEmitter.spawnOffsetX = Mathf.Cos(currentAngle) * circleCollider.radius;
        particleEmitter.spawnOffsetY = Mathf.Sin(currentAngle) * circleCollider.radius;

        float spawnRangeModifier = 0;
        particleEmitter.SetSpawnRange(circleCollider.radius * spawnRangeModifier, circleCollider.radius * spawnRangeModifier);

        particleEmitter.SpawnParticles();
    }

    public void FadeOut(){
        GetComponent<SpriteRenderer>().DOFade(0, .2f).OnComplete(Deactivate);
        DOTween.To(()=> lightComp.color.a, x=> lightComp.color.a = x, 0, .2f);
    }

    public void EnableRenderer(bool enable){
        GetComponent<Renderer>().enabled = enable;
    }

    public void UpdateLines(){
        if(!updatingLines){
            return;
        }

        // lineUpdateDT += Time.deltaTime;
        // if(lineUpdateDT < lineUpdateTime){
        //     return;
        // }

        // float lineLength = 1f;
        float zPosition = 1f;
        // float angleModifier = angleSeperation / 2 - index * angleSeperation;
        // angleModifier *= Mathf.Deg2Rad;
        
        float stepDenominator = 20f;
        float sinAmplitude = DistBetweenOrbitingParticles;
        int pointsLength = 10;
        Vector3[] linePoints;

        for(int j = 0; j < lineRenderers.Length; j++){
            linePoints = new Vector3[pointsLength];
            for(int i = 0; i < linePoints.Length; i++){
                float currX = (i + currLineStepX) / stepDenominator;
                float currY = sinAmplitude * Mathf.Pow(Mathf.Sin(currX), 2);
                if(j == 0){
                    currY *= -1;
                }
                // float currPointAngle = Mathf.Atan2(currY,currX);
                // currPointAngle += angleModifier;
                float currMagnitude =  Mathf.Sqrt(Mathf.Pow(currX, 2f) + Mathf.Pow(currY, 2f));
                linePoints[i] = new Vector3(currX - currLineStepX / stepDenominator, currY + sinAmplitude / 2f - sinAmplitude * j, zPosition);
            }
            float xShift = (linePoints[linePoints.Length - 1].x - linePoints[0].x) / 2f;

            for(int i = 0; i < linePoints.Length; i++){
                linePoints[i].x -= xShift;
            }

            lineRenderers[j].positionCount = linePoints.Length;
            lineRenderers[j].SetPositions(linePoints);
            lineRenderers[j].startColor = color;
            lineRenderers[j].endColor = color;
            lineRenderers[j].sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
        }
            if(currLineStepX == 0f){
                foreach(LineRenderer lr in lineRenderers){
                    lr.widthMultiplier = 0f;
                    DOTween.To(()=> lr.widthMultiplier, x=> lr.widthMultiplier = x, .05f * scale, 1f * (Speed / BASE_SPEED));
                }
            }
        
        // linePoints[0] = new Vector3(0, 0, zPosition);
        // linePoints[1] = new Vector3(lineLength * Mathf.Cos(angleModifier), lineLength * Mathf.Sin(angleModifier), zPosition);
        // LineRenderer lr = lineRendererObject.GetComponent<LineRenderer>();
        /*LineRenderer does not automatically update positionCount! you must manually set the number!*/
        

        lineUpdateDT = 0f;
        currLineStepX += (lineStepIncrement * (Speed / BASE_SPEED));
    }

    // private void OnTriggerEnter2D(Collider2D collider2D){
    //     Debug.Log($"Collision: {collider2D.name}");
    //     Debug.Log($"Particle Angle: {transform.eulerAngles}");
    //     if(collider2D.tag != "Player" && Active){// && collider2D.tag != "CandleLight"){
    //         // EmitCollisionParticles(collider2D);
    //         Deactivate();

    //         if(hitTarget || !Camera.main.GetComponent<CameraBehavior>().IsInCamera(collider2D.bounds)){
    //             return;
    //         }

    //         switch(collider2D.tag){
    //             case "Enemy":
    //                 collider2D.gameObject.GetComponent<Enemy>().ChangeHP(-damagePts);
    //                 break;
    //             case "Lightable":
    //                 collider2D.gameObject.GetComponent<LightableObject>().SetOn(true);
    //                 break;
    //         }

    //         hitTarget = true;
    //     }
    // }

    private void OnCollisionEnter2D(Collision2D collision){
        if(!Active || hitTarget || !Camera.main.GetComponent<CameraBehavior>().IsInCamera(collision.otherCollider.bounds)){
            return;
        }
        bool brokeObject = false;

        switch(collision.gameObject.tag){
                case "Enemy":
                    if(collision.gameObject.GetComponent<Enemy>().FloorLevel == floorLevel){
                        collision.gameObject.GetComponent<Enemy>().ChangeHP(-damagePts);
                    }
                    break;
                case "Lightable":
                    LightableObject lightableObject = collision.gameObject.GetComponent<LightableObject>();

                    if(lightableObject.LightableByBeam){
                        lightableObject.SetOn(true);
                    }
                    break;
                case "Breakable":
                    IBreakable breakableObj = collision.gameObject.GetComponent<IBreakable>();
                    if(breakableObj.BreakableType != Breakable_Type.Mushroom){
                        brokeObject = breakableObj.Break();
                    }
                    break;
            }

        if((collision.gameObject.tag != "Wall" && collision.gameObject.tag != "Door") || collisionDT >= collisionTime){// && collider2D.tag != "CandleLight"){
            if(collision.gameObject.tag == "Breakable" && !brokeObject){
                // Debug.Log($"collision with: {collision.gameObject.tag} and not broken");
                return;
            }
            
            EmitCollisionParticles(collision);
            Deactivate();
            

            hitTarget = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision){
        if(Active && collisionDT >= collisionTime){
            Deactivate();
        }
    }

    private void OnTriggerStay2D(Collider2D collider2D){
        // Debug.Log("beam collision stay");
        // if(collider2D.tag != "Player" && Active){
        //     Deactivate();
        // }
    }
}
