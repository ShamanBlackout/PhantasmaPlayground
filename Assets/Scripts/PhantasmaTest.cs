using UnityEngine;
using PPUC =  PhantasmaPhoenix.Unity.Core;
public class PhantasmaTest : MonoBehaviour
{

    public string host = "https://pharpc1.phantasma.info";
    PPUC.PhantasmaAPI api;
    public string address = "P2KKQBFNmxyD3vWMFFiV15m8w2bLgDBi4JQKm4b7wT8gxi7";
    void Start()
    {
        Debug.Log("Starting Phantasma Test...");
        api = new PPUC.PhantasmaAPI(host);
       StartCoroutine(api.GetAccount(address,(account)=>
        {
            Debug.Log("Account: " + account.Address);
            Debug.Log("Balance: " + account.Balances);
            Debug.Log("Name: " + account.Name);
            
        }));
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
