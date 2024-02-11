using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using Popup;
using Google.Play.Review;

public class PopupRating : PopupBase
{
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    private Coroutine _coroutine;

    public override void Show()
	{
        canClose = false;
        PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
        _coroutine = StartCoroutine(InitReview());
    }

    public override void Close(bool forceDestroying = true)
	{		
		TerminateInternal(forceDestroying);
	}

	public void Rate14Stars()
    {
        SendEmail();

        CloseInternal();
    }

	public void Rate5Stars()
    {
		PlayerData.current.appRated = true;
        RateAndReview();
        //Application.OpenURL("market://details?id=" + Application.identifier);
        //Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);

		CloseInternal();
	}

    public void SendEmail()
    {
        string email = "adareonstudios@gmail.com";
        string subject = MyEscapeURL("Отзыв на Design");
        string body = MyEscapeURL("My Body\\r\\nFull of non-escaped chars");
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&amp;body=" + body);
    }
    string MyEscapeURL(string URL)
    {
        return WWW.EscapeURL(URL).Replace("+", "%20");
    }

    public void RateAndReview()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#elif UNITY_ANDROID
        StartCoroutine(LaunchReview());
#endif
    }

    private IEnumerator InitReview(bool force = false)
    {
        if (_reviewManager == null) _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            if (force) DirectlyOpen();
            yield break;
        }

        _playReviewInfo = requestFlowOperation.GetResult();
    }

    public IEnumerator LaunchReview()
    {
        if (_playReviewInfo == null)
        {
            if (_coroutine != null) StopCoroutine(_coroutine);
            yield return StartCoroutine(InitReview(true));
        }

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            DirectlyOpen();
            yield break;
        }
    }

    private void DirectlyOpen() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }
}
