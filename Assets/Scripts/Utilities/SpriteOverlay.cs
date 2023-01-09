using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpriteOverlay : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition(){
        if(!gameObject.activeSelf){
            return;
        }

        Vector3 newPosition = Camera.main.transform.position;
        newPosition.z = 0f;
        transform.position = newPosition;
    }

    public Tween TweenIn(float tweenDuration, int sortingOrder = -1000, bool startFromAlpha0 = true, float alpha = 1f){
        if(startFromAlpha0){
            Color newColor = spriteRenderer.color;
            newColor.a = 0;
            spriteRenderer.color = newColor;
        }

        spriteRenderer.sortingOrder = sortingOrder;
        return spriteRenderer.DOFade(alpha, tweenDuration);
    }

    public Tween TweenOut(float tweenDuration, float alpha = 0f){
        return spriteRenderer.DOFade(alpha, tweenDuration);
    }
}
