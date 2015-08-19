using UnityEngine;

// 8.16.2015
namespace Assets.Scripts
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        private Transform player;
        private BasicMovement basicMovement;

        private void Awake()
        {
          /*  player = GameObject.FindGameObjectWithTag("Player").transform;

            if (!player)
            {
                Debug.LogWarning("Player doesn't found!");
                return;
            }

            basicMovement = player.GetComponent<BasicMovement>();*/
        }
        private void LateUpdate()
        {
            /*if (!player || !basicMovement.OnGround())
                return;

            transform.position = Vector3.Lerp(transform.position, new Vector3(player.position.x, transform.position.y, player.position.z), speed * Time.deltaTime);
        */}
    }
}