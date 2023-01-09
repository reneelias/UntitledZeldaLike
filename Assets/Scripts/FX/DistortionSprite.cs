using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistortionSprite : MonoBehaviour
{
    [SerializeField] Material[] materials;
    [SerializeField] bool randomizeMaterial = true;
    [SerializeField] SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        if(randomizeMaterial){
            AssignRandomMaterial();
        }
    }

    void AssignRandomMaterial(){
        int randomNum = Random.Range(0, materials.Length);
        spriteRenderer.material = materials[randomNum];
        // spriteRenderer.material.name;
        // Debug.Log($"SpriteRenderer material: {spriteRenderer.material.name}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
