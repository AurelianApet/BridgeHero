using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using MiniJSON;

public class MenuLogic : MonoBehaviour {
	
	string LeaderboardId;
	string AppleAppID;

	public Text Wallet;
	
	public UnityEngine.UI.Text _lastScore=null;
	public UnityEngine.UI.Text _bestScore=null;

	public GameObject RemoveAdsButton = null;
	public GameObject RestoreAdsButton = null;

    public GameObject GameOverPanel = null;

	public bool waitingContinue;

	// Use this for initialization
	void Start () {

		string rawString = Resources.Load<TextAsset>("GenericAppData").text;
		IDictionary root = (IDictionary) MiniJSON.Json.Deserialize (rawString);
		IDictionary platform = null;
		#if UNITY_IPHONE
		 platform = (IDictionary)root ["ios"];
		AppleAppID = platform["appid"].ToString ();
		#else 
		#if UNITY_ANDROID
		 platform = (IDictionary)root ["android"];
		#endif
		#endif
		if (platform != null) {
			LeaderboardId = platform ["leaderboardid"].ToString ();
		}
		UpdateCoins();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static bool IsAdRemoved()
	{
		return PlayerPrefs.GetInt("ad_removed")==1;
	}

	public void updateStates()
	{
		int best_score = PlayerPrefs.GetInt ("best_score");
		int last_score = PlayerPrefs.GetInt ("last_score");

		UpdateCoins();

		_lastScore.text = last_score.ToString ();
		_bestScore.text = best_score.ToString ();

		RemoveAdsButton.SetActive(PlayerPrefs.GetInt("ad_removed")==0);
		RestoreAdsButton.SetActive (PlayerPrefs.GetInt ("ad_removed") == 0);
	}

	public static void RemoveAd()
	{
		PlayerPrefs.SetInt ("ad_removed", 1);
		PlayerPrefs.Save ();
	}

	public void OnRestart(Object sender)
	{
		MenuLogic.PlaySFX ();
		GetComponent<HeroGame> ().RestartGame ();
		
	}
	public void OnContinue(Object sender)
	{
		MenuLogic.PlaySFX ();
		int coins = PlayerPrefs.GetInt ("Coins");
		if (coins > 10) {
			PlayerPrefs.SetInt ("Coins", coins - 10);
			PlayerPrefs.Save ();
			GetComponent<HeroGame> ().ContinueGame ();
		} else {
			waitingContinue = true;
			GetComponent<InAppShop> ().OnBuy25Coins ();
		}
	}
	public void onPurchased()
	{
		if (waitingContinue) {
			waitingContinue = false;
			OnContinue (null);
		}
	}
    public static void PlaySFX(bool Positive=true)
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>(Positive ? "Sounds/dingCling-neutral" : "Sounds/dingCling-negative"), 1);
    }

	

	public void OnLeaderboard()
	{
		MenuLogic.PlaySFX ();
		Social.localUser.Authenticate((bool success) => {
			int last_score = PlayerPrefs.GetInt ("last_score");


			Social.ReportScore(last_score, LeaderboardId, (bool asuccess) => {
			});
#if UNITY_ANDROID
			((PlayGamesPlatform) Social.Active).ShowLeaderboardUI(LeaderboardId);
			#endif
#if UNITY_IPHONE
			Social.ShowLeaderboardUI();
#endif
		});
	}
	
	public void onShare()
	{
		MenuLogic.PlaySFX ();
		#if UNITY_ANDROID

		AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent");
		intentObject.Call<AndroidJavaObject> ("setAction", intentClass.GetStatic<string> ("ACTION_SEND"));
		intentObject.Call<AndroidJavaObject> ("setType", "text/plain");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Playing android game");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), 
		                                     "Hey. I'm playing that small game and it's enjoing me.\n"+
		                                     "It's called Bridge Hero and can be downloaded from the google play store.\n"+
		                                     "market://details?id=com.lugesoft.bridges"
		                                     );
		AndroidJavaClass unity = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject> ("currentActivity");
		currentActivity.Call ("startActivity", intentObject);
		#endif

#if UNITY_IPHONE
		IOSSocialManager.instance.ShareMedia("Hey. I'm playing that small game and it's enjoing me.\n"+
		                                     "It's called Bridge Hero and can be downloaded from the AppStore.\n"+
		                                     "itms-apps://itunes.apple.com/app/id"+AppleAppID);
#endif
	}

	public void OnRate()
	{
		MenuLogic.PlaySFX ();
		#if UNITY_ANDROID
		Application.OpenURL("market://details?id=com.lugesoft.bridges");
		#endif

#if UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/id"+AppleAppID);
#endif
	}

	public void OnRemoveAds()
	{
		MenuLogic.PlaySFX ();

	}

    public GameObject ChangeCharacterMenu;

    public void OnChangeCharacter(){
        MenuLogic.PlaySFX();
        ChangeCharacterMenu.GetComponent<CharacterShopLogic>().menuLogic = this;
      
        pushMenu(ChangeCharacterMenu);
        GameOverPanel.SetActive(false);
    }

    // Simple Navigation pattern

    List<GameObject> _callstack = new List<GameObject>();
    public void pushMenu(GameObject go)
    { 
        if (_callstack.Count > 0)
            _callstack[_callstack.Count - 1].SetActive(false);

        _callstack.Add(go);
        go.SetActive(true);
    }

    public void popMenu()
    {
        if (_callstack.Count > 1)
        {
            _callstack[_callstack.Count - 1].SetActive(false);
            _callstack.RemoveAt(_callstack.Count - 1);
            _callstack[_callstack.Count - 1].SetActive(true);
        }
    }
	public void UpdateCoins()
	{
		int Coins = PlayerPrefs.GetInt("Coins");
		Wallet.text = Coins.ToString();
	}
}
