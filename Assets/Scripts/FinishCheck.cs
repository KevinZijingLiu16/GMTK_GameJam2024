using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AE0672
{
    public class FinishCheck : MonoBehaviour
    {

       AudioSource finishSound;
        //declare a variable named finishSound of type AudioSource. Regarding to OOP, finishSound is like a reference to an instance of AudioSource class.

        private bool isFinished = false;
        // 

        // Start is called before the first frame update
        void Start()
        {
            finishSound = GetComponent<AudioSource>();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name == "Player" && !isFinished)
            {
                finishSound.Play();
                isFinished = true;
                Invoke("CompleteLevel", 2f);
                
            }
        }

        private void CompleteLevel()
        {


            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }

        }

    }

