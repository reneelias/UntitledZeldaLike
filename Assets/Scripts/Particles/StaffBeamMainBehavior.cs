using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class StaffBeamMainBehavior : MonoBehaviour, IParticle
{
    public bool Active{
        get;
        private set;
    }
    private Color color;
    private UnityEngine.Rendering.Universal.Light2D lightComp;
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
    public float DistBetweenOrbitingParticles{
        get;
        set;
    }

    const float BASE_SPEED = .25f;

    float lineOverallTimerDT;
    float lineOverallTimer;
    Vector2 velocity;
    [SerializeField] Rigidbody2D rigidbody2D;

    // private Particle[] lineParticles;

    // private float speed;
    public float Speed{
        get;
        set;
    }

    public void initializeValues(Color color, GameObject parentObject){
        this.color = color;
        GetComponent<SpriteRenderer>().color = color;
        lightComp = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        lightComp.color = color;
        lightComp.intensity = 0;
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

        // GetComponent<Rigidbody2D>().Sleep();
        GetComponent<Rigidbody2D>().simulated = false;
        // if(!lineRenderer0 || !lineRenderer1){
        //     Debug.Log("Line Renderers not initialized.");
        // }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActive();
    }

    void FixedUpdate()
    {
        Move();      
        UpdateLines();  
    }

    void UpdateActive(){
        if(!Active){
            return;
        }

        lifeDT += Time.deltaTime;

        if(lifeDT >= lifeDuration){
            Deactivate();
        }
    }

    public void Activate(float spawnX, float spawnY, float lifeDuration, Vector2 velocity, Color color){
        transform.position = new Vector3(spawnX, spawnY, 0);
        this.lifeDuration = lifeDuration;
        lifeDT = 0;
        this.velocity = velocity;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(yIterator, xIterator) * Mathf.Rad2Deg);
        // Debug.Log($"xIterator: {xIterator}, yIterator: {yIterator}");
        Active = true;
        EnableRenderer(true);
        lightComp.intensity = 1;
        
        currLineStepX = 0f;
        lineUpdateDT = 0f;
        updatingLines = true;

        Debug.Log($"Speed: {Speed}");
        this.color = color;
        
        rigidbody2D.simulated = true;
        rigidbody2D.WakeUp();
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
        lightComp.intensity = 0;
        Active = false;
        rigidbody2D.velocity = Vector2.zero;
        rigidbody2D.Sleep();
        updatingLines = false;

        for(int i = 0; i < lineRenderers.Length; i++){
            lineRenderers[i].positionCount = 0;
            // lineRenderers[i].SetPositions(null);
        }
    }

    public void FadeOut(){
        GetComponent<SpriteRenderer>().DOFade(0, .2f).OnComplete(Deactivate);
        DOTween.To(()=> lightComp.intensity, x=> lightComp.intensity = x, 0, .2f);
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

        float lineLength = 1f;
        float zPosition = 1f;
        // float angleModifier = angleSeperation / 2 - index * angleSeperation;
        // angleModifier *= Mathf.Deg2Rad;
        
        float stepDenominator = 20f;
        float sinAmplitude = DistBetweenOrbitingParticles;
        int pointsLength = 15;
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
        }
            if(currLineStepX == 0f){
                foreach(LineRenderer lr in lineRenderers){
                    lr.widthMultiplier = 0f;
                    DOTween.To(()=> lr.widthMultiplier, x=> lr.widthMultiplier = x, .05f, 1f * (Speed / BASE_SPEED));
                }
            }
        
        // linePoints[0] = new Vector3(0, 0, zPosition);
        // linePoints[1] = new Vector3(lineLength * Mathf.Cos(angleModifier), lineLength * Mathf.Sin(angleModifier), zPosition);
        // LineRenderer lr = lineRendererObject.GetComponent<LineRenderer>();
        /*LineRenderer does not automatically update positionCount! you must manually set the number!*/
        

        lineUpdateDT = 0f;
        currLineStepX += (lineStepIncrement * (Speed / BASE_SPEED));
    }

    private void OnTriggerEnter2D(Collider2D collider2D){
        Debug.Log($"Collision: {collider2D.name}");
        if(collider2D.tag != "Player"){// && collider2D.tag != "CandleLight"){
            Deactivate();
        }
    }
}
