using UnityEngine;
using System.Collections;
using Engine4;
using Engine4.Physics;

public class Swapper3DSensor : MonoBehaviour4
{
    private PlayerController player;
    public float targetSwapValueForZW = 0;
    private float lastSwapValueForZW = 0;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }
    void OnEnable()
    {
        GetComponent<Collider4>().callback += OnCollisionCallback;
        Debug.Log(Physics4.main.collisionCallbacks);
    }
    void OnDisable()
    {
        if (!enabled)
        {
            GetComponent<Collider4>().callback -= OnCollisionCallback;
        }
    }

    // Update is called once per frame
    void OnCollisionCallback(CollisionHit4 hit)
    {
        if (hit.state == CollisionState.Enter)
        {
            player.lockZW = true;
            lastSwapValueForZW = player.angles.v;
            StartCoroutine(TweenPlayerCamera(targetSwapValueForZW));
        }
        if (hit.state == CollisionState.Exit)
        {
            player.lockZW = false;
            StartCoroutine(TweenPlayerCamera(lastSwapValueForZW));
        }
    }

    IEnumerator TweenPlayerCamera(float target)
    {
        float refSpeed = 0;
        player.rigidbody4.enabled = false;
        while (Mathf.Abs(target - player.angles.v) > 0.01f)
        {
            player.angles.v = Mathf.SmoothDampAngle(player.angles.v, target, ref refSpeed, 0.5f);
            yield return null;
        }
        player.rigidbody4.enabled = true;
        player.angles.v = target;
    }
}
