using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeExplosion : MonoBehaviour
{
    [SerializeField] Sprite[] smokeSprites;
    [SerializeField] float spriteScale = 1f;
    [Range(0, .5f)] [SerializeField] float spriteScaleVariance = .2f;
    [SerializeField] float animDuration = .5f;
    [SerializeField] float spreadLengthX = 1f;
    [SerializeField] float spreadLengthY = .5f;
    [Range(0, .5f)] [SerializeField] float spreadLengthVariance = .2f;
    [SerializeField] int amtOfSmokeObjs = 8;
    [Range(0, 4)] [SerializeField] int amtOfSmokeObjsVariance = 0;
    List<GameObject> smokeObjects;
    [SerializeField] Material alternateMaterial;
    [Range(0f, 1f)] [SerializeField] float maxAlpha = 1f;
    float alpha = 1f;
    [Range(0f, 1f)] [SerializeField] float fadeBeginPercent = .75f;
    float animDT = 0f;
    bool exploding = false;
    Vector3 originPosition;
    List<Vector2> smokeDirections;
    GameObject[] smokeObjPool;
    [SerializeField] Color color = Color.red;
    [SerializeField] AudioClip explosionSound;
    
    // Start is called before the first frame update
    void Start()
    {
        smokeObjects = new List<GameObject>();
        smokeDirections = new List<Vector2>();
        InitializeSmokeObjPool();
    }

    void InitializeSmokeObjPool(){
        smokeObjPool = new GameObject[amtOfSmokeObjs + amtOfSmokeObjsVariance];

        for(int i = 0; i < smokeObjPool.Length; i++){
            GameObject smokeObj = new GameObject($"smokeObj_{gameObject.name}_{i.ToString("D2")}");
            SpriteRenderer smokeObjSpriteRenderer = smokeObj.AddComponent<SpriteRenderer>();
            smokeObjSpriteRenderer.sprite = smokeSprites[Random.Range(0, smokeSprites.Length - 1)];
            if(alternateMaterial != null){
                smokeObjSpriteRenderer.material = alternateMaterial;
            }
            smokeObj.AddComponent<SortingOrderByY>();
            smokeObjPool[i] = smokeObj;
            smokeObj.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateExplosion();    
    }
    
    void UpdateExplosion(){
        if(!exploding){
            return;
        }

        animDT += Time.deltaTime;
        float animCompletionRatio = animDT / animDuration;

        if(animCompletionRatio >= fadeBeginPercent){
            float fadeLengthDenominator = 1f - fadeBeginPercent;
            float fadeLengthNumerator = animCompletionRatio - 1f + 1f - fadeBeginPercent;
            alpha = maxAlpha - maxAlpha * (fadeLengthNumerator / fadeLengthDenominator);
        }

        for(int i = 0; i < smokeObjects.Count; i++){
            smokeObjects[i].transform.position = new Vector3(originPosition.x + smokeDirections[i].x * animCompletionRatio, originPosition.y + smokeDirections[i].y * animCompletionRatio, 0f);
            smokeObjects[i].GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, alpha);
        }

        if(animDT >= animDuration){
            exploding = false;

            for(int i = smokeObjects.Count - 1; i >= 0; i--){
                GameObject smokeObj = smokeObjects[i];
                smokeObjects.Remove(smokeObj);
                smokeObj.SetActive(false);
                smokeDirections.RemoveAt(i);
            }

            smokeObjects = new List<GameObject>();
            smokeDirections = new List<Vector2>();
        }

        
    }

    public void SpawnExplosion(Vector3 originPosition, string layerName = "Default"){
        int randomSmokeAddition = (int)(amtOfSmokeObjsVariance * Random.Range(0f, 1f));
        float currAngle = 0f;

        for(int i = 0; i < amtOfSmokeObjs +  randomSmokeAddition; i++){
            GameObject smokeObj = smokeObjPool[i];
            float randomScaleModifier = Random.Range(0f, spriteScaleVariance) * (Random.Range(0, 1) * 2 - 1);
            smokeObj.transform.localScale = new Vector3(spriteScale + randomScaleModifier, spriteScale + randomScaleModifier, 1f);
            smokeObj.GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, maxAlpha);
            smokeObj.GetComponent<SpriteRenderer>().sortingLayerName = layerName;
            smokeObjects.Add(smokeObj);
            smokeObj.SetActive(true);

            float angleIncrement = Mathf.PI * 2f / (amtOfSmokeObjs +  randomSmokeAddition);
            currAngle = angleIncrement * i;
            smokeDirections.Add(new Vector2(Mathf.Cos(currAngle) * (spreadLengthX + spreadLengthVariance * Random.Range(0, 1f)), Mathf.Sin(currAngle) * (spreadLengthY + spreadLengthVariance * Random.Range(0, 1f))));
        }

        this.originPosition = originPosition;
        animDT = 0f;
        exploding = true;
        alpha = maxAlpha;
        GameMaster.Instance.audioSource.PlayOneShot(explosionSound, GameMaster.Instance.MasterVolume);
    }
}
