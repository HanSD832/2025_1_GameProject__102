using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTurnManager : MonoBehaviour
{
    public static bool canPlay = true;
    public static bool anyBallMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckAllBalls();

        if (!anyBallMoving && !canPlay) 
        {
            canPlay = true;
            Debug.Log("턴 종료! 다시 칠 수 있습니다.");
        }
    }

    void CheckAllBalls()
    {
        SimpleBallController[] allBalls = FindObjectsOfType<SimpleBallController>();
        anyBallMoving = false;

        foreach(SimpleBallController ball in allBalls)
        {
            if (ball.IsMoving())
            {
                anyBallMoving = true;
                break;
            }
        }
    }

    public static void OnBallHit()
    {
        canPlay = false;
        anyBallMoving = true;
        Debug.Log("턴 시작! 공이 멈출 때까지 기다리세요.");
    }
}
