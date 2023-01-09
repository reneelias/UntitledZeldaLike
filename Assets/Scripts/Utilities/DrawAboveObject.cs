using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAboveObject : MonoBehaviour
{
    [SerializeField] GameObject referenceObject;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = referenceObject.GetComponent<SpriteRenderer>().sortingLayerName;
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sortingOrder = referenceObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
}
