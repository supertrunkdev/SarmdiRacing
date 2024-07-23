using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RGSK
{
    public class GameEnder : MonoBehaviour
    {
        [SerializeField] private GameObject[] objs;

        private void Start()
        {
            foreach (GameObject obj in objs)
            {
                obj.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Ended")) return;

            foreach (GameObject obj in objs)
            {
                obj.SetActive(true);
            }

            StartCoroutine(StopTimer());
        }

        IEnumerator StopTimer()
        {
            yield return new WaitForSeconds(3);

            SceneManager.LoadScene("DemoHub");
        }
    }
}
