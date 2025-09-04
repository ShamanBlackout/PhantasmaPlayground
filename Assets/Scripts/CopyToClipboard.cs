using TMPro;
using UnityEngine;

public class CopyToClipboard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public TextMeshProUGUI clipboardCopy;
    public void copy()
    {

        GUIUtility.systemCopyBuffer = clipboardCopy.text;
    }
}

