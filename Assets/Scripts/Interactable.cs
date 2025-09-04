using System;
using System.Collections;
using NBitcoin.Protocol;
using NUnit.Framework.Interfaces;
using Org.BouncyCastle.Tls;
using PhantasmaPhoenix.Core;
using PhantasmaPhoenix.Cryptography;
using PhantasmaPhoenix.Protocol;
using PhantasmaPhoenix.Unity.Core;
using PhantasmaPhoenix.VM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Interactable : MonoBehaviour
{
    readonly string net = "https://pharpc1.phantasma.info/rpc";
    [SerializeField] string DonationAddy = "P2KKQBFNmxyD3vWMFFiV15m8w2bLgDBi4JQKm4b7wT8gxi7";
    public GameObject donationMessage;

    public void Interact()
    {
        switch (gameObject.tag)
        {
            case "Donation":
                if (PlayerPrefs.GetInt(PlayerPrefKeys.usePoltergeist, 0) == 1)
                {
                    if (PhantasmaLinkClientSB.Instance != null && PhantasmaLinkClientSB.Instance.IsLogged)
                    {
                        SendTrans();
                        break;
                    }
                    else
                        Debug.Log("Not logged in, cannot send transaction");
                    break;
                }
                else
                {
                    if (PlayerPrefs.GetString(PlayerPrefKeys.wif, "") == "")
                    {
                        Debug.Log("No InGameWallet found, cannot send transaction");
                        break;
                    }
                    InGameWalletTrans();
                    Debug.Log("Using Game wallet to send transaction");
                    break;
                }
            default:
                Debug.Log("Interacted with object: " + gameObject.name);
                break;
                // Handle default interaction
        }
    }

    public void InGameWalletTrans()
    {
        var api = new PhantasmaAPI(net);
        string Payload = "ShamanplaygroundDonation";
        var keys = PhantasmaKeys.FromWIF(PlayerPrefs.GetString(PlayerPrefKeys.wif, ""));
        var senderAddress = keys.Address;
        var nexus = "mainnet"; //will need to make this changeable later
        var toAddress = Address.Parse(DonationAddy);
        var symbol = "KCAL";
        var amount = UnitConversion.ToBigInteger(10, 10);
        //var payload = string.IsNullOrWhiteSpace(Payload) ? null : System.Text.Encoding.UTF8.GetBytes(Payload);
        var feePrice = 100000; // TODO: Adapt to new fee model.
        var feeLimit = 21000;
        var sb = new ScriptBuilder();
        sb.AllowGas(senderAddress, Address.Null, feePrice, feeLimit);

        // Add instruction to transfer tokens from sender to destination, converting human-readable amount to chain format
        sb.TransferTokens(symbol, senderAddress, toAddress, amount);

        // Spend gas necessary for transaction execution
        sb.SpendGas(senderAddress);

        // Finalize and get raw bytecode for the transaction script
        var script = sb.EndScript();
        StartCoroutine(api.SignAndSendTransactionWithPayload(keys, nexus, script, "main", Payload,
                // Callback on success
                (hashText, encodedTx, txHash) =>
                {
                    if (!string.IsNullOrEmpty(hashText))
                    {

                        // Start polling to track transaction execution status on-chain
                        // CheckTxStateLoop() implementation is available in "Check Transaction State" example
                        var Result = StartCoroutine(CheckTxStateLoop(api, hashText, (txState, txResult, debugComment) =>
                        {
                            if (string.IsNullOrWhiteSpace(debugComment))
                            {

                                StartCoroutine(displayMessage("success", donationMessage));
                                WalletInfo.Instance.setBalance();

                            }
                            else
                            {
                                //error with the transaction
                                StartCoroutine(displayMessage("fail", donationMessage));
                                WalletInfo.Instance.setBalance();
                            }

                        }
                        ));

                        return;
                    }

                },
                // Callback for RPC errors (invalid token, network error, etc.)
                (errorCode, errorMessage) =>
                {
                    StartCoroutine(displayMessage("fail", donationMessage));
                    Debug.LogWarning($"[Error][{errorCode}] Failed to send transaction: {errorMessage}");
                }));


    }
    public void SendTrans()
    {

        string Payload = "ShamanPlaygroundDonation";
        // Handle NPC interaction
        ScriptBuilder sb = new ScriptBuilder();
        //Donation Address
        var toAddress = Address.Parse(DonationAddy);
        var userAddress = Address.Parse(PhantasmaLinkClientSB.Instance.Address);
        var symbol = "KCAL";
        //Donation Amount fixed to 10 KCAL for now. Only for testing purposes
        var amount = UnitConversion.ToBigInteger(10, 10);
        var payload = string.IsNullOrWhiteSpace(Payload) ? null : System.Text.Encoding.UTF8.GetBytes(Payload);
        Debug.Log("Interacting with NPC: " + gameObject.name);
        var script = sb.AllowGas(userAddress, Address.Null, PhantasmaLinkClientSB.Instance.GasPrice, PhantasmaLinkClientSB.Instance.GasLimit).
            CallInterop("Runtime.TransferTokens", userAddress, toAddress, symbol, amount).
            SpendGas(userAddress).
            EndScript();
        Debug.Log("Sending transaction to " + toAddress + " with amount: " + amount + " " + symbol);
        StartCoroutine(SendTransactionCoroutine(
            PhantasmaLinkClientSB.Instance.Nexus,
            script,
            payload));

    }


    // Coroutine that monitors the state of a transaction; invokes callback with status and result when complete.
    // Callback is optional.
    // If we could not determine state of tx, null will be passed as first callback argument
    public static IEnumerator CheckTxStateLoop(PhantasmaAPI api, string txHash, Action<ExecutionState?, string, string> callback)
    {
        // Flag to stop polling loop once the transaction is finalized
        bool done = false;

        // Counter for how many times we've polled for transaction status
        uint txStatusQueryAttempts = 0;

        // Counter for how many times we've attempted to get failure debug details, if needed
        uint failureDetailsQueryAttempts = 0;

        while (!done)
        {
            // Make RPC call to fetch transaction info from the chain
            yield return new WaitForSeconds(5f);
            yield return api.GetTransaction(txHash,
                (txResult) =>
                {
                    // Log the current execution state: Running, Halt (success), or other (failure)
                    Debug.Log($"Transaction state is: {txResult.State}");

                    switch (txResult.State)
                    {
                        case PhantasmaPhoenix.Protocol.ExecutionState.Running:
                            // Transaction is still being processed by the chain
                            Debug.Log("Transaction is still processing...");
                            break;

                        case PhantasmaPhoenix.Protocol.ExecutionState.Halt:
                            // Transaction completed successfully (execution halted without errors)

                            // Check if any result string is available (may be empty if not applicable)
                            if (string.IsNullOrEmpty(txResult.Result))
                            {

                                Debug.Log($"Transaction executed successfully, no result available.");
                            }
                            else
                            {
                                Debug.Log($"Transaction executed successfully with result '{txResult.Result}'.");
                            }
                            done = true;


                            // Notify success with result value and no error info
                            callback?.Invoke(txResult.State, txResult.Result, null);
                            break;

                        default:
                            // Transaction failed. We check if we have additional details about failure available.
                            // If failure details are not yet available, and we haven't tried too many times - wait and retry
                            if (txResult.DebugComment == null && failureDetailsQueryAttempts < 6)
                            {
                                // Inform user that we're retrying in case debug info hasn't yet been indexed on the node
                                Debug.Log($"Waiting for failure details... Attempt {failureDetailsQueryAttempts + 1}/6");
                                failureDetailsQueryAttempts++;
                                break;
                            }

                            // Final failure state reached, log failure details and return via callback
                            Debug.LogWarning($"Transaction failed with state: {txResult.State}. Result: {txResult.Result}. Details: {txResult.DebugComment}");
                            done = true;

                            // Notify failure with state, raw result, and debug comment if available

                            callback?.Invoke(txResult.State, txResult.Result, txResult.DebugComment);
                            break;
                    }
                },
                // Error handler for network or RPC-level errors
                (errorCode, errorMessage) =>
                {
                    // Log API error such as invalid hash, RPC timeout, etc
                    // "Transaction not found" also gets here
                    Debug.LogWarning($"[Error][{errorCode}] {errorMessage}");
                });
            // If still running (or DebugComment is unavailable), wait 1 second before checking again
            if (!done)
            {
                // Stop retrying if status check exceeded max allowed attempts
                if (txStatusQueryAttempts == 30)
                {
                    Debug.LogWarning($"Query attempts exhausted after {txStatusQueryAttempts} attempts, tx state could not be confirmed");
                    // Notify that the transaction status could not be confirmed at all (timeout case)
                    callback?.Invoke(null, null, null);
                    break;
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                    txStatusQueryAttempts++;
                }
            }
        }
    }

    public IEnumerator displayMessage(string state, GameObject donationMessage)
    {
        donationMessage.SetActive(true);
        switch (state)
        {
            case "fail":
                donationMessage.GetComponent<TextMeshPro>().text = "Donation Failed :(";
                yield return new WaitForSeconds(1.5f);
                break;
            case "success":
                donationMessage.GetComponent<TextMeshPro>().text = "Thank you for your Donation :)";
                yield return new WaitForSeconds(1.5f);
                break;

        }
        donationMessage.GetComponent<TextMeshPro>().text = "";
        donationMessage.SetActive(false);

    }





    public IEnumerator SendTransactionCoroutine(string chain, byte[] script, byte[] payload, Action<Hash, string> callback = null)
    {
        bool done = false;
        Hash txHash = Hash.Null;
        string errorMsg = null;

        PhantasmaLinkClientSB.Instance.SendTransaction(chain, script, payload, (result, msg) =>
        {
            txHash = result;
            errorMsg = msg;
            done = true;
        });

        // Wait until the callback sets 'done' to true
        yield return new WaitUntil(() => done);

        if (txHash.IsNull)
        {
            Debug.LogWarning("Transaction failed: " + errorMsg);
            StartCoroutine(displayMessage("fail", donationMessage));
            yield return new WaitForSeconds(1.5f);
            PhantasmaLinkClientSB.Instance.ReloadAccount();
            WalletInfo.Instance.SoulBalance.text = System.Math.Round(PhantasmaLinkClientSB.Instance.GetBalance("SOUL"), 3).ToString();
            WalletInfo.Instance.KcalBalance.text = System.Math.Round(PhantasmaLinkClientSB.Instance.GetBalance("KCAL"), 3).ToString();

        }
        else
        {
            //will reload the account to update balances. Still need to implement a way to update the balance UI
            yield return new WaitForSeconds(1.5f);
            PhantasmaLinkClientSB.Instance.ReloadAccount();
            WalletInfo.Instance.SoulBalance.text = System.Math.Round(PhantasmaLinkClientSB.Instance.GetBalance("SOUL"), 3).ToString();
            WalletInfo.Instance.KcalBalance.text = System.Math.Round(PhantasmaLinkClientSB.Instance.GetBalance("KCAL"), 3).ToString();
            StartCoroutine(displayMessage("success", donationMessage));
            Debug.Log("Transaction succeeded! Hash: " + txHash);
        }
    }

}