using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

namespace Player
{
    public class Player : MonoBehaviour
    {
        public abstract class PlayerComponent
        {
            public abstract void StartComponent();
            public abstract void UpdateComponent();
        }

        [SerializeField]
        private PlayerStats stats;

        private Rigidbody rb;

        public PlayerMovement movementComponent;
        public PlayerShooting shootComponent;
        private List<PlayerComponent> components = new List<PlayerComponent>();

        private void CreatePlayerComponents()
        {
            movementComponent = new PlayerMovement(this, rb, stats);
            shootComponent = new PlayerShooting(this, stats);

            components.Add(movementComponent);
            components.Add(shootComponent);


        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            CreatePlayerComponents();
        }
        void Start()
        {
            foreach (var component in components)
            {
                component.StartComponent();
            }
        }
        void Update()
        {
            foreach (var component in components)
            {
                component.UpdateComponent();
            }
        }



        //Remove
        private void OnDrawGizmos()
        {
            var inputHandler = GetComponent<PlayerInputHandler>();

            var playerShootPoint = new Vector3(transform.position.x, 0, transform.position.z);
            var aimDir = (inputHandler.mouseAimPoint - playerShootPoint).normalized;

            Gizmos.DrawLine(playerShootPoint, playerShootPoint + aimDir);
        }
    }
}

