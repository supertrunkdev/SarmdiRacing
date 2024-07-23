using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class DeactiveObject : MonoBehaviour
    {
        [SerializeField] private GameObject Deactive;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Deactive.SetActive(false);
            }
        }
    }
}
