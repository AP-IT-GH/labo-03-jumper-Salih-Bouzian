using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class JumperAgent : Agent
{
    public Transform Obstacle;
    public float ObstacleSpeed;
    private bool hasJumped = false;
    private bool NegativeReceived = false;
    private bool Passed = false;
    private bool goodJumpTime = false;
    private bool blockNegative = false;
    public Rigidbody rb;
    public Rigidbody obstacleRb;

    public override void OnEpisodeBegin(){
        ObstacleSpeed = Random.Range(4f, 7f);


        this.transform.localPosition = new Vector3(0,0.5f,0);
        this.Obstacle.localPosition = new Vector3(0,0.5f,9);
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(Obstacle.localPosition);
        sensor.AddObservation(this.transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int jumpAction = actionBuffers.DiscreteActions[0];

        if (jumpAction == 1 && !hasJumped)
        {
            hasJumped = true;
            rb.AddForce(Vector3.up * 5f, ForceMode.VelocityChange);
        } 


        if (hasJumped && goodJumpTime && !Passed)
        {
            Debug.Log("Goede jump +");
            blockNegative = true;
            AddReward(1.0f);
            Passed = true;
        } else if(hasJumped  && !goodJumpTime && !blockNegative &&!NegativeReceived){
            Debug.Log("Slechte jump -");
            AddReward(-0.05f);
            NegativeReceived = true;
        }
    }

    public void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Plane"))
            {
            //check collision met grond.
            hasJumped = false;
            NegativeReceived = false;
            blockNegative = false;
            }

        if(other.gameObject.CompareTag("Obstacle"))
        {
            //check collision met de obstakel
            Debug.Log("Obstakel geraakt grote fout");
            AddReward(-1.0f);
            this.transform.localPosition = new Vector3(0,0.5f,0);
            this.Obstacle.localPosition = new Vector3(0,0.5f,9);
            Passed = false;
            NegativeReceived = false;
            goodJumpTime = false;            

        }
    }


    void Update()
    {
        Vector3 moveDirection = Vector3.back * ObstacleSpeed * Time.deltaTime;
        obstacleRb.MovePosition(obstacleRb.position + moveDirection);

        if (obstacleRb.position.z < -9f)
        {
            Passed = false;
            NegativeReceived = false;
            ObstacleSpeed = Random.Range(4f, 7f);
            obstacleRb.position = new Vector3(0, 0.5f, 9);
        }

        if (obstacleRb.position.z < 3.4f)
        {
            goodJumpTime = true;
            
        }

        if (obstacleRb.position.z < 0.7f)
        {
            goodJumpTime = false;            
        }
        

        if(this.transform.localPosition.y < 0){
            EndEpisode();
        }
    }
    

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        
    }
    
}
