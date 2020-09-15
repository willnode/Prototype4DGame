using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Engine4;

public class GUIController : MonoBehaviour4
{
    public TextMeshProUGUI statusText;
    public RectTransform beaconRect;
    public RectTransform beaconDirRect;
    public Rect mapSize;

    public void SetWZLock(bool locked)
    {
        statusText.text = locked ? "WZ Locked" : "WZ Unlocked (Scroll to warp)";
    }

    void Update()
    {
        Euler4 angles = GetComponent<PlayerController>().angles;
        float rx = Mathf.InverseLerp(mapSize.xMin, mapSize.xMax, transform4.position.z);
        float ry = Mathf.InverseLerp(mapSize.yMin, mapSize.yMax, transform4.position.w);
        Vector2 rel = new Vector2(rx, ry);
        beaconRect.anchorMin = rel;
        beaconRect.anchorMax = rel;
        beaconRect.rotation = Quaternion.Euler(0, 0, angles.v);
        beaconDirRect.anchoredPosition = Vector2.right * Mathf.Cos(angles.y * Mathf.Deg2Rad) * 50;
    }
}
