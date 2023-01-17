using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class FillBar : MonoBehaviour
{
    public float FillPercent{
        get;
        set;
    }

    [SerializeField] GameObject innerBar;
    [SerializeField] GameObject outerBar;
    [SerializeField] bool showText;
    [SerializeField] bool deactivateOnZeroPercent;
    [SerializeField] Canvas canvas;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject parentObject;
    [SerializeField] bool alwaysStayUpright = true;
    float distanceFromUprightObject;
    [Header("Flash When Low Settings")]
    [SerializeField] bool flashWhenLow = false;
    [SerializeField] bool innerBarFlash = true;
    [SerializeField] bool outerBarFlash = true;
    [SerializeField] float lowFlashPercentage = .2f;
    [SerializeField] Color outerBarFlashColor = Color.yellow;
    [SerializeField] float flashTweenDuration = .25f;
    Color outerBarOgColor;
    bool flashing = false;
    Tween flashingTween_innerBar;
    Tween alphaRestoreTween_innerBar;
    Tween colorFlashTween_outerBar;
    Tween colorRestoreTween_outerBar;
    GameObject UI_Camera;
    [SerializeField] bool isUI = false;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        FillPercent = 1f;

        canvas.worldCamera = Camera.main;
        canvas.gameObject.SetActive(showText);
        
        if(isUI){
            UI_Camera = GameObject.Find("UI Camera");
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        if(alwaysStayUpright){
            distanceFromUprightObject = (transform.position - parentObject.transform.position).magnitude;
        }

        flashingTween_innerBar = innerBar.GetComponent<SpriteRenderer>().DOFade(0f, flashTweenDuration).SetLoops(-1, LoopType.Yoyo);
        flashingTween_innerBar.Pause();
        outerBarOgColor = outerBar.GetComponent<SpriteRenderer>().color;
        colorFlashTween_outerBar = outerBar.GetComponent<SpriteRenderer>().DOColor(outerBarFlashColor, flashTweenDuration).SetLoops(-1, LoopType.Yoyo);
        colorFlashTween_outerBar.Pause();
        // Color innerBarColor = innerBar.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCanvasOrder();
        if(isUI){
            // transform.position = UI_Camera.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(-4.72f, 9.72f, 0f));
        }

        UpdateStayUpright();
        UpdateLowFlashing();
    }

    void FixedUpdate()
    {
        UpdateBarWidth();
    }

    public void UpdateBarWidth()
    {
        float xScale = innerBar.transform.localScale.x - (innerBar.transform.localScale.x - FillPercent) * .5f;
        innerBar.transform.localScale = new Vector3(xScale, innerBar.transform.localScale.y, innerBar.transform.localScale.z);

        if(deactivateOnZeroPercent && innerBar.transform.localScale.x <= .005f)
        {
            // canvas.active = false;
            gameObject.SetActive(false);
        }
    }

    public void UpdateStayUpright(){
        if(!alwaysStayUpright){
            return;
        }

        transform.localEulerAngles = new Vector3(0f, 0f, -parentObject.transform.eulerAngles.z);
        Vector3 uprightPosition = new Vector3();
        uprightPosition.x = Mathf.Cos(Mathf.Deg2Rad * (transform.eulerAngles.z + 90)) * distanceFromUprightObject;
        uprightPosition.y = Mathf.Sin(Mathf.Deg2Rad * (transform.eulerAngles.z + 90)) * distanceFromUprightObject;
        uprightPosition.z = 0f;

        transform.localPosition = uprightPosition;
    }

    public void UpdateText(string newText)
    {
        text.text = newText;
    }

    private void UpdateCanvasOrder()
    {
        if(!parentObject){
            // canvas.GetComponent<Canvas>().sortingOrder = innerBar.GetComponent<SpriteRenderer>().sortingOrder + 1;
            return;
        }
        
        canvas.sortingOrder = parentObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        outerBar.GetComponent<SpriteRenderer>().sortingOrder = parentObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
        innerBar.GetComponent<SpriteRenderer>().sortingOrder = parentObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    private void UpdateLowFlashing(){
        if(!flashWhenLow){
            return;
        }

        if(FillPercent <= lowFlashPercentage){
            if(!flashing){
                if(innerBarFlash){
                    Color innerBarColor = innerBar.GetComponent<SpriteRenderer>().color;
                    innerBarColor.a = 1f;
                    if(alphaRestoreTween_innerBar != null){
                        alphaRestoreTween_innerBar.Kill();
                    }
                    flashingTween_innerBar.Restart();
                }

                if(outerBarFlash){
                    outerBar.GetComponent<SpriteRenderer>().color = outerBarOgColor;
                    if(colorRestoreTween_outerBar != null){
                        colorRestoreTween_outerBar.Kill();
                    }
                    colorFlashTween_outerBar.Restart();
                }

                flashing = true;
            }
        } else {
            if(flashing){
                if(innerBarFlash){
                    flashingTween_innerBar.Pause();
                    alphaRestoreTween_innerBar = innerBar.GetComponent<SpriteRenderer>().DOFade(1f, flashTweenDuration);
                }

                if(outerBarFlash){
                    colorFlashTween_outerBar.Pause();
                    colorRestoreTween_outerBar = outerBar.GetComponent<SpriteRenderer>().DOColor(outerBarOgColor, flashTweenDuration);
                }

                flashing = false;
            }
        }
    }
}
