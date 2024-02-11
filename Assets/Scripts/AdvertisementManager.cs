// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class AdvertisementManager : MonoSingleton<AdvertisementManager>
// {
//     public Action RewardedVideoSucessfulExtraEvent;
//
//     private Action InjectSucessfulRewardEvent;
//
//     private Action InterstitialAdCloseEvent;
//
//     private Action InterstitialAdSucceededEvent;
//
//     private Action InterstitialAdFailedEvent;
//
//     private bool isShowingAd;
//
//     bool doneAdsInterval = true;
//
//     private int waitFailTime = 0;
//     private int waitFailTimeMax = 5;
//     private int addWaitFailTime = 5;
//
//     string idAds = "15d585405";
//
//     void SetAdsInterval()
//     {       
//         doneAdsInterval = false;
//
//         StartCoroutine(WaitEndAdsInterval());
//     }
//
//     IEnumerator WaitEndAdsInterval()
//     {
//         yield return new WaitForSecondsRealtime(waitFailTimeMax);
//         doneAdsInterval = true;
//     }
//
//     public bool IsRewardedVideoAvailable
//     {
//         get 
//         {
// #if UNITY_EDITOR
//             return true;
// #else
//             var isDone = IronSource.Agent.isRewardedVideoAvailable();
//             if(isDone == false)
//             {
//                 LoadRewardAd();
//             }
//             return isDone;
// #endif
//         }
//     }
//
//     public override void Awake()
//     {        
//         base.Awake();
//     }
//
//     private void Start()
//     {        
//         IronSource.Agent.init(idAds, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);
//         IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewarded;
//         IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosed;
//
//         IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReady;
//         IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailed;
//         IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosed;
//         IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceeded;
//         IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailed;
//
//         Invoke("LoadInterstitialAd", 2);
//         Invoke("LoadRewardAd", 2);
//         
//     }
//
//     private void LoadRewardAd()
//     {
//         IronSource.Agent.loadRewardedVideo();
//     }
//     
//     private void LoadInterstitialAd()
//     {
//         IronSource.Agent.loadInterstitial();
//     }
//
//     private void InterstitialAdReady()
//     {
//         waitFailTime = 0;
//     }
//
//     private void InterstitialAdClosed()
//     {
//         InterstitialAdCloseEvent?.Invoke();
//         InterstitialAdCloseEvent = null;
//
//         waitFailTime = 0;
//         SetAdsInterval();
//         LoadInterstitialAd();
//     }
//
//     void InterstitialAdLoadFailed(IronSourceError error)
//     {
//         if (waitFailTime < waitFailTimeMax)
//         {
//             waitFailTime += addWaitFailTime;
//         }
//
//         StartCoroutine(WaitForLoadAds(waitFailTime));
//     }
//
//     IEnumerator WaitForLoadAds(float time)
//     {
//         yield return new WaitForSecondsRealtime(time);
//         LoadInterstitialAd();
//     }
//
//     private void InterstitialAdShowSucceeded()
//     {
//         LoadInterstitialAd();
//
//         InterstitialAdSucceededEvent?.Invoke();
//         InterstitialAdSucceededEvent = null;
//     }
//
//     private void InterstitialAdShowFailed(IronSourceError error)
//     {
//         waitFailTime = 0;
//         SetAdsInterval();
//         LoadInterstitialAd();
//
//         InterstitialAdFailedEvent?.Invoke();
//         InterstitialAdFailedEvent = null;
//     }
//
//     private void RewardedVideoAdRewarded(IronSourcePlacement obj)
//     {
//         StartCoroutine(RewardedVideoRewardCoroutine());
//     }
//
//     private void RewardedVideoAdClosed()
//     {
//         LoadRewardAd();
//     }
//
//     IEnumerator RewardedVideoRewardCoroutine()
//     {
//         yield return new WaitForSecondsRealtime(0.2f);
//
//         InvokeSucessRewardVideoEvent();
//     }
//
//     private void InvokeSucessRewardVideoEvent()
//     {
//         if (InjectSucessfulRewardEvent != null)
//         {
//             InjectSucessfulRewardEvent();
//             InjectSucessfulRewardEvent = null;
//         }
//
//         TempData tempData = PlayerData.current.tempData;
//         tempData.rewardedVideoCount++;
//
//         if (tempData.rewardedVideoCount == 7)
//         {
//             // Firebase.Analytics.FirebaseAnalytics.LogEvent("Feature_selected_7_ads");
//         }
//
//         RewardedVideoSucessfulExtraEvent?.Invoke();
//     }
//
//     void OnApplicationPause(bool isPaused)
//     {
//         IronSource.Agent.onApplicationPause(isPaused);
//     }
//
//     public void ShowInterstitial(Action OnClose = null, Action SuccessEvent = null, Action FailEvent = null)
//     {
//         if (doneAdsInterval) 
//         {
//             if (IronSource.Agent.isInterstitialReady())
//             {
//
//                 isShowingAd = true;
//                 IronSource.Agent.showInterstitial();
//
//                 InterstitialAdCloseEvent = OnClose;
//                 InterstitialAdSucceededEvent = SuccessEvent;
//                 InterstitialAdFailedEvent = FailEvent;
//             }
//             else
//             {
//                 LoadInterstitialAd();
//
//                 OnClose?.Invoke();
//                 FailEvent?.Invoke();
//             }
//         }
//         else 
//         {
//             OnClose?.Invoke();
//         }       
//     }
//
//     public void ShowRewardedVideo(Action sucessEvent, Action failEvent = null)
//     {
// #if UNITY_EDITOR
//         InjectSucessfulRewardEvent = sucessEvent;
//         
//         InvokeSucessRewardVideoEvent();
// #else
//         if (IsRewardedVideoAvailable)
//         {
//             InjectSucessfulRewardEvent = sucessEvent;
//             isShowingAd = true;
//             IronSource.Agent.showRewardedVideo();        
//         }
//         else
//         {
//             if (failEvent != null)
//             {
//                 failEvent();
//                 failEvent = null;
//             }
//         }
// #endif
//     }
// }
