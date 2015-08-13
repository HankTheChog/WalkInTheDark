using UnityEngine;

namespace Assets
{
    // Created by Ben Ukhanov (8.13.2015)
    public class BasicMovement : MonoBehaviour
    {
        // SerializeField attribute use it for show the variable on the Inspector.
        [SerializeField]
        private float speed;

        [SerializeField]
        private float gravity;

        private Vector3 moveDirection;
        private new Rigidbody rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Move();

            if (!OnGround())
            {
                moveDirection.y -= gravity * Time.fixedDeltaTime;
                return;
            }

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        }

        /// <summary>
        /// This function move the Player object.
        /// </summary>
        private void Move()
        {
            rigidbody.velocity = new Vector3(moveDirection.x * speed, moveDirection.y, moveDirection.z * speed);
        }

        /// <summary>
        /// The function checks if the Player on the Floor.
        /// By the way, Make sure the Ground Game Object contains "Floor" layer.
        /// </summary>
        private bool OnGround()
        {
            Debug.DrawRay(transform.position, Vector3.down); // Only for debug
            return Physics.Raycast(transform.position, Vector3.down, 0.6f, LayerMask.GetMask("Floor"));
        }
    }
}