using UnityEngine;
using UnityEngine.UI;
using PhantasmaPhoenix.Cryptography;
using Unity.VisualScripting;
using Unity.Mathematics;
using TMPro;
using Org.BouncyCastle.Asn1.Mozilla;


public class PlayerPrefKeys
{
    public const string wif = "WIF";
    public const string publicKey = "PublicKey";
    public const string usePoltergeist = "UsePoltergeist";

}

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] string WIF { get; set; }
    [SerializeField] string PublicKey { get; set; }
    [SerializeField] bool UsePoltergeistSelected { get; set; }
    public Toggle UsePoltergeistToggle;
    public Button CreateWalletButton;
    public TextMeshProUGUI WIFText;
    public TextMeshProUGUI PubKeyText;
    //Settings that should be saved:
    //Use Poltergeist or not
    //Create Wallet Button interacable is Use Poltergeist is false or if wif already exist in player prefs

    void Awake()
    {
        LoadSettings();
        UsePoltergeistToggle.onValueChanged.AddListener(UsePoltergeist);
        if (UsePoltergeistSelected)
            UsePoltergeistToggle.isOn = true;

    }
    void Start()
    {

        if (WIF != string.Empty)
        {
            // Generate a new random private key and derive address and public key

            CreateWalletButton.interactable = false;



        }
        else
        {
            CreateWalletButton.interactable = true;

        }
    }

    // Update is called once per frame


    void UsePoltergeist(bool on)
    {
        PlayerPrefs.SetInt(PlayerPrefKeys.usePoltergeist, on ? 1 : 0);
        PlayerPrefs.Save();
    }
    //Just going to save everything to PlayerPrefs for now. Not secure but this is just a playground
    public void OnCreateWallet()
    {

        Debug.Log("Creating new wallet:" + PlayerPrefs.GetString(PlayerPrefKeys.wif, ""));
        //Generate a new key and save it to player prefs

        var key = PhantasmaKeys.Generate();
        PlayerPrefs.SetString(PlayerPrefKeys.wif, key.ToWIF());
        PlayerPrefs.SetString(PlayerPrefKeys.publicKey, key.Address.ToString());
        PlayerPrefs.Save();
        WIF = PlayerPrefs.GetString(PlayerPrefKeys.wif, string.Empty);
        PublicKey = PlayerPrefs.GetString(PlayerPrefKeys.publicKey, string.Empty);
        CreateWalletButton.interactable = false;


    }
    //This is only for Playground purposes only. Not for production use.

    //This is only for Playground purposes only. Not for production use.Especially Public key and private key
    void LoadSettings()
    {
        WIF = PlayerPrefs.GetString(PlayerPrefKeys.wif, string.Empty);
        PublicKey = PlayerPrefs.GetString(PlayerPrefKeys.publicKey, string.Empty);
        UsePoltergeistSelected = PlayerPrefs.GetInt(PlayerPrefKeys.usePoltergeist, 0) == 1;


    }

    // Use a Prompt to show the Wif,// could use a password in the future
    public void ShowWIF()
    {

        WIFText.text = PlayerPrefs.GetString(PlayerPrefKeys.wif, string.Empty);

    }

    public void showPublicKey()
    {
        PubKeyText.text = PlayerPrefs.GetString(PlayerPrefKeys.publicKey, string.Empty);
    }

    void OnDestroy()
    {
        UsePoltergeistToggle.onValueChanged.RemoveAllListeners();
        //CreateWalletButton.onClick.RemoveAllListeners();
        WIFText.text = string.Empty;
    }
}
