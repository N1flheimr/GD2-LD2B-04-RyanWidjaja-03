using System.Collections.Generic;
using UnityEngine;

namespace NifuDev
{
    public class AddTime : MonoBehaviour, ICollectable
    {
        private void Start()
        {

        }
        public void Collect(PlayerController player)
        {
            player.RefillDash();
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                Collect(player);
                Debug.Log("Trigger");
            }
        }
    }
}

