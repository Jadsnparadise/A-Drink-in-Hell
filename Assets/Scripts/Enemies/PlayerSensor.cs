using System;
using UnityEngine;

namespace Enemies
{
    public class PlayerSensor : MonoBehaviour
    {
        [NonSerialized] public Collider2D Collider = null;

        protected void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                Collider =  other;
        }

        protected void OnTriggerExit2D(Collider2D other)
        {
            if  (other.CompareTag("Player"))
                Collider = null;
        }
    }
}