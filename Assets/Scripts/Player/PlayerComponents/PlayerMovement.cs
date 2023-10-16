using UnityEngine;


namespace Player
{
    public class PlayerMovement : Player.PlayerComponent
    {
        public PlayerMovement(Player player, Rigidbody rb, PlayerStats stats)
        {
            this.player = player;
            this.rb = rb;
            this.stats = stats;
        }

        private Rigidbody rb;
        private PlayerStats stats;
        private Player player;

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
            //hardcoded speed
            velocity = inputDir * stats.MoveSpeed;
            rb.velocity = velocity;
        }

        private void Dash(Vector3 dashInput)
        {

        }

        public override void StartComponent()
        {
        }

        public override void UpdateComponent()
        {
        }
    }
}
