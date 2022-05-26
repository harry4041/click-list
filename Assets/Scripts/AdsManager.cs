using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using TMPro;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{

    private BannerView bannerView;
    private RewardedAd rewardedAd;

#if UNITY_IOS
    string gameId = "4750176";
    string rewarded = "Rewarded_iOS";
    string banner = "Banner_iOS";
    string Interstitial = "Interstitial_iOS";
    
    //Google (Currently placeholder)
    string adUnitId = "ca-app-pub-3940256099942544/2934735716";
    string rewardedGoogleAd = "ca-app-pub-3940256099942544/1712485313";

#else
    string gameId = "4750177";
    string rewarded = "Rewarded_Android";
    string banner = "Banner_Android";
    string Interstitial = "Interstitial_Android";

    //Google (Currently placeholder)
    string adUnitId = "ca-app-pub-3940256099942544/6300978111";
    string rewardedGoogleAd = "ca-app-pub-3940256099942544/5224354917";
#endif

    void Start()
    {
        Advertisement.Initialize(gameId);
        Advertisement.AddListener(this);
        InvokeRepeating(nameof(ShowBanner), 1, 5);

        //Google ads
        MobileAds.Initialize(initStatus => { });
    }

    public void PlayAd()
    {
        if (Advertisement.IsReady(Interstitial))
        {
            Advertisement.Show(Interstitial);
        }
    }

    public void PlayRewardedAd()
    {


        if (Advertisement.IsReady(rewarded))
        {
            Advertisement.Show(rewarded);
        }
        else
        {
            //If unity fails, show google
            this.rewardedAd = new RewardedAd(rewardedGoogleAd);

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded ad with the request.
            this.rewardedAd.LoadAd(request);

            if (this.rewardedAd.IsLoaded())
            {
                this.rewardedAd.Show();
            }
            else
            {
                PlayRewardedAd();
            }

            Debug.Log("Unity ewarded ad not ready!");
        }
    }

    public void ShowBanner()
    {
        if (Advertisement.IsReady(banner))
        {
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(banner);
            //Destroy google ad to stop overlap
            bannerView.Destroy();
        }
        else
        {
            //If unity fails to load, load google ads
            RequestBanner();
        }
    }

    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    IEnumerator RepeatShowBanner()
    {
        yield return new WaitForSeconds(1);
        ShowBanner();
    }

    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("Ads ready");
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.Log("ERROR:" + message);
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("Ad started");

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == "Rewarded_Android" && showResult == ShowResult.Finished)
        {
            Debug.Log("Player should be rewarded");
        }
    }

    //
    // GOOGLE ADS
    //

    private void RequestBanner()
    {
        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        //If google ad is loaded, dont load unity ad on top
        this.bannerView.OnAdLoaded += this.HandleOnAdLoaded;

        //If fails to load, try to run unity ads, then google ads, then try again
        this.bannerView.OnAdFailedToLoad += this.HandleOnAdFailedToLoad;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);

        

        
    }


    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        MonoBehaviour.print("HandleAdLoaded event received");
        HideBanner();
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        MonoBehaviour.print("HandleFailedToReceiveAd event received with message: "
                            + args);

        StartCoroutine(RepeatShowBanner());
    }
}
