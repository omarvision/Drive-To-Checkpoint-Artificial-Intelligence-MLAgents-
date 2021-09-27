using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(DecisionRequester))]
public class CarAgent : Agent
{
    private float Movespeed = 16;
    private float Turnspeed = 90;
    private Rigidbody rb = null;
    private Vector3 recall_position;
    private Quaternion recall_rotation;
    private string CurrentWall;
    private Checkpoints CheckpointScript = null;

    public override void Initialize()
    {
        rb = this.GetComponent<Rigidbody>();
        CheckpointScript = GameObject.Find("Checkpoints").GetComponent<Checkpoints>();
        recall_position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        recall_rotation = new Quaternion(this.transform.rotation.x, this.transform.rotation.y, this.transform.rotation.z, this.transform.rotation.w);
    }
    public override void OnEpisodeBegin()
    {
        this.transform.position = recall_position;
        this.transform.rotation = recall_rotation;
        CurrentWall = "wall (1)";
    }
    public override void Heuristic(float[] actionsOut)
    {
        float move = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");

        if (move < 0)
            actionsOut[0] = 0;
        else if (move == 0)
            actionsOut[0] = 1;
        else if (move > 0)
            actionsOut[0] = 2;

        if (turn < 0)
            actionsOut[1] = 0;
        else if (turn == 0)
            actionsOut[1] = 1;
        else if (turn > 0)
            actionsOut[1] = 2;
    }
    public override void OnActionReceived(float[] vectorAction)
    {
        //  Vector Action:
        //      Space Type: Discrete
        //          Branches Size: 2
        //              Branch 0 Size: 3 values (0=reverse, 1=noaction, 2=forward)
        //              Branch 1 Size: 3 values (0=turnleft, 1=noaction, 2=turnright)

        switch (vectorAction[0])
        {
            case 0: //back
                rb.AddRelativeForce(Vector3.back * Movespeed * Time.deltaTime, ForceMode.VelocityChange);
                AddReward(-0.01f);
                break;
            case 1: //noaction
                AddReward(-0.3f);
                break;
            case 2: //forward
                rb.AddRelativeForce(Vector3.forward * Movespeed * Time.deltaTime, ForceMode.VelocityChange);
                AddReward(0.0001f);
                break;
        }

        switch (vectorAction[1])
        {
            case 0: //left
                this.transform.Rotate(Vector3.up, -Turnspeed * Time.deltaTime);
                break;
            case 1: //noaction
                break;
            case 2: //right
                this.transform.Rotate(Vector3.up, Turnspeed * Time.deltaTime);
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint") == true)
        {
            if (other.name == CurrentWall)
            {
                Debug.Log("hit " + other.name);
                AddReward(1.0f);
                EndEpisode();
                //CurrentWall = CheckpointScript.GetNextCheckpointName(CurrentWall);
                //if (CurrentWall == "wall (0)")
                //{
                //    AddReward(1.0f);
                //    EndEpisode();
                //}
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Barrier") == true)
        {
            Debug.Log("hit " + collision.gameObject.name);
            AddReward(-1.0f);
            EndEpisode();
        }
    }
}
