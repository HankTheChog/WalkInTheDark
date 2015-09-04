using UnityEngine;

namespace Assets
{
    public class BasicMovement : MonoBehaviour
    {
        /// <note>Only guaranteed meaningful if using grid movement.</note>
		[HideInInspector]
        public Vector3 currentTile;

        /// <summary>Time at which the current grid-based move started.</summary>
        private float moveStartTime = -1;

        /// <summary>Destination of current grid-based move.</summary>
        private Vector3 nextTile; // TODO: make this an actual tile reference.

        [SerializeField]
        private Animation screenFade;

        /// <summary>Time to traverse a tile (in s), if using grid-based movement.</summary>
        [SerializeField]
        float tileMoveTime;

        private Vector3 moveDirection;

		[HideInInspector]
        public Vector3 startPosition;

        // If the camera see the Player, So this will be equals to true.
        [HideInInspector]
        public bool visibleByCamera;

		/// <summary>If false, sprite will use an unlit material, meaning it will be visible even in darkness.</summary>
		public bool isLit {
			get {
				return transform.FindChild("LitSprite").GetComponent<SpriteRenderer>().enabled;
			}

			set {
				transform.FindChild("LitSprite").GetComponent<SpriteRenderer>().enabled = value;
				transform.FindChild("UnlitSprite").GetComponent<SpriteRenderer>().enabled = !value;
			}
		}

        // Reset the position of the Player.
        public void Reset()
        {
            transform.position = startPosition;

            moveStartTime = -1;
            currentTile = transform.position;

            if (screenFade)
                screenFade.Play("UnFadeScreen");
        }		    

        private void Update()
        {
            Move();		
			            
            // Note that we only recognize one concurrent directional key. No diagonals etc.
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                moveDirection = new Vector3(-1, 0);
				transform.eulerAngles = new Vector3(0, 0, 90);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                moveDirection = new Vector3(0, 1);
				transform.eulerAngles = Vector3.zero;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                moveDirection = new Vector3(1, 0);
				transform.eulerAngles = new Vector3(0, 0, 270);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                moveDirection = new Vector3(0, -1);
				transform.eulerAngles = new Vector3(0, 0, 180);
            } else
                moveDirection = Vector3.zero;            
        }

        private void Move()
        {            
            // Grid-based movement            
            if (moveStartTime >= 0) {
                // Continue current move
                var ratio = (Time.time - moveStartTime) / tileMoveTime;
                if (ratio >= 1f) {
                    // Arrived
                    transform.position = nextTile;
                    currentTile = nextTile;
                    moveStartTime = -1;
                } else {
                    // Still en route
                    transform.position = Vector3.Lerp(currentTile, nextTile, ratio);
                }
            } else {
                // Not moving, try to start new move
                if (moveDirection != Vector3.zero) {
                    B.Assert(moveDirection.sqrMagnitude == 1f);
                    var target = transform.position + moveDirection * Game.instance.tileSize;


					if (Game.instance.wallsLethal || !Game.instance.Blocked(new Vector2(target.x, target.y))) {
						// No obstacle found, or we're allowed to move into obstacles. Begin move.
						nextTile = target;

						moveStartTime = Time.time;
						
						// Report start of new move to game
						Game.instance.OnStartMove();
					}										
				}

                // Also, keep it kinematic
                transform.position = currentTile;
            }

			// Kludge: die if we exit the map bounds (or otherwise end up inside a "blocked" tile without dying in the collision callback)
			// This is ugly and less than robust
			if (Game.instance.Blocked(transform.position)) {
				B.Assert(Game.instance.wallsLethal);
				Game.instance.Die();
			}

        }

        // Obstacle need to be with tag "Obstacle"!       
		private void OnTriggerEnter2D(Collider2D trigger) {
			OnTriggerStay2D(trigger);
		}

		private void OnTriggerStay2D(Collider2D trigger) {
			if (trigger.gameObject.CompareTag("Hazard"))			
				Game.instance.Die();
			else if (trigger.gameObject.CompareTag("Obstacle"))	{		
				B.Assert(Game.instance.wallsLethal);
				Game.instance.Die();
			}
			else if (trigger.gameObject.CompareTag("Goal"))			
				Game.instance.Win();			
			else if (Scripts.Enemy.Instance.gameStarted && trigger.gameObject.CompareTag("Enemy"))			
				Game.instance.Die();                	
		}

        // - By camera.
        private void OnBecameVisible()
        {
            visibleByCamera = true;
        }
    }
}