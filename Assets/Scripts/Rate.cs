using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rate : MonoBehaviour {

	[SerializeField] private bool clearPlayerPrefs;
	[SerializeField] private GameObject rateScreen;
	[SerializeField] private Button playButton;

	// Use this for initialization
	void Awake () {
		if(clearPlayerPrefs) ResetPlayerPrefs();
		if(CanShowRateScreen() && checkNetworkAvailability()) {
			ShowRateScreen();
		}
	}

	private void ResetPlayerPrefs(){
		PlayerPrefs.DeleteKey("HasPlayed");
		PlayerPrefs.DeleteKey("RateAskCounter");
		PlayerPrefs.Save();
	}

	private string getRateURL() {
		string rateURL = "";
		
		#if UNITY_ANDROID
        	string appId = "com.sylvangamestudios.PepperedWithMissiles"; //GooglePlay
        	rateURL = "market://details?id=" + appId; //Google Play
		#endif

		#if UNITY_STANDALONE_WIN
			rateURL = "https://play.google.com/store/apps/details?id=com.sylvangamestudios.PepperedWithMissiles";
		#endif

		#if UNITY_EDITOR
			rateURL = "https://play.google.com/store/apps/details?id=com.sylvangamestudios.PepperedWithMissiles";
		#endif

		return rateURL;
    }
	
	private void ShowRateScreen() {
		playButton.interactable = false;
		rateScreen.SetActive(true);
	}

	private bool CanShowRateScreen() {

		int hasPlayed = PlayerPrefs.GetInt("HasPlayed", 0);
 
		if(hasPlayed == 0) {	// First time playing
			PlayerPrefs.SetInt("HasPlayed", 1);
			PlayerPrefs.SetInt("RateAskCounter", 4);
			PlayerPrefs.Save();
			return false;
		} else {
			int rateAskCounter = PlayerPrefs.GetInt("RateAskCounter") - 1;

			if(rateAskCounter <= 0) {	// Show rate screen
				return true;
			} else {	// Counter hasn't reached 0
				PlayerPrefs.SetInt("RateAskCounter", rateAskCounter);
				PlayerPrefs.Save();
				return false;
			}
		}
	}

    private bool checkNetworkAvailability() {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

	public void RateButtonPressed() {
        // Open Rate URL
		playButton.interactable = true;
		PlayerPrefs.SetInt("RateAskCounter", 9999999);
		PlayerPrefs.Save();
        Application.OpenURL(getRateURL());
        rateScreen.SetActive(false);
	}

	public void MaybeLaterButtonPressed() {
		playButton.interactable = true;
		PlayerPrefs.SetInt("RateAskCounter", 10);
		PlayerPrefs.Save();
        rateScreen.SetActive(false);
	}
}
