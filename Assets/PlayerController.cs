using Engine4;
using UnityEngine;
using Vector4 = Engine4.Vector4;

public class PlayerController : MonoBehaviour4
{
    public float moveSpeed = 2;
    public float rotateSpeed = 15;
    internal Euler4 angles;
    internal Vector3 angles3D;
    internal bool m_lockZW = false;

    public bool lockZW
    {
        get => m_lockZW;
        set => GetComponent<GUIController>().SetWZLock(m_lockZW = value);
    }

    // Update is called once per frame
    void Update()
    {
        transform4.position += transform4.rotation * new Vector4(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"), 0) * moveSpeed * Time.deltaTime;
        angles += new Euler4(0, Input.GetAxis("Mouse X"), 0, 0, 0, Input.GetAxis("Mouse ScrollWheel") * (lockZW ? 0 : 2)) * rotateSpeed;
        transform4.rotation = Matrix4.Euler(angles);

        // For Y rotation, use 3D transform because we need the background rotates too
        angles3D.x = Mathf.Clamp(Input.GetAxis("Mouse Y") * -rotateSpeed * 0.5f + angles3D.x, -90, 90);
        Camera.main.transform.eulerAngles = angles3D;
    }
}
