using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitter : MonoBehaviour
{
    public int numberOfParticles = 60;
    [SerializeField] Particle particlePrefab;
    [SerializeField] GameObject objectToFollow;
    [SerializeField] GameObject orderParentObject;
    [SerializeField] Sprite[] particleSprites;
    [SerializeField] Material alternateMaterial;
    [SerializeField] public Color color;
    [SerializeField] bool keepParticlesBehindObject = true;
    [SerializeField] bool spawnInFrontOfObject = false;
    Particle[] particles;
    public float particleSpawnTime = .05f;
    private float particleSpawnDT = 0;
    private int particleIndex = 0;
    public int particleSpawnAmount = 3;
    [Tooltip("X offset to the center spawn point.")]
    public float spawnOffsetX = 0f;
    [Tooltip("Y offset to the center spawn point.")]
    public float spawnOffsetY = 0f;
    [Tooltip("Random x range from offset center")]
    public float spawnRangeX = 0f;
    [Tooltip("Random y range from offset center")]
    public float spawnRangeY = 0f;
    float base_spawnOffsetX;
    float base_spawnOffsetY;
    public float timeBase = .5f;
    public float timeRange = .5f;
    public float xDisplacementRangeMin = 0f;
    public float xDisplacementRangeMax = .5f;
    public float yDisplacementRangeMin = 0f;
    public float yDisplacementRangeMax = .5f;
    public float angleOrigin = 90f;
    public float angleRange = 0f;
    public float maxDisplacement = .75f;
    public float minDisplacement = .25f;
    float base_xDisplacementRangeMin;
    float base_xDisplacementRangeMax;
    float base_yDisplacementRangeMin;
    float base_yDisplacementRangeMax;
    [Range(0.01f, 1)] public float particleFadeInTimePercentage = .5f;

    public bool activelySpawning;
    [Range(0.1f, 5f)] public float particleScale = 1f;
    float baseScale;
    [Range(0, 10)] [SerializeField] float particleScaleRange = .5f;
    public bool particleLightsEnabled = true;
    float particleTransformScale;
    [SerializeField] bool considerObjectRotation = true;
    [SerializeField] bool useSortingOrderByY = false;
    // [SerializeField] bool keepInFront


    // Start is called before the first frame update
    void Start()
    {

        particles = new Particle[numberOfParticles];
        for(int i = 0; i < numberOfParticles; i++){
            particles[i] = Instantiate(particlePrefab);
            if(alternateMaterial != null){
                particles[i].GetComponent<SpriteRenderer>().material = alternateMaterial;
                // particles[i].GetComponent<SpriteRenderer>().color = color;
                // particles[i].GetComponent<SpriteRenderer>().sortingOrder = 4;
            }
        }

        baseScale = particles[0].transform.localScale.x * particleScale;
        particleTransformScale = baseScale;
        // Debug.Log($"color: {color}");

        
        base_xDisplacementRangeMin = xDisplacementRangeMin;
        base_xDisplacementRangeMax = xDisplacementRangeMax;
        base_yDisplacementRangeMin = yDisplacementRangeMin;
        base_yDisplacementRangeMax = yDisplacementRangeMax;

        base_spawnOffsetX = spawnOffsetX;
        base_spawnOffsetY = spawnOffsetY;

        if(orderParentObject == null){
            orderParentObject = objectToFollow;
        }

        if(useSortingOrderByY){

        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLayerOrder();
    }

    void FixedUpdate()
    {
        if(activelySpawning){
            SpawnParticles();
        }
    }

    public void SetSpawnRange(float spawnRangeX, float spawnRangeY)
    {
        this.spawnRangeX = spawnRangeX;
        this.spawnRangeY = spawnRangeY;
    }

    public void SpawnParticles(){
        particleSpawnDT += Time.deltaTime;

        if(particleSpawnDT >= particleSpawnTime){
            int startingIndex = particleIndex;
            for(var i = 0; i < particleSpawnAmount; i++){
                while(particles[particleIndex].Active){
                    if(++particleIndex >= numberOfParticles){
                        particleIndex = 0;
                    }

                    if(particleIndex == startingIndex){
                        particleSpawnDT = 0f;
                        if(particles[particleIndex].Active){
                            return;
                        }
                        break;
                    }
                }

                if(considerObjectRotation){
                    CalculateRotationDisplacements();
                }

                float startX = objectToFollow.transform.position.x + spawnOffsetX - spawnRangeX * .5f + spawnRangeX * Random.value;
                float startY = objectToFollow.transform.position.y + spawnOffsetY - spawnRangeY * .5f + spawnRangeY * Random.value;

                float particleLifeDuration = timeBase + Random.value * timeRange;

                particleTransformScale = baseScale + baseScale * particleScaleRange * Random.value * (Random.value < .5f ? 1 : -1);

                float xDisplacement = Random.Range(xDisplacementRangeMin, xDisplacementRangeMax);
                float yDisplacement = Random.Range(yDisplacementRangeMin, yDisplacementRangeMax);

                float randAngle = angleOrigin - angleRange / 2f + Random.Range(0f, angleRange);
                randAngle *= Mathf.Deg2Rad;
                float randMagnitude = Random.Range(minDisplacement, maxDisplacement);
                Vector2 displacementVector = new Vector2(Mathf.Cos(randAngle) * randMagnitude, Mathf.Sin(randAngle) * randMagnitude);
                xDisplacement = displacementVector.x;
                yDisplacement = displacementVector.y;

                Sprite particleSprite = (particleSprites == null || particleSprites.Length == 0) ? null : particleSprites[Random.Range(0, particleSprites.Length - 1)];

                particles[particleIndex].Activate(startX, startY, particleLifeDuration, particleLifeDuration * particleFadeInTimePercentage, particleLifeDuration * (1f - particleFadeInTimePercentage), xDisplacement, yDisplacement, color, particleTransformScale, particleLightsEnabled, particleSprite, useSortingOrderByY);
                if(spawnInFrontOfObject){
                    particles[particleIndex].SetSortingOrder(orderParentObject.GetComponent<SpriteRenderer>().sortingOrder + 1);
                }

                if(++particleIndex >= numberOfParticles){
                    particleIndex = 0;
                }
            }
            

            particleSpawnDT = 0;
        }
    }

    // void CalculateRotationDisplacements(){
    //     Vector2 particleTrajectoryLimit_00 = new Vector2(base_xDisplacementRangeMin, 0f);
    //     Vector2 particleTrajectoryLimit_01 = new Vector2(base_xDisplacementRangeMin, 0f);
    //     Vector2 particleTrajectoryLimit_02 = new Vector2(0f, base_yDisplacementRangeMin);
    //     Vector2 particleTrajectoryLimit_03 = new Vector2(0f, base_yDisplacementRangeMax);
        
    //     float ogAngle_00 = Mathf.Atan2(particleTrajectoryLimit_00.y, particleTrajectoryLimit_00.x);
    //     float ogAngle_01 = Mathf.Atan2(particleTrajectoryLimit_01.y, particleTrajectoryLimit_01.x);
    //     float ogAngle_02 = Mathf.Atan2(particleTrajectoryLimit_02.y, particleTrajectoryLimit_02.x);
    //     float ogAngle_03 = Mathf.Atan2(particleTrajectoryLimit_03.y, particleTrajectoryLimit_03.x);

    //     float currentAngle = transform.eulerAngles.z * Mathf.Deg2Rad;

    //     float newAngle_00 = ogAngle_00 + currentAngle;
    //     float newAngle_01 = ogAngle_01 + currentAngle;
    //     float newAngle_02 = ogAngle_02 + currentAngle;
    //     float newAngle_03 = ogAngle_03 + currentAngle;

    //     // Vector2 newTrajectoryLimit_00 = new Vector2(Mathf.Cos(newAngle_00) * xDisplacementRangeMin, Mathf.Sin(newAngle_00) * yDisplacementRangeMin);
    //     // Vector2 newTrajectoryLimit_01 = new Vector2(Mathf.Cos(newAngle_01) * xDisplacementRangeMax, Mathf.Sin(newAngle_01) * yDisplacementRangeMax);
    //     // Vector2 newTrajectoryLimit_02 = new Vector2(Mathf.Cos(newAngle_02) * xDisplacementRangeMin, Mathf.Sin(newAngle_02) * yDisplacementRangeMin);
    //     // Vector2 newTrajectoryLimit_03 = new Vector2(Mathf.Cos(newAngle_03) * xDisplacementRangeMax, Mathf.Sin(newAngle_03) * yDisplacementRangeMax);

    //     // xDisplacementRangeMin = Mathf.Min(newTrajectoryLimit_00.x, newTrajectoryLimit_01.x);
    //     // xDisplacementRangeMax = Mathf.Max(newTrajectoryLimit_00.x, newTrajectoryLimit_01.x);
    //     // yDisplacementRangeMin = Mathf.Min(newTrajectoryLimit_00.y, newTrajectoryLimit_01.y);
    //     // yDisplacementRangeMax = Mathf.Max(newTrajectoryLimit_00.y, newTrajectoryLimit_01.y);
        
    //     xDisplacementRangeMin = Mathf.Cos(newAngle_00) * base_xDisplacementRangeMin;
    //     xDisplacementRangeMax = Mathf.Cos(newAngle_01) * base_xDisplacementRangeMax;
    //     yDisplacementRangeMin = Mathf.Sin(newAngle_02) * base_yDisplacementRangeMin;
    //     yDisplacementRangeMax = Mathf.Sin(newAngle_03) * base_yDisplacementRangeMax;

    //     spawnOffsetX = Mathf.Cos(currentAngle) * base_spawnOffsetX;
    //     spawnOffsetY = Mathf.Sin(currentAngle) * base_spawnOffsetY;

    //     // float spawnRangeModifier = 0;
    //     // particleEmitter.SetSpawnRange(circleCollider.radius * spawnRangeModifier, circleCollider.radius * spawnRangeModifier);
    // }

    void CalculateRotationDisplacements(){
        Vector2 particleTrajectoryLimit_00 = new Vector2(base_xDisplacementRangeMin, base_yDisplacementRangeMin);
        Vector2 particleTrajectoryLimit_01 = new Vector2(base_xDisplacementRangeMin, base_yDisplacementRangeMax);
        
        float ogAngle_00 = Mathf.Atan2(particleTrajectoryLimit_00.y, particleTrajectoryLimit_00.x);
        float ogAngle_01 = Mathf.Atan2(particleTrajectoryLimit_01.y, particleTrajectoryLimit_01.x);

        float currentAngle = transform.eulerAngles.z * Mathf.Deg2Rad;

        float newAngle_00 = ogAngle_00 + currentAngle;
        float newAngle_01 = ogAngle_01 + currentAngle;

        Vector2 newTrajectoryLimit_00 = new Vector2(Mathf.Cos(newAngle_00), Mathf.Sin(newAngle_00)) * particleTrajectoryLimit_00.magnitude;
        Vector2 newTrajectoryLimit_01 = new Vector2(Mathf.Cos(newAngle_01), Mathf.Sin(newAngle_01)) * particleTrajectoryLimit_01.magnitude;

        xDisplacementRangeMin = Mathf.Min(newTrajectoryLimit_00.x, newTrajectoryLimit_01.x);
        xDisplacementRangeMax = Mathf.Max(newTrajectoryLimit_00.x, newTrajectoryLimit_01.x);
        yDisplacementRangeMin = Mathf.Min(newTrajectoryLimit_00.y, newTrajectoryLimit_01.y);
        yDisplacementRangeMax = Mathf.Max(newTrajectoryLimit_00.y, newTrajectoryLimit_01.y);
        
        // xDisplacementRangeMin = newTrajectoryLimit_00.x;
        // xDisplacementRangeMax = newTrajectoryLimit_01.x;
        // yDisplacementRangeMin = newTrajectoryLimit_00.y;
        // yDisplacementRangeMax = newTrajectoryLimit_01.y;

        spawnOffsetX = Mathf.Cos(currentAngle) * base_spawnOffsetX;
        spawnOffsetY = Mathf.Sin(currentAngle) * base_spawnOffsetY;

        // float spawnRangeModifier = 0;
        // particleEmitter.SetSpawnRange(circleCollider.radius * spawnRangeModifier, circleCollider.radius * spawnRangeModifier);
    }

    void UpdateLayerOrder(){
        if(!keepParticlesBehindObject){
            return;
        }

        for(var i = 0; i < particles.Length; i++){
            if(!particles[i].Active){
                continue;
            }

            particles[particleIndex].GetComponent<SpriteRenderer>().sortingOrder = orderParentObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
        }
    }
}
