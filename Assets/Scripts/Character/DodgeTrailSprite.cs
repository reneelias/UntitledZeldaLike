using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DodgeTrailSprite : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public bool Active{
        protected set;
        get;
    } = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(Sprite sprite, Vector3 position, float startingAlpha, float fadeDuration, string sortingLayerName, int sortingOrder, Vector3? scale = null, Color? color = null){
        gameObject.SetActive(true);
        Active = true;

        spriteRenderer.sprite = sprite;
        Color newColor = color ?? Color.white;
        newColor.a = startingAlpha;
        spriteRenderer.color = newColor;
        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = sortingOrder;
        transform.localScale = scale ?? Vector3.one;
        transform.position = position;

        spriteRenderer.DOFade(0f, fadeDuration)
            .OnComplete(()=>{
                Active = false;
                gameObject.SetActive(false);
            });
    }
}
