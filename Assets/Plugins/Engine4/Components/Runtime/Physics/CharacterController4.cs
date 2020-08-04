using Engine4;
using Engine4.Physics.Internal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine4.Physics
{
    public class CharacterController4 : MonoBehaviour4
    {

        // Start is called before the first frame update
        void Start()
        {

        }

        Vector4 velocity;

        // Update is called once per frame
        void FixedUpdate()
        {
            var gravity = Common.GRAVITY;
            if (Vector4.LengthSq(gravity) <= 1e-4) return;
            gravity = Vector4.Normalize(gravity);
            var center = transform4.position;
            var pen = 0f;
            var collider = GetComponent<Collider4>();
            if (collider is SphereCollider4 sc)
                pen = Vector4.MinPerElem(transform4.localScale) * sc.radius;
            else if (collider is CapsuleCollider4 cc)
                pen = Vector4.MinPerElem(transform4.localScale) * (cc.radius + cc.height);

            var ray = Physics4.Raycast(new Ray4(center, gravity));
            
            if (ray.hit && ray.collider.rigidbody4.type == BodyType.StaticBody && ray.distance < pen)
            {
                velocity = Vector4.zero;
            } 
            else
            {
                velocity += Common.GRAVITY * Time.deltaTime;
            }
            transform4.position += velocity * Time.deltaTime;
        }
    }

}
