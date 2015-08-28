using UnityEngine;
using System.Collections;

// 8.16.2015
namespace Assets.Scripts
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        private BasicMovement basicMovement;
        private new Camera camera;

        private void Awake()
        {
            basicMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<BasicMovement>();
            camera = GetComponent<Camera>();

            StartCoroutine(ShowTheMap());
        }

        /// <summary>
        /// Change the size of view until the camera will see the Player.
        /// </summary>
        private IEnumerator ShowTheMap()
        {
            while (!basicMovement.visibleByCamera)
            {
                camera.orthographicSize += speed * Time.deltaTime;
                yield return null;
            }

            float currentTime = Time.time;

            while (Time.time < currentTime + 0.2f)
            {
                camera.orthographicSize += speed * Time.deltaTime;
                yield return null;
            }
        }
    }
}

#region Deprecated
        /*
        private void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;

            if (!player)
            {
                Debug.LogWarning("Player doesn't found!");
                return;
            }

            basicMovement = player.GetComponent<BasicMovement>();
        }
        private void LateUpdate()
        {
            if (!player || !basicMovement.OnGround())
                return;

            transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, transform.position.y, player.position.z), speed * Time.deltaTime);
        }
        */
        #endregion