using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using OnePF;

public class InAppShop : MonoBehaviour {

	public GameObject RemoveAdsButton;
	public GameObject RestoreAdButton;

	public GameObject ProcessingRequest;
	bool _processingPayment = false;
	public Text Wallet;
    public CharacterShopLogic CharacterShopComponent;

	private void Awake()
	{
		// Subscribe to all billing events
		OpenIABEventManager.billingSupportedEvent += OnBillingSupported;
		OpenIABEventManager.billingNotSupportedEvent += OnBillingNotSupported;
		OpenIABEventManager.queryInventorySucceededEvent += OnQueryInventorySucceeded;
		OpenIABEventManager.queryInventoryFailedEvent += OnQueryInventoryFailed;
		OpenIABEventManager.purchaseSucceededEvent += OnPurchaseSucceded;
		OpenIABEventManager.purchaseFailedEvent += OnPurchaseFailed;
		OpenIABEventManager.transactionRestoredEvent += OnTransactionRestored;
		OpenIABEventManager.restoreSucceededEvent += OnRestoreSucceeded;
		OpenIABEventManager.restoreFailedEvent += OnRestoreFailed;
	}

	const string SKU_REMOVE_ADS = "com.lugesoft.bridges.removeads";
	const string SKU_25_COINS = "com.lugesoft.bridges.25coins";

	private void Start()
	{
		// Map SKUs for iOS
		OpenIAB.mapSku(SKU_REMOVE_ADS, OpenIAB_iOS.STORE, "com.lugesoft.bridges.removeads");
		OpenIAB.mapSku(SKU_25_COINS, OpenIAB_iOS.STORE, "com.lugesoft.bridges.25coins");

		// Map SKUs for Google Play
		OpenIAB.mapSku(SKU_REMOVE_ADS, OpenIAB_Android.STORE_GOOGLE, "com.lugesoft.bridges.removeads");
		OpenIAB.mapSku(SKU_25_COINS, OpenIAB_Android.STORE_GOOGLE, "com.lugesoft.bridges.25coins");

		// Map Sku for Amazon
		OpenIAB.mapSku(SKU_REMOVE_ADS, OpenIAB_Android.STORE_AMAZON, "com.lugesoft.bridges.removeads");
		OpenIAB.mapSku(SKU_25_COINS, OpenIAB_Android.STORE_AMAZON, "com.lugesoft.bridges.25coins");

		// Set some library options
		var options = new OnePF.Options();
		
		options.checkInventory = false;
		options.verifyMode = OptionsVerifyMode.VERIFY_ONLY_KNOWN;
		
		// Add Google Play public key
		options.storeKeys.Add(OpenIAB_Android.STORE_GOOGLE, "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEApUV5Y56BJYtD4j/q448aSStbKb0WWWOCC/NvrHO2RAWgPjMtNlAdh270O8MwZ68awQoYX9Wwjx3DwvwaEkhnftH4PUHcxT2ZAPadqNrFBNGRQbmedFQl2Jns3Ed4eT86MSygOyNht6zSma2ZlxMujAmRaysFED9CUuowfEULS4MZrQ/uvTKkdY96fKeptYptfZr8thzh6twMKrduxsxMZ5/rT66+z4RmXnE8dKDdu5g30RRWNjlwHs9AgcImYGDIWB/eehs2JGKJVuJ8i/mXZIpvQBKzqqLmgR1fqhU/pIeAwbK86DlXhaAoPDxB7zUQFlOOWPcqrMV1WKW+PRkeaQIDAQAB");
		options.availableStoreNames=new string[]{OpenIAB_Android.STORE_GOOGLE,OpenIAB_Android.STORE_AMAZON};
		options.prefferedStoreNames=new string[]{OpenIAB_Android.STORE_GOOGLE,OpenIAB_Android.STORE_AMAZON};
		options.storeSearchStrategy = SearchStrategy.INSTALLER_THEN_BEST_FIT;
		OpenIAB.init(options);
	}
	
	private void OnDestroy()
	{
		// Unsubscribe to avoid nasty leaks
		OpenIABEventManager.billingSupportedEvent -= OnBillingSupported;
		OpenIABEventManager.billingNotSupportedEvent -= OnBillingNotSupported;
		OpenIABEventManager.queryInventorySucceededEvent -= OnQueryInventorySucceeded;
		OpenIABEventManager.queryInventoryFailedEvent -= OnQueryInventoryFailed;
		OpenIABEventManager.purchaseSucceededEvent -= OnPurchaseSucceded;
		OpenIABEventManager.purchaseFailedEvent -= OnPurchaseFailed;
		OpenIABEventManager.transactionRestoredEvent -= OnTransactionRestored;
		OpenIABEventManager.restoreSucceededEvent -= OnRestoreSucceeded;
		OpenIABEventManager.restoreFailedEvent -= OnRestoreFailed;
	}

	public void OnEnable()
	{
		ProcessingRequest.SetActive (_processingPayment == true);
	}

	#region Billing
	
	private void OnBillingSupported()
	{
		Debug.Log("Billing is supported");
		OpenIAB.queryInventory(new string[] { SKU_REMOVE_ADS });
		RemoveAdsButton.SetActive (MenuLogic.IsAdRemoved () == false);
		RestoreAdButton.SetActive (MenuLogic.IsAdRemoved () == false);
	}
	
	private void OnBillingNotSupported(string error)
	{
		Debug.Log("Billing not supported: " + error);
		RemoveAdsButton.SetActive (false);
		RestoreAdButton.SetActive (false);
	}
	
	private void OnQueryInventorySucceeded(Inventory inventory)
	{
		Debug.Log("Query inventory succeeded: " + inventory);
		
		// Do we have the infinite ammo subscription?
		Purchase removeAdPurchase = inventory.GetPurchase(SKU_REMOVE_ADS);

		if (removeAdPurchase != null) {
			MenuLogic.RemoveAd();
		}

	}
	
	private void OnQueryInventoryFailed(string error)
	{
		Debug.Log("Query inventory failed: " + error);
	}
	
	private void OnPurchaseSucceded(Purchase purchase)
	{
		Debug.Log("Purchase succeded: " + purchase.Sku + "; Payload: " + purchase.DeveloperPayload);
	
		// Check what was purchased and update game
		switch (purchase.Sku)
		{
		case SKU_REMOVE_ADS:
			MenuLogic.RemoveAd();
			GetComponent<HeroGame> ().HideBanners ();
			RemoveAdsButton.SetActive(false);
			RestoreAdButton.SetActive(false);

			break;
        case SKU_25_COINS:
            PlayerPrefs.SetInt("Coins",PlayerPrefs.GetInt("Coins") + 25);
            PlayerPrefs.Save();
			UpdateCoins();
			GetComponent<MenuLogic>().onPurchased();

			if (CharacterShopComponent != null)
                CharacterShopComponent.UpdateCoins();

            OpenIAB.consumeProduct(purchase);
            break;
		default:
			Debug.LogWarning("Unknown SKU: " + purchase.Sku);
			break;
		}
		_processingPayment = false;
		ProcessingRequest.SetActive (_processingPayment == true);
	}
	public void UpdateCoins()
	{
		int Coins = PlayerPrefs.GetInt("Coins");
		Wallet.text = Coins.ToString();
	}
	private void OnPurchaseFailed(int errorCode, string error)
	{
		Debug.Log("Purchase failed: " + error);
		_processingPayment = false;
		ProcessingRequest.SetActive (_processingPayment == true);
	}
	
	private void OnTransactionRestored(string sku)
	{
		Debug.Log("Transaction restored: " + sku);

		// Check what was purchased and update game
		switch (sku)
		{
		case SKU_REMOVE_ADS:
			MenuLogic.RemoveAd();
			RemoveAdsButton.SetActive(false);
			RestoreAdButton.SetActive(false);
			GetComponent<HeroGame> ().HideBanners ();

			// It should not be here. but onRestoreSucceeded is never called.
			_processingPayment = false;
			ProcessingRequest.SetActive (_processingPayment == true);
			break;
		default:
			Debug.LogWarning("Unknown SKU: " + sku);
			break;
		}
	}
	
	private void OnRestoreSucceeded()
	{
		Debug.Log("Transactions restored successfully");
		_processingPayment = false;
		ProcessingRequest.SetActive (_processingPayment == true);
	}
	
	private void OnRestoreFailed(string error)
	{
		Debug.Log("Transaction restore failed: " + error);
		_processingPayment = false;
		ProcessingRequest.SetActive (_processingPayment == true);
	}
	
	#endregion // Billing

	public void OnRestoreTransactions()
	{
		MenuLogic.PlaySFX ();
		_processingPayment = true;
		ProcessingRequest.SetActive (_processingPayment == true);
		OpenIAB.restoreTransactions();
	}

	public void OnRemoveAds()
	{
		MenuLogic.PlaySFX ();
		_processingPayment = true;
		ProcessingRequest.SetActive (_processingPayment == true);
		OpenIAB.purchaseProduct(SKU_REMOVE_ADS);
	}

    public void OnBuy25Coins()
    {
        MenuLogic.PlaySFX();
        _processingPayment = true;
        ProcessingRequest.SetActive(_processingPayment == true);
        OpenIAB.purchaseProduct(SKU_25_COINS);
    }
}
