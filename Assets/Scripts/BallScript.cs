using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BallScript : MonoBehaviour
{
    private Rigidbody rb;
    public float launchForce;
    public Transform ballStart;
    public Transform ballEnd;
    public Transform inCannon;
    public GamePlay gameScript;
    public HandleUI ui;
    public GameObject soundManager;
    public GameObject cannon;
    public ParticleSystem cannonFlame;
    
    private float distanceToReset;
    private float distanceToLaunch = 1.5f;
    private float x_limit = 6f;
    private float z_limit = 4f;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        distanceToReset = ballEnd.GetComponent<SphereCollider>().radius;
    }


    void Update()
    {
        if (Vector3.Distance(transform.position, ballEnd.position) < distanceToReset) 
        {
            ResetBall();
            gameScript.resetBallTimer();
            gameScript.lives -= 1;

            if (gameScript.lives < 1)
            {
                ui.endGame();
                gameScript.gameHasStarted = false;
            }
        }

        checkReadyToLaunch();
    }

    public void Launch()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.forward * launchForce, ForceMode.Impulse);
    }

    public void ResetBall()
    {
        transform.position = ballStart.transform.position;
        rb.velocity = Vector3.zero;
        gameScript.readyToLaunch = true;
        gameScript.disableBlockingWall();
        gameScript.scorePerTick = gameScript.defaultScorePerTick;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Anchor")
        {
            gameScript.score += 500;
            soundManager.GetComponent<SoundEffects>().playSound();
        }
    }

    public void checkReadyToLaunch()
    {
        if (transform.position.x < x_limit)
            gameScript.enableBlockingWall();

        if (transform.position.x > x_limit && transform.position.z < z_limit)
            gameScript.disableBlockingWall();

        if (Vector3.Distance(transform.position, inCannon.position) < distanceToLaunch)
        {
            gameScript.resetBallTimer();
            gameScript.readyToLaunch = true;
            cannonFlame.Play();
            cannonFlame.gameObject.SetActive(true);
        }
        else
        {
            if (gameScript.readyToLaunch)
                gameScript.startBallTimer();

            gameScript.readyToLaunch = false;
            cannonFlame.Pause();
            cannonFlame.gameObject.SetActive(false);
            
        }

    }

}
