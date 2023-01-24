using System.Collections;
using UnityEngine;

namespace NifuDev
{
    public class AddTime : MonoBehaviour, ICollectable
    {
        [SerializeField] private float spawnTime;
        private bool isSpawning;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Collect(PlayerController player)
        {
            player.RefillDash();
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

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isSpawning)
            {
                if (collision.CompareTag("Player") && collision.isTrigger)
                {
                    PlayerController player = collision.GetComponent<PlayerController>();
                    Collect(player);
                }
            }
        }
    }
}

