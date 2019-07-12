using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    [Header("---| Menu Settings |---")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseCountdown;
    [SerializeField] private GameObject reviveUI;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private ScrollBackground ground;
    [SerializeField] private ScrollBackground clouds;

    [Header("---| Game Settings |---")]
    [SerializeField] private PickupSpawner pickupSpawner;
    [SerializeField] private MissileSpawner missileSpawner;

    [Header("---| Attributes |---")]
    [SerializeField] private GameObject cameraHolder;
    [SerializeField] private AdManager adManager;
    [SerializeField] private GameObject uiSoundSystem;
	[SerializeField] public GameObject explosionParent;
	[SerializeField] public GameObject missileParent;
    [SerializeField] public GameObject planeObject;

    [Header("---| Sounds |---")]
    [SerializeField] private AudioClip uiClickSound;

    [Header("---| Editor Only |---")]
    [SerializeField] private bool clearPlayerPrefs = false;

    //private bool destroyed = false;
    private bool firstDeath = true; // Represents if it's the first time a player has died in a run
    private bool firstTimePlaying = false;
    private float level = 0;

    void Awake(){
        Instance = this;
        if(clearPlayerPrefs) PlayerPrefs.DeleteAll();
        firstTimePlaying = PlayerPrefs.GetInt("returningPlayer", 0) == 0;  // 0 for first time players, else for returningPlayers
        planeObject = GameObject.FindGameObjectWithTag("Player").gameObject;
        ground.parallaxFactor = 250;
        clouds.parallaxFactor = 170;
    }

    // Called when the game is minimised
    void OnApplicationPause(bool pauseStatus)
    {
        // If the player is currently playing, pause
        if(gameUI.activeInHierarchy == true) {
            PauseGame();
        }
    }

    public void StartGame(){
        ground.parallaxFactor = 120;
        clouds.parallaxFactor = 100;
        planeObject.SetActive(true);
        gameOverScreen.SetActive(false);
        gameUI.SetActive(true);
        cameraHolder.GetComponent<CameraFollow>().follow = true;
        ScoreManager.Instance.CurrentScore = 0;
        StartCoroutine(ZoomOutCamera());
        if(firstTimePlaying){   //Show Tutorial
            firstTimePlaying = false;
            PlayerPrefs.SetInt("returningPlayer", 1);
            pauseButton.SetActive(false);
            tutorialUI.SetActive(true);
        } else {
            planeObject.GetComponent<Movement>().CanMove = true;
            pickupSpawner.StartSpawning();
            missileSpawner.StartSpawning();
        }
    }

    private IEnumerator ZoomOutCamera(){
        yield return new WaitForSeconds(3);
        cameraHolder.GetComponent<CameraFollow>().start = false;
    }

    public void DoDeathLogic(){
        level = missileSpawner.StopSpawning() / 2;
        pickupSpawner.StopSpawning();
        if(firstDeath){
            firstDeath = false;
            gameUI.SetActive(false);
            ScoreManager.Instance.SetReviveScore();
            reviveUI.SetActive(true);
        } else {
            Invoke("FinishClearingScene", 2.0f);
        }
    }

    public void FinishClearingScene(){
        planeObject.GetComponent<Movement>().CanMove = false;
        cameraHolder.GetComponent<CameraFollow>().follow = false;
        
        gameUI.SetActive(false);
        ScoreManager.Instance.SetGameOverScore();
        gameOverScreen.SetActive(true);

        planeObject.transform.position = gameObject.transform.position;
        
        //destroyed = false;
        firstDeath = true;
        ClearScene();
    }

    // Clear Scene should only be called right before a new game is starting
    private void ClearScene() {

        //Clear pickups
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
        foreach (GameObject pickup in pickups) {
            Destroy(pickup);
        }

        //ClearParticleEffects();
    }

    private void ClearParticleEffects(){
        foreach (Transform child in explosionParent.transform) {
            Destroy(child.gameObject);
        }
    }

    // Called after an ad has been watched
    public void Revive(){
        //ClearParticleEffects();
        planeObject.SetActive(true);
        reviveUI.SetActive(false);
        pauseUI.SetActive(true);
        pauseMenu.SetActive(false);
        pauseCountdown.SetActive(true);
        StartCoroutine(DoRevivalCountdown());
    }

    private void ExplodeAllMissiles(){
        GameObject[] missiles = GameObject.FindGameObjectsWithTag("Missile");
        foreach (GameObject missile in missiles) {
            SmartMissile sm = missile.GetComponent<SmartMissile>();
            if(sm){
                sm.SelfDestruct();
            } else {
                DumbMissile dm = missile.GetComponent<DumbMissile>();
                dm.SelfDestruct();
            }
        }
    }

    IEnumerator DoRevivalCountdown(){
        TextMeshProUGUI text = pauseCountdown.GetComponent<TextMeshProUGUI>();
        AudioSource audioSource = uiSoundSystem.GetComponent<AudioSource>();
        audioSource.PlayOneShot(uiClickSound);
        yield return new WaitForSecondsRealtime(1);
        audioSource.PlayOneShot(uiClickSound);
        text.SetText("2");
        yield return new WaitForSecondsRealtime(1);
        audioSource.PlayOneShot(uiClickSound);
        text.SetText("1");
        yield return new WaitForSecondsRealtime(1);
        audioSource.PlayOneShot(uiClickSound);
        pauseCountdown.SetActive(false);
        gameUI.SetActive(true);
        missileSpawner.StartSpawning(level);
        pickupSpawner.StartSpawning();
        planeObject.GetComponent<Movement>().CanMove = true;
        text.SetText("3");
    }

    // ------------------ Buttons ------------------ //

    public void PlayButtonPressed() {
        mainMenu.SetActive(false);
        StartGame();
    }

    public void MuteButtonPressed(GameObject image) {
        AudioListener.pause = !AudioListener.pause;
        image.SetActive(!image.activeInHierarchy);
    }

    public void PauseGame() {
        if(Time.timeScale == 1) {
            gameUI.SetActive(false);
            Time.timeScale = 0;
            pauseUI.SetActive(true);
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame(){
        if(Time.timeScale == 0) {
            pauseMenu.SetActive(false);
            pauseCountdown.SetActive(true);
            StartCoroutine(ResumeGameRoutine());
        }
    }

    private IEnumerator ResumeGameRoutine() {
        TextMeshProUGUI text = pauseCountdown.GetComponent<TextMeshProUGUI>();
        AudioSource audioSource = uiSoundSystem.GetComponent<AudioSource>();
        yield return new WaitForSecondsRealtime(1);
        audioSource.PlayOneShot(uiClickSound);
        text.SetText("2");
        yield return new WaitForSecondsRealtime(1);
        audioSource.PlayOneShot(uiClickSound);
        text.SetText("1");
        yield return new WaitForSecondsRealtime(1);
        audioSource.PlayOneShot(uiClickSound);
        Time.timeScale = 1;
        pauseCountdown.SetActive(false);
        pauseUI.SetActive(false);
        gameUI.SetActive(true);
        text.SetText("3");
    }

    public void ReviveButtonPressed() {
        #if UNITY_ADS
        AdManager.Instance.ShowRewardedAd();
        #endif
        #if UNITY_STANDALONE_WIN
        Revive();
        #endif
    }

    public void SkipButtonPressed() {
        reviveUI.SetActive(false);
        FinishClearingScene();
    }

    public void TutorialReadyButtonPressed() {
        tutorialUI.SetActive(false);
        pauseButton.SetActive(true);
        planeObject.GetComponent<Movement>().CanMove = true;
        pickupSpawner.StartSpawning();
        missileSpawner.StartSpawning();
    }

    /*public void ReturnToMainMenu(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }*/

}
