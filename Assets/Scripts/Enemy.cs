using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    class Enemy : MonoBehaviour
    {
        public static Enemy Instance;

        // Time.
        [SerializeField]
        private float speed;

        [HideInInspector]
        public bool gameStarted;

        private Vector3 firstPosition;

        public void Initiate(Vector3 pos)
        {
            GetComponent<MeshRenderer>().enabled = false;

            firstPosition = pos;
            transform.position = pos;

            StartCoroutine(Move());
        }
        public void Reset()
        {
            GetComponent<MeshRenderer>().enabled = false;

            transform.position = firstPosition;

            StopAllCoroutines();
            StartCoroutine(Move());
        }

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// This method checks if there isn't obstacle on right.
        /// If yes, Enemy will move forward.
        /// If not, Enemy will move right.
        /// </summary>
        private bool CanMoveRight()
        {
            return !Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z),
                new Vector3(1, 0, 0), 
                1, 
                LayerMask.GetMask("Obstacle"));
        }

        private IEnumerator Move()
        {
            yield return new WaitForSeconds(3);

            GetComponent<MeshRenderer>().enabled = true;
            gameStarted = true;

            while (true)
            {
                if (CanMoveRight())
                    transform.Translate(new Vector3(1, 0, 0));
                else
                    transform.Translate(Vector3.forward);
                yield return new WaitForSeconds(speed);
            }
        }
    }
}