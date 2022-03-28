using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.IO;
using UnityEngine.AI;

public class EnemyAI_A : MonoBehaviour
{
    public LayerMask whatIsPlayer;
    public bool playerInSightRange;
    public float sightRange;
    public bool playerInAttackRange;
    public float attackRange;
    public Transform player;
    public bool enemyDeadSeq1;
    public bool enemyOnGround;
    public GameObject scoreController;
    //public GameObject score
    public enum AIState
    {
        Walking,
        LookingForPlayer,
        LookingAtPlayer,
        ShootingAtPlayer,
        AttackingPlayer,
        Dead
    }
    public enum SearchForPlayerState
    {
        WalkForward,
        TurnAround
    }

    public Vector3 rotation;

    public int enemyHealth;
    private CharacterController controller;
    public float rotateSpeed;
    public float seachForPlayerSpeed;
    public AIState enemyState;

    public SearchForPlayerState searchState;
    //AIState enemyState = AIState.Walking;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        enemyState = AIState.Walking;
    }

    private void SearchForPlayer()
    {
        //enemy vision
        Ray rayEnemyVision = new Ray(transform.position, transform.forward);
        Physics.Raycast(rayEnemyVision, out RaycastHit rayEnemyVisionInfo);
       // Debug.Log(rayEnemyVisionInfo.collider.name);
        Debug.DrawRay(transform.position, transform.forward, Color.green);

        // Debug.Log(rayEnemyVisionInfo.collider.tag);
        // if (rayEnemyVisionInfo.collider.tag == "Player")
        // {
        //     enemyState = AIState.AttackingPlayer;
        // }
        
        // enemy walk
        Vector3 dir = new Vector3(0,0,0);
        switch (searchState)
        {
            case SearchForPlayerState.WalkForward:
            {
                //searchState = SearchForPlayerState.TurnAround;
                dir = transform.forward;
                if (rayEnemyVisionInfo.distance < 5)
                {
                    searchState = SearchForPlayerState.TurnAround;
                }
            } break;

            case SearchForPlayerState.TurnAround:
            {
                // rotate in player direction
                //if ()
                Quaternion target = Quaternion.Euler(rotation);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, Time.deltaTime * rotateSpeed);
                if (target == transform.rotation)
                {
                    //Debug.Log(rotation);
                    if (rotation.y == 270)
                    {
                      //  Debug.Log("hit1");
                        rotation.y = 90;
                    }
                    else if (rotation.y == 90)
                    {
                       // Debug.Log("hit2");
                        rotation.y = 270;
                    }
                    searchState = SearchForPlayerState.WalkForward;
                }
            } break;
        }
        controller.Move(dir * seachForPlayerSpeed * Time.deltaTime);
    }

    private void EnemyDead()
    {
        //FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

        // StreamReader reader = new StreamReader("Assets/highScore.txt");
        // string scoreReader = reader.ReadLine();
        // int scoreInt = int.Parse(scoreReader);
        // reader.Close();
        if (enemyDeadSeq1)
        {
            scoreController.GetComponent<ScoreManager>().currentScore++;
            FileStream fs = new FileStream("Assets/highScore.txt",FileMode.OpenOrCreate, 
                FileAccess.ReadWrite, 
                FileShare.None);

            StreamReader reader = new StreamReader(fs);
            string scoreReader = reader.ReadLine();
            int scoreInt = int.Parse(scoreReader);
            reader.Close();
        
            Debug.Log(scoreInt);
            Debug.Log("size:" + scoreController.GetComponent<ScoreManager>().currentScore);
            if (scoreInt < scoreController.GetComponent<ScoreManager>().currentScore)
            {
                StreamWriter writer = new StreamWriter("Assets/highScore.txt", false);
                writer.WriteLine(scoreController.GetComponent<ScoreManager>().currentScore);
                writer.Close();
            }

            enemyDeadSeq1 = false;
        }
        
        //StreamWriter writer = new StreamWriter("Assets/highScore.txt", true);
        //writer.Write(current)
    }
    private void AttackingPlayer()
    {
       
    }
    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case AIState.Walking:
            {
                SearchForPlayer();
                playerInSightRange = Physics.CheckSphere(transform.position, sightRange);
            } break;
            
            case AIState.AttackingPlayer:
            {
                AttackingPlayer();
            } break;

            case AIState.Dead:
            {
                EnemyDead();
            } break;
            
        }
        if (enemyHealth <= 0)
        {
            enemyState = AIState.Dead;
        }
    }
}
