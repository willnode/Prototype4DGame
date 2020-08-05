using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class GUIController : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    public void SetWZLock(bool locked)
    {
        statusText.text = locked ? "WZ Locked" : "WZ Unlocked (Scroll to warp)";
    }
}
