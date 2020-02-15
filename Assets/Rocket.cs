using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour

{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 10f;
    [SerializeField] float levelLoadDelay = 1f;
    [SerializeField] GameData gameData;

    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip loadLevelSound;
    [SerializeField] AudioClip deathSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false;
    bool collisionsDisabled = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {
            RespondToThrustInput();
            RespondToRotationInput();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugInput();
        }
    }

    private void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; // toggle
        }
    }

    void OnCollisionEnter(Collision collision)
        {
          if (isTransitioning || collisionsDisabled) {  return; } // disable collisions on certain conditions

            switch (collision.gameObject.tag)
            {
                case "Friendly":
                    //do nothing
                    break;
                case "Finish":
                    StartSuccessSequence();
                    break;
                default:
                    StartDeathSequence();
                break;
            }
        }
    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(loadLevelSound);
        successParticles.Play();
        mainEngineParticles.Stop();
        Invoke("LoadNextScene", levelLoadDelay); 
    }

    private void StartDeathSequence()
    {
        gameData.RemoveLives(1);
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        mainEngineParticles.Stop();
        int curentLives = gameData.GetLives();
        if (curentLives <= 0)
        {
            Invoke("LoadFirstScene", levelLoadDelay);
        }
        else
        {
            Invoke("LoadCurrentScene", levelLoadDelay);
        }
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalSceneIndex = SceneManager.sceneCountInBuildSettings - 1;

        if (totalSceneIndex == currentSceneIndex)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            int nextSceneIndex = currentSceneIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    private void LoadFirstScene()
    {
        gameData.SetLives(3);
        SceneManager.LoadScene(0);
    }

    private void LoadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RespondToThrustInput()
    {

        float thrustThisFrame = mainThrust * Time.deltaTime; // calculate thrust based on framerate

        if (Input.GetKey(KeyCode.W)) // can thrust while rotating
        {
            ApplyThrust(thrustThisFrame);
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust(float thrustThisFrame)
    {
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSound);
            mainEngineParticles.Play();
            audioSource.loop = true;
        }
    }

    private void RespondToRotationInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rcsThrust * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rcsThrust * Time.deltaTime);
        }
    }

    private void RotateManually(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true; //take manual control of rotation
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; //resume physics control of rotation
    }
}
