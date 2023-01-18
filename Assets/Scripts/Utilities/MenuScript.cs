using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public static bool GamePaused = false;
    // PlayerControls controls;
    [SerializeField] CharacterControls characterControls;
    [SerializeField] Pause_UI Pause_UI;
    [SerializeField] Controls_UI Controls_UI;
    [SerializeField] GameObject pauseSelectedButton;

    void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
        // characterControls.Controls.Gameplay.Pause.performed += ctx => SetPause();
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateControls();
    }

    void UpdateControls(){
        if(Input.GetKeyDown("escape")){
        // if(controls.Gameplay.Pause.triggered){
            SetPause();
        }
    }

    public void SetPause(){
        if(GameMaster.Instance.DemoFinished || GameMaster.Instance.GameOver){
            return;
        }
        
        if(GamePaused){
            Resume();
        } else {
            Pause();
        }
    }

    public void Resume(){
        GamePaused = false;
        Time.timeScale = 1f;
        Pause_UI.gameObject.SetActive(false);
        GameMaster.Instance.crosshair.SetCrosshairVisible(true);
        GameMaster.Instance.crosshair.SetCursorVisible(false);
    }
    
    public void Pause(){
        GamePaused = true;
        Time.timeScale = 0f;
        Pause_UI.gameObject.SetActive(true);
        GameMaster.Instance.crosshair.SetCrosshairVisible(false);
        GameMaster.Instance.crosshair.SetCursorVisible(true);
        GameMaster.Instance.EventSystem.SetSelectedGameObject(pauseSelectedButton);
    }

    public void ExitGame(){
        Debug.Log("Exiting Game...");
        Application.Quit();
    }

    public void ShowControlsGUI(bool show){
        Controls_UI.gameObject.SetActive(show);
    }

    public void SetVSync(bool on){
        QualitySettings.vSyncCount = on ? 1 : 0;
    }
}