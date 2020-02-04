using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour

{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 10f;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {Alive, Dying, Transcending}
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
      // TODO somewhere stop sound on death
      if (state == State.Alive)
      {
        Thrust();
        Rotate();
      }
    }

    void OnCollisionEnter(Collision collision)
        {
          if (state != State.Alive) {  return; } // ignore collisions when dead

            switch (collision.gameObject.tag)
            {
              case "Friendly":
                //do nothing
                break;
              case "Finish":
                state = State.Transcending;
                Invoke("LoadNextScene", 1f); // parameterise time
                break;
              default:
              print("hit something deadly");
                state = State.Dying;
                Invoke("LoadFirstScene", 1f); // parameterise time
                break;
            }
        }

    private void LoadNextScene()
      {
        SceneManager.LoadScene(1); // TODO allow for more than 2 levels
      }

    private void LoadFirstScene()
      {
        SceneManager.LoadScene(0);
      }

    private void Thrust()
    {

        float thrustThisFrame = mainThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.W)) // can thrust while rotating
        {
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                audioSource.loop = true;
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true; //take manual control of rotation

        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; //resume physics control of rotation
    }
}
