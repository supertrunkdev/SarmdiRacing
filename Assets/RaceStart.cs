using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RGSK
{
    public class RaceStart : MonoBehaviour
    {
        [SerializeField] private GameObject[] gameObjects;
        [SerializeField] private float timer = 3f;
        // Start is called before the first frame update
        void Start()
        {
            Invoke("StartObject", timer);
        }

        private void StartObject()
        {
            foreach (var obj in gameObjects)
            {
                obj.SetActive(true);
            }

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
