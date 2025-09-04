using TMPro;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float interactionRadius = 1f;
    public GameObject PromptUI;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Collider2D[] colliderArray = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
        bool showPrompt = false;


        foreach (Collider2D collider in colliderArray)
        {

            if (collider.TryGetComponent(out Interactable interactable))
            {
                showPrompt = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.Interact();
                }
                break;

            }
        }
        PromptUI.SetActive(showPrompt);

    }


}
