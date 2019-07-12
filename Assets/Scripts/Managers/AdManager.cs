using UnityEngine;
using UnityEngine.UI;
#if UNITY_ADS
using UnityEngine.Advertisements; // only compile Ads code on supported platforms
#endif

public class AdManager : MonoBehaviour {

    public static AdManager Instance { get; private set; }

    public bool testMode = true;
    public Button adButton;

    private string rewardedPlacementId = "rewardedVideo";
    private string gameId = "2976462";

    void Start() {
        Instance = this;

        #if UNITY_ADS
        if (Advertisement.isSupported) {
            Advertisement.Initialize(gameId, true);
        }
        #endif

        if (adButton) {
            adButton.onClick.AddListener(ShowRewardedAd);
        }
    }

    public void ShowRewardedAd() {

        #if UNITY_ADS
        if (!Advertisement.IsReady(rewardedPlacementId))
        {
            Debug.Log(string.Format("Ads not ready for placement '{0}'", rewardedPlacementId));
            return;
        }

        var options = new ShowOptions { resultCallback = HandleShowResult };
        Advertisement.Show(rewardedPlacementId, options);
        #endif
        
    }

    #if UNITY_ADS
    void HandleShowResult(ShowResult result) {
        if (result == ShowResult.Finished) {
            GameManager.Instance.Revive();
        } else if (result == ShowResult.Skipped) {
            Debug.LogWarning ("The player skipped the video - DO NOT REWARD!");
            GameManager.Instance.FinishClearingScene();
        } else if (result == ShowResult.Failed) {
            Debug.LogError ("Video failed to show");
            GameManager.Instance.FinishClearingScene();
        }
    }
    #endif

}
