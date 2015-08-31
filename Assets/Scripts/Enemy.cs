using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    class Enemy : PathFollower
    {
        public static Enemy Instance;

		[SerializeField]
		private float spawnTime;		        

        [HideInInspector]
        public bool gameStarted;

        private Vector3 firstPosition;

		public void Start() {
			// Just shadow the superclass method to prevent it from running
		}

        public void Initiate(Vector3 pos)
        {            
            firstPosition = pos;
            
			Reset();
        }

        public void Reset()
        {
			gameStarted = false;

            GetComponent<SpriteRenderer>().enabled = false;

            transform.position = firstPosition;

            StopAllCoroutines();
            StartCoroutine(Spawn());
        }

        private void Awake()
        {
            Instance = this;
        }		        

		private IEnumerator Spawn()
		{
			yield return new WaitForSeconds(spawnTime);
			
			GetComponent<SpriteRenderer>().enabled = true;
			gameStarted = true;

			StartCoroutine(Move());
		}
    }
}