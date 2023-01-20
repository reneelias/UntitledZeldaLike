using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class GameMaster : Singleton<GameMaster>
{
    public Dungeon dungeon;
    public AudioSource audioSource;
    [SerializeField] SpriteOverlay blackOverlay;
    [SerializeField] GameObject[] UI_canvasGroupsToFadeOut_gameOver;
    [SerializeField] GameObject[] UI_spritesToFadeOut_gameOver;
    [SerializeField] GameObject[] UI_buttonsToFadeIn_gameOver;
    [SerializeField] GameObject[] UI_textToFadeIn_gameOver;
    public TextPrompt mainTextPrompt;
    public bool GameOver{
        private set; get;
    }
    [SerializeField] float coinCount = 0f;
    [SerializeField] TextMeshProUGUI coinCountText;
    [SerializeField] GameObject player;
    public GameObject Player{
        protected set => player = value;
        get => player;
    }
    [SerializeField] Controls_UI controls_UI;
    public Controls_UI Controls_UI{
        protected set => controls_UI = value;
        get => controls_UI;
    }
    [SerializeField] float gameOverResetDelay = 1f;
    [SerializeField] GameObject restartButton;
    public Crosshair crosshair;
    [SerializeField] EndOfDemo_UI endOfDemo_UI;
    [SerializeField] float endOfDemoUiFadeDuration = 2f;
    [SerializeField] float endOfDemoUiFadeDelay = 1f;
    bool demoFinished = false;
    public bool DemoFinished{
        protected set => demoFinished = value;
        get => demoFinished;
    }
    [SerializeField] EventSystem eventSystem;
    public EventSystem EventSystem{
        protected set => eventSystem = value;
        get => eventSystem;
    }
    public bool UI_FadedOut{
        protected set;
        get;
    }
    [SerializeField] float darknessMultiplier = 1f;
    [SerializeField] float darknessSliderRange = .12f;
    public float DarknessSliderRange{
        get => darknessSliderRange;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(Application.platform == RuntimePlatform.Android) {
            Camera.main.orthographicSize = 5f;
        } else {
            // GameObject.Find("JoystickCanvas").SetActive(false);
            Application.targetFrameRate = -1;
            // QualitySettings.vSyncCount = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGameOver(){
        SetBlackOverlay();

        float fadeTime = 1f;


        FadeInSpriteOverlay(fadeTime);

        // character.gameObject.layer = 5;
        SpriteRenderer characterSpriteRenderer = player.GetComponent<SpriteRenderer>();
        characterSpriteRenderer.sortingLayerName = "UnderUI";
        player.GetComponent<SortingOrderByY>().enabled = false;
        characterSpriteRenderer.sortingOrder = 0;
        dungeon.PlayerDied();

        FadeOutUI();
        FadeInUI_GameOver();

        GameOver = true;
        crosshair.SetCrosshairVisible(false);
        crosshair.SetCursorVisible(true);
    }

    public void ResetGameOver(){
        dungeon.ResetAfterDeath(gameOverResetDelay * .5f);
        foreach(GameObject gameObject in UI_buttonsToFadeIn_gameOver){
            gameObject.GetComponent<Button>().interactable = false;
        }

        DOVirtual.DelayedCall(gameOverResetDelay, ()=>{        
            GameOver = false;
            SetBlackOverlay(false);
            FadeInUI(.25f);
            
            foreach(GameObject buttonObject in UI_buttonsToFadeIn_gameOver){
                buttonObject.SetActive(false);
            }

            foreach(GameObject textObject in UI_textToFadeIn_gameOver){
                textObject.SetActive(false);
            }

            dungeon.Camera.SetCameraOnPlayerPosition();
            crosshair.SetCrosshairVisible(true);
        });
        
        crosshair.SetCursorVisible(false);
    }

    public void EndOfDemo(){
        DemoFinished = true;
        endOfDemo_UI.gameObject.SetActive(true);
        endOfDemo_UI.GetComponent<CanvasGroup>().alpha = 0f;
        endOfDemo_UI.GetComponent<CanvasGroup>().DOFade(1f, endOfDemoUiFadeDuration).SetDelay(endOfDemoUiFadeDelay)
            .OnComplete(()=>{
                endOfDemo_UI.EnableSubObjects();
            });
        
        crosshair.SetCrosshairVisible(false);
        crosshair.SetCursorVisible(true);
    }

    public void SetBlackOverlay(bool enable = true, bool fadeIn = false, float fadeTime = 1f){
        blackOverlay.gameObject.SetActive(enable);

        if(!enable){
            return;
        }

        blackOverlay.transform.localScale = new Vector3(Screen.width * 2f, Screen.height * 2f, 1f);
        blackOverlay.transform.position = Camera.main.transform.position;

        if(fadeIn){
            FadeInSpriteOverlay(fadeTime);
        } else {
            blackOverlay.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 1f);
        }
    }

    public void SetDarknessMultiplier(float darknessMultiplier){
        dungeon.SetDarknessValue(darknessMultiplier);
    }

    public void ResetScene(){
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);

        
        // if(Application.platform == RuntimePlatform.Android) {
        //     Camera.main.orthographicSize = 5f;
        // } else {
        //     GameObject.Find("JoystickCanvas").SetActive(false);
        // }
    }

    public void InputSchemeChange(string scheme){
        // if(scheme == "Gamepad"){

        // } else if(scheme == "Keyboard_Mouse"){

        // }

        crosshair.SetInputeScheme(scheme);
    }

    public void UpdateCoinCount(int deltaCount){
        coinCount = (int)Mathf.Clamp(coinCount + deltaCount, 0, 199);
        coinCountText.text = ":" + coinCount.ToString();
    }

    public void FadeOutUI(float fadeTime = 1f){
        foreach(GameObject gameObject in UI_canvasGroupsToFadeOut_gameOver){
            gameObject.GetComponent<CanvasGroup>().DOFade(0f, fadeTime);
        }

        foreach(GameObject gameObject in UI_spritesToFadeOut_gameOver){
            gameObject.GetComponent<SpriteRenderer>().DOFade(0f, fadeTime);
        }

        crosshair.GetComponent<SpriteRenderer>().DOFade(0f, fadeTime);
        UI_FadedOut = true;
    }

    public void FadeInUI(float fadeTime = 1f){
        foreach(GameObject gameObject in UI_canvasGroupsToFadeOut_gameOver){
            gameObject.GetComponent<CanvasGroup>().DOFade(1f, fadeTime);
        }

        foreach(GameObject gameObject in UI_spritesToFadeOut_gameOver){
            gameObject.GetComponent<SpriteRenderer>().DOFade(1f, fadeTime);
        }

        crosshair.GetComponent<SpriteRenderer>().DOFade(1f, fadeTime);
        UI_FadedOut = false;
    }

    public void FadeInUI_GameOver(float fadeTime = 1f){
        // foreach(GameObject gameObject in UI_buttonsToFadeIn_gameOver){
        //     gameObject.SetActive(true);
        //     Color currColor = gameObject.GetComponent<Image>().color;
        //     currColor.a = 0f; 
        //     gameObject.GetComponent<Image>().color = currColor;
        //     gameObject.GetComponent<Image>().DOFade(1f, fadeTime);
        //     gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 0f; 
        //     gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(1f, fadeTime);
        // }

        foreach(GameObject gameObject in UI_textToFadeIn_gameOver){
            gameObject.SetActive(true);
            gameObject.GetComponent<TextMeshProUGUI>().alpha = 0f;
            // textColor.a = 0f;
            // gameObject.GetComponent<Text>().color = textColor;
            gameObject.GetComponent<TextMeshProUGUI>().DOFade(1f,fadeTime);
        }

        DOVirtual.DelayedCall(fadeTime * 2f, ()=>{
            foreach(GameObject buttonObject in UI_buttonsToFadeIn_gameOver){
                buttonObject.SetActive(true);
                Color currColor = buttonObject.GetComponent<Image>().color;
                currColor.a = 0f; 
                buttonObject.GetComponent<Image>().color = currColor;
                buttonObject.GetComponent<Image>().DOFade(1f, fadeTime)
                    .OnComplete(()=>{
                        eventSystem.SetSelectedGameObject(restartButton);
                    });
                buttonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().alpha = 0f; 
                buttonObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().DOFade(1f, fadeTime);
                buttonObject.GetComponent<Button>().interactable = true;
            }
        });
    }

    public Tween FadeInSpriteOverlay(float fadeTime, bool setToScreenSize = true){
        if(setToScreenSize){
            blackOverlay.transform.localScale = new Vector3(Screen.width * 2f, Screen.height * 2f, 1f);
            // gameOverOverlay.transform.position = Camera.main.transform.position;
        }
        blackOverlay.gameObject.SetActive(true);
        return blackOverlay.TweenIn(fadeTime);  
    }

    public Tween FadeOutSpriteOverlay(float fadeTime, bool deactivateObject = true){
        return blackOverlay.TweenOut(fadeTime)
            .OnComplete(()=>{
                if(deactivateObject){
                    blackOverlay.gameObject.SetActive(false);
                }
            });
    }
}
