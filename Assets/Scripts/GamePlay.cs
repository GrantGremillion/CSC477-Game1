
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GamePlay : MonoBehaviour
{
    [HideInInspector] public PinballInput input;
    public BallScript ball;
    public Flipper flipperLeft;
    public Flipper flipperRight;
    public GameObject blockingWall;
    public GameObject canvas;
    public Transform centerOfBoard;
    public Transform cameraPosition;
    public GameObject soundManager;
    public TargetTracker targetTracker;
    public bool gameHasStarted = false;
    public bool readyToLaunch = true;
    public int scorePerTick = 100;
    public int defaultScorePerTick = 100;
    public int score = 0;
    public int lives = 5;
    public float timeOnBoard;


    public HandleUI ui;
    private Camera cam;
    private Rigidbody ballRB;
    private float timeOfLaunch;
    private float tickTime = 1f;
    private float timeOfLastTick;
    private float cameraSwivelSpeed = 1/4f;
    private float cameraSwivelDistance = 20f;
    private float cameraSwivelHeight = 12f;
    private int numOfTicksEarned = 0;
    public int HighScore { get; private set; }
    public int high { get; private set; }


    void Start () {
        input = new PinballInput();
        input.Enable();
        canvas.SetActive(true);
        cam = GetComponent<Camera>();
        ballRB = ball.GetComponent<Rigidbody>();
        HighScore = PlayerPrefs.GetInt(Consts.PlayerPrefs.HIGHSCORE, 0);
    }

    void Update()
    {
        handleKeyPresses();

        handleScore();

        if (Input.GetMouseButton(1)) // Right Click is pressed
            teleportBallToPosition(Input.mousePosition);

        swivelCamera();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    void handleKeyPresses()
    {
        if (gameHasStarted)
        {
            //////////////////////////////////////////////////  KEY  PRESSES  ////////////////////////////////////////////////////

            if (input.Default.FlipperLeftClick.WasPressedThisFrame())
                soundManager.GetComponent<SoundEffects>().playSoundFlipperL();

            if (input.Default.FlipperLeftClick.IsPressed())
                flipperLeft.Flip();

            if (input.Default.FlipperRightClick.WasPressedThisFrame())
                soundManager.GetComponent<SoundEffects>().playSoundFlipperR();

            if (input.Default.FlipperRightClick.IsPressed())
                flipperRight.Flip();

            if (input.Default.Launch.WasPressedThisFrame() && readyToLaunch) {
                ball.Launch();
                startBallTimer();
            }

            if (Input.GetKeyDown(KeyCode.R))
                ResetAll();

            if (Input.GetKeyDown(KeyCode.Escape))
                UnityEditor.EditorApplication.isPlaying = false;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        }
    }

    void handleScore()
    {
        if (gameHasStarted && !readyToLaunch)
        {
            timeOnBoard = Time.time - timeOfLaunch;
            if (Time.time - timeOfLastTick > tickTime)
            {
                timeOfLastTick = Time.time;
                score += scorePerTick;

                if (numOfTicksEarned % 5 == 0)
                    scorePerTick += 100;

                numOfTicksEarned++;
            }
        }
    }

    void teleportBallToPosition(Vector3 position)
    {
        if (gameHasStarted)
        {

            Ray ray = cam.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
            {
                Vector3 intersectionPoint = raycastHit.point;
                if (raycastHit.transform.tag == "Board")
                {
                    intersectionPoint.y = 4.7f;
                    ball.transform.position = intersectionPoint;
                    ballRB.velocity = Vector3.zero;
                }
            }
        }
    }

    void swivelCamera()
    {
        if (!gameHasStarted)
        {
            float timeFromStart = Time.timeSinceLevelLoad;
            float xOffSet = Mathf.Sin(timeFromStart * cameraSwivelSpeed + Mathf.PI) * cameraSwivelDistance;
            float zOffSet = Mathf.Cos(timeFromStart * cameraSwivelSpeed + Mathf.PI) * cameraSwivelDistance;

            cam.transform.position = new Vector3(centerOfBoard.position.x + xOffSet,
                                                 cameraSwivelHeight,
                                                 centerOfBoard.position.z + zOffSet);
            cam.transform.LookAt(centerOfBoard);
        }
    }

    public void enableBlockingWall()
    {
        blockingWall.GetComponent<MeshCollider>().enabled = true;
        blockingWall.GetComponent<MeshRenderer>().enabled = true;
    }

    public void disableBlockingWall()
    {
        blockingWall.GetComponent<MeshCollider>().enabled = false;
        blockingWall.GetComponent<MeshRenderer>().enabled = false;
    }

    public void startBallTimer()
    {
        timeOfLaunch = Time.time;
        timeOfLastTick = Time.time;
    }

    public void resetBallTimer()
    {
        timeOnBoard = 0f;
    }

    public void ResetAll()
    {
        ball.ResetBall();
        score = 0;
        lives = 5;
    }


    public void gameOver()
    {
        ui.endGame();
        gameHasStarted = false;

        high = PlayerPrefs.GetInt(Consts.PlayerPrefs.HIGHSCORE);
        
        // if player score is greater than highest score, update high score
        if (score > high){
            print("New high score!");
            PlayerPrefs.SetInt(Consts.PlayerPrefs.HIGHSCORE, score);
        }
        
    }
}
