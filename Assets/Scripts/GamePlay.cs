using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    public BallScript ball;
    public Flipper flipperLeft;
    public Flipper flipperRight;
    public bool readyToLaunch;
    public int score = 0;

    private float timeOfLaunch;
    private int scorePerTick = 100;
    private float tickTime = 0.5f;
    private float timeOnBoard;
    private float timeOfLastTick;

    // Start is called before the first frame update
    void Start()
    {
        readyToLaunch = true;
    }

    // Update is called once per frame
    void Update()
    {
        handleKeyPresses();

        if (!readyToLaunch)
        {
            timeOnBoard = Time.time - timeOfLaunch;
            if (timeOnBoard - timeOfLastTick > tickTime)
            {
                timeOfLastTick = Time.time;
                score += scorePerTick;
                print(score);
            }
        }


    }

    void handleKeyPresses()
    {
        if ((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow)) && readyToLaunch)
        {
            ball.Launch();
            readyToLaunch = false;
            timeOfLaunch = Time.time;
            timeOfLastTick = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            flipperLeft.Flip();

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            flipperRight.Flip();
    }
}
