using UnityEngine;

namespace Assets
{
    public class BasicMovement : MonoBehaviour
    {
        /// <note>Only guaranteed meaningful if using grid movement.</note>
        public Vector3 currentTile;

        ///<summary>If true, we are FUCKING FLYING, man!</summary>
        bool falling = true;

        [SerializeField]
        private float gravity;

        ///<summary>The size of the player mesh. Used for a redundant raycasting kludge to workaround extraneous physics. TEMP and ugly.</summary>
        [SerializeField]
        float size;

        /// <summary>y-value of rock bottom. Game restarts if player falls this low.</summary>
        [SerializeField]
        float lowestPoint;

        /// <summary>Time that started current grid-based move.</summary>
        private float moveStartTime = -1;

        /// <summary>Destination of current grid-based move.</summary>
        private Vector3 nextTile; // TODO: make this an actual tile reference.

        /// <summary>Speed, if using physics-based movement.</summary>
        [SerializeField]
        private float physicsSpeed;

        [SerializeField]
        private Animation screenFade;

        /// <summary>Time to traverse a tile (in s), if using grid-based movement.</summary>
        [SerializeField]
        float tileMoveTime;

        private Vector3 moveDirection;
        private new Rigidbody rigidbody;

        private Vector3 startPosition;

        /// <summary>If true, object will kinematically move along the grid in tile-sized increments once it hits the ground, else it will use physics-based movement.</summary>
        [SerializeField]
        bool useGrid;

        // If the camera see the Player, So this will be equals to true.
        [HideInInspector]
        public bool visibleByCamera;

        // Reset the position of the Player.
        public void Reset()
        {
            transform.position = startPosition;
            transform.eulerAngles = Vector3.zero;

            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            moveStartTime = -1;
            currentTile = transform.position;

            if (screenFade)
                screenFade.Play("UnFadeScreen");
        }

        /// <summary>Checks if the Player on the Floor.</summary>
        public bool OnGround()
        {
            return GetTile() != null;
        }

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>Returns the tile we're on, null if in midair.</summary>
        GameObject GetTile() {
            var hit = new RaycastHit();
            /*Debug.DrawRay(transform.position, Vector3.down); // Only for debug
			if (Physics.Raycast(transform.position, Vector3.down, out hit, 0.4f, LayerMask.GetMask("Floor"))) // assume anything in the floor layer is a tile
				return hit.collider.gameObject;*/
            if (Physics.SphereCast(transform.position, size / 2, Vector3.down, out hit, 0.2f, LayerMask.GetMask("Floor"))) // assume anything in the floor layer is a tile
                return hit.collider.gameObject;

            return null;
        }

        private void Update()
        {
            // Note that physics behavior generally belongs in FixedUpdate(), and input response in Update(), but since we aren't presently using the physics for anything but falling, I've elected to stick everything
            // in Update()
            Move();

            // Have we hit rock bottom?
            if (transform.position.y <= lowestPoint)
                // Yeah, restart
                Game.instance.Restart();

            var tile = GetTile();
            if (tile == null)
            {
                // We're in midair
                falling = true;
                moveDirection.y -= gravity * Time.deltaTime;
                return;
            }

            if (falling) {
                // Looks like we were falling, and just hit the ground. Align us with the current tile, so we can move on the grid
                falling = false;

                transform.eulerAngles = Vector3.zero;
                transform.position = new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z);
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
                currentTile = transform.position;

                // Possible kludge: also restart the gameclock. ASSUMPTION: no falling unless we're doing the "intro cinematic" or falling off the edge of the map
                Game.instance.StartClock();
            }

            // To make sure we restart the Player on the ground.
            if (startPosition == Vector3.zero) {
                startPosition = transform.position;
            }

            // We trying to move somewhere?
            if (!useGrid) {
                // Physics-based movement. Use GetAxis, with motion smoothing
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            } else {
                // Grid-based movement. Check inputs directly, for minimal latency. TODO: implementation that recognizes custom keymaps
                // Note that we only recognize one concurrent directional key. No diagonals etc.
                if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                    moveDirection = new Vector3(-1, 0, 0);
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    moveDirection = new Vector3(0, 0, 1);
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                    moveDirection = new Vector3(1, 0, 0);
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    moveDirection = new Vector3(0, 0, -1);
                } else
                    moveDirection = Vector3.zero;
            }
        }

        /// <summary>
        /// This function move the Player object.
        /// </summary>
        private void Move()
        {
            if (falling || !useGrid) {
                // Physics-based movement
                rigidbody.velocity = new Vector3(moveDirection.x * physicsSpeed, moveDirection.y, moveDirection.z * physicsSpeed);
                return;
            }

            // Grid-based movement
            transform.eulerAngles = Vector3.zero; // Keep orientation fixed
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
                    nextTile = transform.position + new Vector3(moveDirection.x, 0, moveDirection.z) * Game.instance.tileSize;
                    moveStartTime = Time.time;

                    // Report start of new move to game
                    Game.instance.OnStartMove();
                }

                // Also, keep it kinematic
                transform.position = currentTile;
            }
        }

        // Obstacle need to be with tag "Obstacle"!
        private void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.CompareTag("Obstacle"))
            {
                Game.instance.Die();
                Scripts.Enemy.Instance.Reset();
            }
        }
		private void OnTriggerEnter(Collider trigger)
		{
            if (trigger.gameObject.CompareTag("Goal"))
            {
                Game.instance.Win();
                Scripts.Enemy.Instance.Reset();
            }

            if (Scripts.Enemy.Instance.gameStarted && trigger.gameObject.CompareTag("Enemy"))
            {
                Game.instance.Die();
                Scripts.Enemy.Instance.Reset();
            }
        }

        // - By camera.
        private void OnBecameVisible()
        {
            visibleByCamera = true;
        }
    }
}