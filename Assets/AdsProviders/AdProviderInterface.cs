using UnityEngine;
using System.Collections;

public abstract class AdProviderInterface : MonoBehaviour {
	
	abstract public bool IsVideoAvailable();
	abstract public void ShowVideo();
	abstract public bool IsInterstitialAvailable();
	abstract public void ShowInterstitial();
	abstract public bool IsBannerAvailable();
	abstract public void ShowBanner(bool onTop);
	abstract public void HideBanner();
}


