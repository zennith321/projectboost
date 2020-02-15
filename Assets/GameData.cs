using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{

    [SerializeField] int lives = 10;

    bool isCreated = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject gdInstance = GameObject.Find("GameData");
        if (!isCreated)
        {
            SetLives(3);
            DontDestroyOnLoad(this);
            isCreated = true;
        }

    }

    public void AddLives(int livesToAdd)
    {
        lives += livesToAdd;
    }

    public void RemoveLives(int livesToRemove)
    {
        lives -= livesToRemove;

    }

    public void SetLives(int newLives)
    {
        lives = newLives;
    }

    public int GetLives()
    {
        return lives;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
