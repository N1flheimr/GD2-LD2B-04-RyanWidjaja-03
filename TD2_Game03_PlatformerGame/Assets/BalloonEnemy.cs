using System.Collections;
using UnityEngine;

namespace NifuDev
{
    public class BalloonEnemy : MonoBehaviour, IDamageable
    {
        [SerializeField] private float spawnTime;
        private bool isSpawning;
        private SpriteRenderer spriteRenderer;

        [SerializeField] private float jumpForce;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            isSpawning = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player") && collision.isTrigger)
            {
                if (collision.TryGetComponent<PlayerController>(out var playerController))
                {
                    if (!playerController.GetIsDashAttacking() && !isSpawning)
                    {
                        playerController.BalloonJump(jumpForce);
                    }
                    Damage();
                }
            }
        }

        public void Damage()
        {
            spriteRenderer.enabled = false;
            isSpawning = true;
            StartCoroutine(nameof(Spawn));
        }

        private IEnumerator Spawn()
        {
            yield return Helpers.GetWaitForSeconds(spawnTime);
            spriteRenderer.enabled = true;
            isSpawning = false;
            StopCoroutine(nameof(Spawn));
        }
    }
}

