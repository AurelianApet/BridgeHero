using UnityEngine;
using System.Collections;
using GoogleMobileAds;

public class AdMobProvider : AdProviderInterface {

	private GoogleMobileAds.Api.InterstitialAd interstitial;
	private GoogleMobileAds.Api.BannerView banner;

	bool bannerIsLoaded = true;

	string BannerAdUnitID;
	string InterstitialAdUnitID;

	public void OnBannerLoad(object sender, System.EventArgs args)
	{

		bannerIsLoaded = true;

		if (banner != null) {
			banner.Show();
		}
	}

	public void OnBannerFailedToLoad(object sender, System.EventArgs args)
	{
		bannerIsLoaded = false;

		if (banner != null) {
			banner.Destroy();
			banner=null;
		}
	}
	public void OnInterstitialLoad(object sender, System.EventArgs args)
	{
		interstitial.Show ();
	}
	
	private void Awake()
	{
		string rawString = Resources.Load<TextAsset>("GenericAppData").text;
		IDictionary root = (IDictionary) MiniJSON.Json.Deserialize (rawString);
		IDictionary platform = null;
		#if UNITY_IPHONE
		platform = (IDictionary)root ["ios"];
		#else 
		#if UNITY_ANDROID
		platform = (IDictionary)root ["android"];
		#endif
		#endif

		if (platform != null) {
			if (platform.Contains ("admob_interstitial_adunit_id")) {
				InterstitialAdUnitID = platform ["admob_interstitial_adunit_id"].ToString ();
			}

			if (platform.Contains ("admob_banner_adunit_id")) {
				BannerAdUnitID = platform ["admob_banner_adunit_id"].ToString ();
			}

			CreateInterstitial();
		}
	}
	public void CreateInterstitial()
	{
		Debug.Log ("interstitial oluştu");
		interstitial = new GoogleMobileAds.Api.InterstitialAd (InterstitialAdUnitID);
//		interstitial.AdLoaded += OnInterstitialLoad;
		interstitial.LoadAd ((new GoogleMobileAds.Api.AdRequest.Builder ()).Build ());

	}
	override public bool IsVideoAvailable()
	{
		return false;
	}
	override public void ShowVideo()
	{
		return;
	}
	override public bool IsInterstitialAvailable()
	{
		return interstitial!=null && interstitial.IsLoaded ();
	}
	override public void ShowInterstitial()
	{
		Debug.Log ("interstiitla gösterildi");
		if (interstitial.IsLoaded() && interstitial!=null) {
			interstitial.Show ();
		}
		CreateInterstitial ();
	}

	override public bool IsBannerAvailable()
	{
		return bannerIsLoaded;
	}

	override public void ShowBanner(bool onTop)
	{
		banner = new GoogleMobileAds.Api.BannerView (BannerAdUnitID, GoogleMobileAds.Api.AdSize.SmartBanner,GoogleMobileAds.Api.AdPosition.Bottom);
		banner.AdLoaded += OnBannerLoad;
		banner.LoadAd ((new GoogleMobileAds.Api.AdRequest.Builder ()).Build ());
	}
	override public void HideBanner()
	{
		if (banner != null) {
			banner.Hide();
		}
	}

	public void onbannerstest()
	{
		ShowBanner (false);
	}
}