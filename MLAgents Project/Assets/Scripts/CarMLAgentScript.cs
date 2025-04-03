using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine.SocialPlatforms.Impl;
using Unity.VisualScripting;

public class CarMLAgentScript : Agent
{

    public GameObject RewardWallsObject;
    private RewardWallScript RewardWallScript;

    private CarController carController;


    private int NumberOfRewardWalls;
    private int NextRewardWallNumber;
    private GameObject NextRewardWallObject;






    private float Score;
    public TextMeshProUGUI ScoreText;





    private int LastActionHorizontal;
    private int LastActionVertical;




    public override void Initialize()
    {
        base.Initialize();
        carController = transform.GetComponent<CarController>();

        LastActionHorizontal = 1; LastActionVertical = 1;

        RewardWallScript = RewardWallsObject.GetComponent<RewardWallScript>();
        NumberOfRewardWalls = RewardWallScript.getNumberOfRewardWalls();
    }

    public override void OnEpisodeBegin()
    {
        Score = 0;
        ScoreText.text = Score.ToString();

        carController.StartReset();

        NextRewardWallNumber = 0;
        NextRewardWallObject = RewardWallScript.getRewardWallWithIndex(0);
    }



    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position / 100.0f);
        sensor.AddObservation(NextRewardWallObject.transform.position / 100.0f);

        Vector3 NextRewardWallForward = NextRewardWallObject.transform.forward;
        float directionDot = Vector3.Dot(transform.forward, NextRewardWallForward);
        sensor.AddObservation(directionDot);

        sensor.AddObservation(carController.getSpeed());

    }



    public override void OnActionReceived(ActionBuffers actions)
    {
        //  1st discrete action -> accelarate or brake
        LastActionVertical = actions.DiscreteActions[0];
        switch (LastActionVertical)        
        {
            case 0: carController.brake(); break;
            case 2: carController.accelerate(); break;
            case 1: carController.applyFriction(); break;
        }

        //  2nd discrete action -> left or right
        LastActionHorizontal = actions.DiscreteActions[1];
        switch (LastActionHorizontal)
        {
            case 0: carController.turnLeft(); break;
            case 2: carController.turnRight(); break;
            case 1: break;  // no turn
        }

        
        //  Adding reward if speed is high
        if (carController.getSpeed()> 10) AddReward(0.5f);

        
        //  Addiing reward based on distance
        float distanceToWall = Vector3.Distance(transform.position, NextRewardWallObject.transform.position);
        float maxDistance = 10.0f; // Adjust based on your environment
        float distanceReward = 1.0f - Mathf.Clamp(distanceToWall / maxDistance, 0.0f, 1.0f);
        AddReward(distanceReward * 0.1f); // Adjust the factor (0.1f) to tune the reward magnitude
        

        //  Addiing reward if looking towards the wall
        Vector3 directionToWall = (NextRewardWallObject.transform.position - transform.position).normalized;
        float alignment = Vector3.Dot(transform.forward, directionToWall);        
        AddReward(alignment * 0.1f);





    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.UpArrow)) discreteActions[0] = 2;
        else if (Input.GetKey(KeyCode.DownArrow)) { 
            discreteActions[0] = 0; 
        }
        else discreteActions[0] = 1;


        if (Input.GetKey(KeyCode.LeftArrow)) discreteActions[1] = 0;
        else if (Input.GetKey(KeyCode.RightArrow)) discreteActions[1] = 2;
        else discreteActions[1] = 1;
    }





    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("RewardWall"))
        {

            bool NextWallWasHit = other.gameObject == NextRewardWallObject;

            if (NextWallWasHit)
            {
                NextRewardWallNumber++;
                if (NextRewardWallNumber >= NumberOfRewardWalls) NextRewardWallNumber = 0;
                NextRewardWallObject = RewardWallScript.getRewardWallWithIndex(NextRewardWallNumber);

                AddReward(2f);
                Score += 2;
                Debug.Log("+2");
            }
            else
            {
                //  Wrong Reward Wall
                AddReward(-2f);
                Score -= 2;
                Debug.Log("-2");
            }
        }

        ScoreText.text = Score.ToString();
    }







    private float CarWallCollisionPunishment = -5f;
    private float CarWallCollisionStayPunishment = -1f;


    //  Collissions with the wall give negative reward

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            AddReward(CarWallCollisionPunishment);
            Score += CarWallCollisionPunishment;
            Debug.Log(CarWallCollisionPunishment);
        }
        ScoreText.text = Score.ToString();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            AddReward(CarWallCollisionStayPunishment);
            Score += CarWallCollisionStayPunishment;
            Debug.Log(CarWallCollisionStayPunishment);
        }
        ScoreText.text = Score.ToString();
    }




}
