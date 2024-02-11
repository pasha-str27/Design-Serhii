using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdManager : MonoBehaviour
{
    [Header("Admob Ad Units :")]
    string idBanner = "ca-app-pub-2860978374448480/7561131872";
    string idInterstitial = "ca-app-pub-2860978374448480/1715028388";
    string idReward = "ca-app-pub-2860978374448480/9401946717";

    //test id
    //string idBanner = "ca-app-pub-3940256099942544/6300978111";
    //string idInterstitial = "ca-app-pub-3940256099942544/1033173712";
    //string idReward = "ca-app-pub-3940256099942544/5224354917";


    AndroidJavaObject currentActivity;
    AndroidJavaClass UnityPlayer;
    AndroidJavaObject context;
    AndroidJavaObject toast;
    
    [Header("Toggle Admob Ads :")]
   private bool bannerAdEnabled = false;
   private bool interstitialAdEnabled = true;
   private bool rewardedAdEnabled = true;

    [HideInInspector] public BannerView AdBanner;
    [HideInInspector] public InterstitialAd AdInterstitial;
    [HideInInspector] public RewardedAd AdReward;


    public static AdManager Instance;

    bool isRewardedLoaded;
    bool isInterstitialLoaded;

    protected  void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);

#if UNITY_ANDROID && !UNITY_EDITOR

        UnityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        currentActivity = UnityPlayer
            .GetStatic<AndroidJavaObject>("currentActivity");


        context = currentActivity
            .Call<AndroidJavaObject>("getApplicationContext");
#endif
            
            Instance = this;
            this.InitAd();
            
        }
        else
        { 
            Destroy(this.gameObject);
        }
      
     
    }
    
    public void ShowToast(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#elif UNITY_ANDROID
            currentActivity.Call
                (
                    "runOnUiThread",
                    new AndroidJavaRunnable(() =>
                    {
                        AndroidJavaClass Toast
                        = new AndroidJavaClass("android.widget.Toast");
            
                        AndroidJavaObject javaString
                        = new AndroidJavaObject("java.lang.String", message);
            
                        toast = Toast.CallStatic<AndroidJavaObject>
                        (
                            "makeText",
                            context,
                            javaString,
                            Toast.GetStatic<int>("LENGTH_SHORT")
                        );
            
                        toast.Call("show");
                    })
                 );
#endif
    }
 
    public void InitAd()
    {
        //RequestConfiguration requestConfiguration =
        //    new RequestConfiguration.Builder()
        //        .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
        //        .build();


        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        MobileAds.Initialize(initstatus => {
            //MobileAdsEventExecutor.ExecuteInUpdate(() => {
            // ShowBanner();
            Debug.LogError("advertisement initilizated");
                RequestRewardAd();
                RequestInterstitialAd();
            //});
        });
    }

    private void OnDestroy()
    {
        DestroyBannerAd();
        DestroyInterstitialAd();
    }

    public void Destroy() => Destroy(gameObject);

    public bool IsRewardAdLoaded()
    {
#if UNITY_EDITOR
        return true;
#endif

        return rewardedAdEnabled && isRewardedLoaded && AdReward != null;
    }
    
    AdRequest CreateAdRequest()
    {
        //RequestConfiguration requestConfiguration =
        //    new RequestConfiguration.Builder()
        //        .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
        //        .build();

        return new AdRequest.Builder()//.AddExtra("npa", PlayerPrefs.GetInt("npa", 1).ToString())
           //.TagForChildDirectedTreatment(false)
           //.AddExtra("npa", PlayerPrefs.GetInt("npa", 1).ToString())
           .Build();
    }

    #region Banner Ad ------------------------------------------------------------------------------
    public void ShowBanner()
    {
        if (!bannerAdEnabled) return;

        DestroyBannerAd();

        AdBanner = new BannerView(idBanner, AdSize.Banner, AdPosition.Bottom);

        AdBanner.LoadAd(CreateAdRequest());
    }
    
    public void AdsButtonPressed()
    {
        PlayerPrefs.SetInt("npa", -1);

        //load gdpr scene
        LoadLevel(1);
    }
    
    public static void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogWarning("LEVELLOADER LoadLevel Error: invalid scene specified");
        }
    }

    public void DestroyBannerAd()
    {
        if (AdBanner != null)
            AdBanner.Destroy();
    }
    #endregion

    #region Interstitial Ad ------------------------------------------------------------------------
    public void RequestInterstitialAd()
    {
        if (AdInterstitial != null)
        {
            AdInterstitial.Destroy();
            AdInterstitial = null;
        }

        var adRequest = CreateAdRequest();// new AdRequest.Builder()
        //.AddKeyword("unity-admob-sample")
        //.Build();

        isInterstitialLoaded = false;

        InterstitialAd.Load(idInterstitial, adRequest,
          (InterstitialAd ad, LoadAdError error) =>
          {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
              {
                  Debug.LogError("interstitial ad failed to load an ad " +
                                 "with error : " + error);
                  RequestInterstitialAd();
                  return;
              }

              Debug.Log("Interstitial ad loaded with response : "
                        + ad.GetResponseInfo());

              isInterstitialLoaded = true;

              AdInterstitial = ad;
              AdInterstitial.OnAdFullScreenContentClosed += HandleInterstitialAdClosed;

          });


        //AdInterstitial = new InterstitialAd(idInterstitial);

        //AdInterstitial.OnAdClosed += HandleInterstitialAdClosed;

        //AdInterstitial.LoadAd(CreateAdRequest());
    }

    public void ShowInterstitialAd()
    {
        if (!interstitialAdEnabled) return;

        if (isInterstitialLoaded && AdInterstitial != null)
        {
            AdInterstitial.Show();
        }
    }
    
    public bool IsInterstitialAdLoad()
    {
#if UNITY_EDITOR
        return true;
#endif

        return interstitialAdEnabled && isInterstitialLoaded && AdInterstitial != null;
    }

    public void DestroyInterstitialAd()
    {
        if (AdInterstitial != null)
            AdInterstitial.Destroy();
    }
    #endregion

    #region Rewarded Ad ----------------------------------------------------------------------------
    public void RequestRewardAd()
    {
        //AdReward = new RewardedAd(idReward);

        //AdReward.OnAdClosed += HandleOnRewardedAdClosed;
        //AdReward.OnUserEarnedReward += HandleOnRewardedAdWatched;

        //AdReward.LoadAd(CreateAdRequest());

        if (AdReward != null)
        {
            AdReward.Destroy();
            AdReward = null;
        }

        var adRequest = CreateAdRequest();// new AdRequest.Builder().Build();

        isRewardedLoaded = false;

        // send the request to load the ad.
        RewardedAd.Load(idReward, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);

                    Invoke(nameof(RequestRewardAd), error.GetCode() == 1 ? 5 : 3);
                    //RequestRewardAd();
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                AdReward = ad;
                isRewardedLoaded = true;
                AdReward.OnAdFullScreenContentClosed += RequestRewardAd;
            });
    }   
    
   
    public void ShowRewardAd()
    {
        if (!rewardedAdEnabled) return;

        if (isRewardedLoaded)
            AdReward.Show(HandleOnRewardedAdWatched);
        else
        {
            RequestRewardAd();
            ShowToast("Reward based video ad is not ready yet");
            RewardFailAction?.Invoke();
        }
    } 
    

    public bool IsCanShowRewardAD()
    {
        return isRewardedLoaded;
    }   
    
    
    #endregion

    #region Event Handler
    
    public Action InteralADAction = null;

    private void HandleInterstitialAdClosed()
    {
        Invoke(nameof(InterstitialAdClosedDelayCalled), 0.3f);
    }

    void InterstitialAdClosedDelayCalled()
    {
        if (InteralADAction != null)
        {
            InteralADAction.Invoke();
        }
        InteralADAction?.Invoke();
        DestroyInterstitialAd();
        RequestInterstitialAd();
    }

    private void HandleInterstitialAdClosed(object sender, EventArgs e)
    {
        if (InteralADAction != null)
        {
            InteralADAction.Invoke();
        }
        InteralADAction?.Invoke();
        DestroyInterstitialAd();
        RequestInterstitialAd();
    }

    public Action RewardAction = null;
    public Action RewardFailAction = null;
    private void HandleOnRewardedAdClosed(object sender, EventArgs e)
    {
        RequestRewardAd();
    }

    private void HandleOnRewardedAdWatched(Reward reward)
    {
        if (RewardAction != null)
        {
            RewardAction.Invoke();
        }
        RequestRewardAd();
        RewardAction = null;
    }

    private void HandleOnRewardedAdWatched(object sender, Reward e)
    {
        if (RewardAction != null)
        {
            RewardAction.Invoke();
        }
        RequestRewardAd();
        RewardAction = null;
    }
    #endregion
}
