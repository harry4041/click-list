using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using TMPro;

public class AdsManager : MonoBehaviour, IUnityAdsListener
{

#if UNITY_IOS
    string gameId = "4750176";
    string rewarded = "Rewarded_iOS";
    string banner = "Banner_iOS";
    string Interstitial = "Interstitial_iOS";
#else
    string gameId = "4750177";
    string rewarded = "Rewarded_Android";
    string banner = "Banner_Android";
    string Interstitial = "Interstitial_Android";
#endif

    void Start()
    {
        Advertisement.Initialize(gameId);
        Advertisement.AddListener(this);
        InvokeRepeating(nameof(ShowBanner), 5, 5);
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
            Debug.Log("Rewarded ad not ready!");
        }
    }

    public void ShowBanner()
    {
        if (Advertisement.IsReady(banner))
        {
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
            Advertisement.Banner.Show(banner);
        }
        else
        {
            StartCoroutine(RepeatShowBanner());
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
}
