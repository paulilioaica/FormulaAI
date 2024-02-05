using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CarDriverAgent : Agent
{

	private Rigidbody agentRb;
	private CheckpointSingle checkpointSingle;
	private CarDriver carDriver;
	
	private float startTime;
	private float bestTime = float.MaxValue;

	public Text lapTimeText;


	[SerializeField] TrackCheckpoints trackCheckpoints;

	private void Awake()
	{
		agentRb = GetComponent<Rigidbody>();
		carDriver = GetComponent<CarDriver>();
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		CheckpointSingle checkpoint = trackCheckpoints.GetNextCheckpoint(transform);
		Transform checkpointTransform = checkpoint.transform;

		Vector3 checkpointDistance = checkpointTransform.position - transform.position;

		sensor.AddObservation(agentRb.velocity);
		sensor.AddObservation(checkpointDistance);

		AddReward(-0.01f);
	}

	public override void OnEpisodeBegin()
	{
		carDriver.SpawnPosition();
		trackCheckpoints.ResetCheckpoint(transform);
		startTime = Time.time; // Record the start time of the lap  
	}

	public override void OnActionReceived(ActionBuffers actionBuffers)
	{
		ActionSegment<float> continuousActions = actionBuffers.ContinuousActions;

		float horizontal = continuousActions[0];
		float vertical = continuousActions[1];

		carDriver.SetInputs(vertical, horizontal);
	}

	public override void Heuristic(in ActionBuffers actionsOut)
	{
		ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

		continuousActions[0] = Input.GetAxis("Horizontal");
		continuousActions[1] = Input.GetAxis("Vertical");
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
		{
			// Hit a wall 
			AddReward(-0.09f);
		}
		if (collision.gameObject.TryGetComponent<CarDriver>(out CarDriver carDriver))
		{
			// Hit a car
			AddReward(-0.09f);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
		{
			// Hitting a wall 
			AddReward(-0.01f);
		}
		if (collision.gameObject.TryGetComponent<CarDriver>(out CarDriver carDriver))
		{
			// Hitting a car
			AddReward(-0.01f);
		}
	}


	private void OnTriggerEnter(Collider other)
	{
		bool isCorrect, isLastCheckpoint;

		if (other.gameObject.tag == "Checkpoint")
		{
			checkpointSingle = other.GetComponent<CheckpointSingle>();

			isCorrect = checkpointSingle.IsCorrectCheckPoint(carDriver);
			isLastCheckpoint = checkpointSingle.IsLastCheckpoint();

			if (checkpointSingle != null && carDriver != null)
			{
				if (isCorrect)
				{
					AddReward(+5f);
					if (isLastCheckpoint)
					{
						float lapTime = Time.time - startTime; // Calculate the lap time  
						Debug.Log("Lap Time: " + lapTime.ToString("F2"));
						if (lapTime < bestTime)
						{
							bestTime = lapTime;
							AddReward(+50f); // Give a large reward for a new best time  
						}
						else if (lapTime < bestTime * 1.1f)
						{
							AddReward(+10f); // Give a smaller reward for a good time  
						}
						else
						{
							AddReward(-5f); // Give a penalty for a slow time  
						}
						startTime = Time.time;
					}
				}
				else
				{
					AddReward(-0.05f);
				}
			}
		}
	}


}
