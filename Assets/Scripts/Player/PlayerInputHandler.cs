using UnityEngine;
using UnityEngine.InputSystem;


namespace Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        //Let makes this easy peasy 
        [SerializeField] private EnemySpawner spawner;

        private Player player;

        public Vector3 MoveDir;
        public Vector3 mouseAimPoint;
        public bool mouse1Pressed;

        Plane plane = new Plane(Vector3.up, 0);
        float distance;
        Vector2 mouseScreenPos;


        private void Awake()
        {
            player = GetComponent<Player>();
        }

        public void MoveInput(InputAction.CallbackContext context)
        {
            var x = context.ReadValue<Vector2>().x;
            var z = context.ReadValue<Vector2>().y;

            MoveDir = new(x, 0, z);
        }

        public void SpawnInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                spawner.SpawnEnemies();
                Debug.Log("Spawn!");
            }

        }

        public void ToggleJobs(InputAction.CallbackContext context)
        {
            if (context.started) { spawner.ToggleJobs();}
        }

        public void TogglePI(InputAction.CallbackContext context)
        {
            if (context.started) { spawner.TogglePiAndSleep(); }
        }

        public void MouseAimInput(InputAction.CallbackContext context)
        {
            mouseScreenPos = context.ReadValue<Vector2>();
        }

        public void ShootInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                mouse1Pressed = true;
            }

            if (context.canceled) {
                mouse1Pressed = false;
            }
        }



        //This is w/e, we dont care about archetecture anyway
        private void Update()
        {
            if (mouse1Pressed)
            {
                player.shootComponent.GetShootInput(mouseAimPoint);
            }

            player.movementComponent.GetMoveInput(MoveDir);

            var mouseRay = Camera.main.ScreenPointToRay(mouseScreenPos);
            if (plane.Raycast(mouseRay, out distance))
            {
                mouseAimPoint = mouseRay.GetPoint(distance);
            }
        }
    }
}
