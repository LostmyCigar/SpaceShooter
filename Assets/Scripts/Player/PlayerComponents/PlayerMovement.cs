using UnityEngine;


namespace Player
{
    public class PlayerMovement : Player.PlayerComponent
    {
        public PlayerMovement(Player player, Transform transform, PlayerStats stats)
        {
            this.player = player;
            this.transform = transform;
            this.stats = stats;
        }

        private Transform transform;
        private PlayerStats stats;
        private Player player;

        private Vector3 inputVelocity;
        private Vector3 velocity;

        public void GetMoveInput(Vector3 moveInput)
        {
            moveInput.Normalize();
            Move(moveInput);
        }

        public void GetDashInput(Vector3 dashInput)
        {
            dashInput.Normalize();
            Dash(dashInput);
        }

        private void Move(Vector3 inputDir)
        {
            inputVelocity = inputDir * stats.MoveSpeed;
        }

        private void Dash(Vector3 dashInput)
        {

        }

        public override void StartComponent()
        {
        }

        public override void UpdateComponent()
        {
            velocity += inputVelocity * Time.deltaTime;

            transform.LookAt(transform.position + velocity);
        }

        public override void FixedUpdateComponent()
        {
            velocity /= stats.Drag;

            if (velocity.magnitude > stats.MaxSpeed)
            {
                velocity = velocity.normalized * stats.MaxSpeed;
            }

            transform.position += velocity;
        }
    }
}
