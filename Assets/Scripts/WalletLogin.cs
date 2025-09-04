using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using PhantasmaPhoenix.Cryptography;
public class WalletLogin : MonoBehaviour
{
    #region Events
    public event Action<string, bool> OnLoginEvent;
    #endregion

    /// <summary>
    /// Method used to connect to the wallet.Copied From PhantasmaLinkClientSB Login.cs
    /// </summary>

    public TMP_Text LoginButton; // Assign this in the Inspector


    void Start()

    {
        if (PhantasmaLinkClientSB.Instance == null)
        {
            Debug.LogError("PhantasmaLinkClientSB.Instance is null!");

            return;
        }


    }

    public void Logout()
    {
        if (PhantasmaLinkClientSB.Instance.IsLogged)
        {
            PhantasmaLinkClientSB.Instance.Logout();
            //WalletInfo.Instance.SoulBalance.text = "0.00";
            //WalletInfo.Instance.KcalBalance.text = "0.00";
            LoginButton.text = "Login";
            OnLoginEvent?.Invoke("Logged Out.", false);
        }
    }
    public void OnLogin()
    {
        Debug.Log(PhantasmaLinkClientSB.Instance.Ready);
        if (PhantasmaLinkClientSB.Instance.Ready)
        {
            if (!PhantasmaLinkClientSB.Instance.IsLogged)
                PhantasmaLinkClientSB.Instance.Login((result, msg) =>
                {
                    if (result)
                    {
                        // Call event to Handle Login
                        OnLoginEvent?.Invoke("Logged In.", false);
                        Debug.LogWarning("Phantasma Link authorization logged.");
                        //WalletInfo.Instance.SoulBalance.text = System.Math.Round(PhantasmaLinkClientSB.Instance.GetBalance("SOUL"), 3).ToString();
                        //WalletInfo.Instance.KcalBalance.text = System.Math.Round(PhantasmaLinkClientSB.Instance.GetBalance("KCAL"), 3).ToString();
                        LoginButton.text = "Logged In";

                    }
                    else
                    {
                        if (LoginButton.text == "Logged In")
                            LoginButton.text = "Login";
                        OnLoginEvent?.Invoke("Phantasma Link authorization failed.", true);
                        Debug.LogWarning("Phantasma Link authorization failed.");
                    }
                });
            else
                OnLoginEvent?.Invoke("Logged In.", false);

        }
        else
        {
            Debug.LogWarning("Phantasma Link connection is not ready.");
            OnLoginEvent?.Invoke("Phantasma Link connection is not ready.", true);

        }
    }

}
