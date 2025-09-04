using UnityEngine;
using TMPro;
using PhantasmaPhoenix.Core;
using PhantasmaPhoenix.Unity.Core;

public class WalletInfo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static WalletInfo Instance { get; private set; }
    public TMP_Text SoulBalance;
    public TMP_Text KcalBalance;
    readonly string url = "https://pharpc1.phantasma.info/rpc";


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);

    }
    void Start()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefKeys.wif, "")))
        {
            SoulBalance.text = "0.00";
            KcalBalance.text = "0.00";
        }
        else
        //might need to start an async thread to get the current balance of the in game wallet
        {
            setBalance();
        }

    }
    //Should Change to be more dynamic but for now make it only for SOUL and KCAL
    public void setBalance()
    {
        GetAddressTokenBalance("SOUL", SoulBalance);
        GetAddressTokenBalance("KCAL", KcalBalance);

    }




    void GetAddressTokenBalance(string token, TMP_Text symb)
    {
        // Initialize PhantasmaAPI instance
        var api = new PhantasmaAPI(url);

        // Address to check balance for
        var address = PlayerPrefs.GetString(PlayerPrefKeys.publicKey, "");

        // Token symbol to query (e.g. SOUL, KCAL, NFT symbol)
        var symbol = token;

        StartCoroutine(api.GetTokenBalance(address, symbol, "main",
            // Callback on success
            (tokenBalanceResult) =>
            {
                // Check whether the token is fungible (e.g. SOUL, KCAL) or non-fungible (NFT)

                // UnitConversion.ToDecimal() converts raw token amount into human-readable decimal format
                Debug.Log($"[Balance] Fungible {symbol} amount for {address}: {UnitConversion.ToDecimal(tokenBalanceResult.Amount, (int)tokenBalanceResult.Decimals)}");
                var balance = UnitConversion.ToDecimal(tokenBalanceResult.Amount, (int)tokenBalanceResult.Decimals);
                symb.text = System.Math.Round(balance, 3).ToString();

            },
            // Callback for RPC errors (invalid token, network error, etc.)
            (errorCode, errorMessage) =>
            {
                symb.text = "0";
                Debug.LogWarning($"[Error][{errorCode}] {errorMessage}");
            }
        ));

    }
}
