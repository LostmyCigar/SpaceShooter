using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting;

namespace Player
{
    public class Player : MonoBehaviour, IDamagable
    {
        public abstract class PlayerComponent
        {
            public abstract void StartComponent();
            public abstract void UpdateComponent();
            public abstract void FixedUpdateComponent();
        }

        [SerializeField]
        private PlayerStats stats;

        [SerializeField] private BulletPool bulletPool;

        public PlayerMovement movementComponent;
        public PlayerShooting shootComponent;
        private List<PlayerComponent> components = new List<PlayerComponent>();

        private Camera cam;


        private void CreatePlayerComponents()
        {
            movementComponent = new PlayerMovement(this, transform, stats);
            shootComponent = new PlayerShooting(this, stats, bulletPool);

            components.Add(movementComponent);
            components.Add(shootComponent);
        }

        private void Awake()
        {
            cam = Camera.main;
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

            CheckIfPlayerOutOfBounds();
        }

        private void FixedUpdate()
        {
            foreach (var component in components)
            {
                component.FixedUpdateComponent();
            }
        }

        public void CheckIfPlayerOutOfBounds()
        {
            var playercamPos = cam.WorldToScreenPoint(transform.position);
            if (playercamPos.x > Screen.width || playercamPos.x < 0)
                transform.position = new(transform.position.x * -0.95f, transform.position.y, transform.position.z);
            if (playercamPos.y > Screen.height || playercamPos.y < 0)
                transform.position = new(transform.position.x, transform.position.y, transform.position.z * -0.95f);
        }

        public void TakeDamage(int damage)
        {
        }
    }
}

