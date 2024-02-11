using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using DG.Tweening;
using Popup;
using System.Text;

public class PopupRemoveAds : PopupBase
{
    private static float priceUsd = 1.99f;
    public Text priceText;

    public override void Show()
    {
        priceText.text = "$" + priceUsd;
        canClose = false;
        canvasGroup.alpha = 1f;
        PopupAnimationUtility.AnimateScale(transform, Ease.OutBack, 0.25f, 1f, 0.25f, 0f).OnComplete(() => canClose = true);
    }

    public override void Close(bool forceDestroying = true)
    {
        TerminateInternal(forceDestroying);
    }

    public void PressBuyButton()
    {
        if (IAPManager.Instance.IsInitialized())
        {
            IAPManager.Instance.BuyConsumable(IAPManager.Instance.no_ads, PurchaseGemSucessful);
        }
    }

    public void PurchaseGemSucessful(string id,bool issuccess, PurchaseFailureReason reason)
    {
        if (id == IAPManager.Instance.no_ads)
        {
            if (issuccess == true)
            {
                AudioManager.Instance.PlaySFX(AudioClipId.Purchased);
                PlayerData.current.noAds = true;

                AcceptEvent?.Invoke(null);

                CloseInternal();

                PlayerData.current.tempData.spentIAP += priceUsd;
                //AppEventTracker.LogEventIap(productId, priceUsd.ToString());    
            }
        }
    }
}


