using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterPadRing : MonoBehaviour
{
    bool active = false;
    float currentScale = 1f;
    float animationTime = .5f;
    float animationDT = 0f;
    float animationStep = 0f;
    float alphaStepMultiplier = 4f;
    [SerializeField] SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Initialize(){
        active = false;
        spriteRenderer.color = new Color(0f, 0f, 0f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        UpdateAnimation();
    }

    void UpdateAnimation(){
        if(!active){
            return;
        }

        currentScale -= animationStep;
        transform.localScale = new Vector3(currentScale, currentScale, currentScale);
        if(spriteRenderer.color.a < 1f){
            Color newColor = spriteRenderer.color;
            float newAlpha = newColor.a + animationStep * alphaStepMultiplier;
            if(newAlpha > 1f){
                newAlpha = 1f;
            }

            newColor.a = newAlpha;
            spriteRenderer.color = newColor;
        }

        if(currentScale <= 0f){
            active = false;
        }
    }

    public bool Activate(float animationTime, int sortingOrder, Color color, float alphaStepMultiplier){
        if(active){
            return false;
        }        
        
        active = true;
        currentScale = 1f;
        transform.localScale = Vector3.one;
        Color newColor = color;
        // Color newColor = Color.white;
        newColor.a = 0f;
        spriteRenderer.color = newColor;
        this.animationTime = animationTime;
        animationStep = 1f / (animationTime / Time.fixedDeltaTime);
        spriteRenderer.sortingOrder = sortingOrder;
        this.alphaStepMultiplier = alphaStepMultiplier;

        return true;
    }

}
