using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DrawAboveObject : MonoBehaviour
{
    [SerializeField] GameObject referenceObject;
    [SerializeField] bool useSortingGroup = false;
    [SerializeField] SortingGroup sortingGroup;
    [SerializeField] SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {if(useSortingGroup){
            sortingGroup.sortingLayerName = referenceObject.GetComponent<SpriteRenderer>().sortingLayerName;
        } else {
            spriteRenderer.sortingLayerName = referenceObject.GetComponent<SpriteRenderer>().sortingLayerName;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(useSortingGroup){
            sortingGroup.sortingOrder = referenceObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        } else {
            spriteRenderer.sortingOrder = referenceObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
    }
}
