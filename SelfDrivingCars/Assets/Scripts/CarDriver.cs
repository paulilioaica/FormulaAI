using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDriver : MonoBehaviour
{
	public List<AxleInfo> axleInfos;
	public float maxMotorTorque;
	public float maxSteeringAngle;

	private float forwardAmount;
	private float turnAmount;
	private float brake = 10f;
	private float currentSpeed = 0f;
	private float accelerationRate = 5f;
	private float understeerEffect = 1f;
	private float oversteerEffect = 1f;

	
	private Vector3 spawnPos;
	private Quaternion spawnRot;

	private void Awake()
	{
		spawnPos = transform.localPosition;
		spawnRot = transform.rotation;
	}

	public void SpawnPosition()
	{
		transform.localPosition = spawnPos;
		transform.rotation = spawnRot;
	}

	public void StopMovement()
	{
		foreach (AxleInfo axleInfo in axleInfos)
		{
			if (axleInfo.steering)
			{
				axleInfo.leftWheel.steerAngle = 0f;
				axleInfo.leftWheel.steerAngle = 0f;
			}
			if (axleInfo.motor)
			{
				axleInfo.leftWheel.motorTorque = 0f;
				axleInfo.rightWheel.motorTorque = 0f;
			}
		}
	}

	private void FixedUpdate()
	{
		float motor = maxMotorTorque * forwardAmount * 5f;
		float steering = maxSteeringAngle * turnAmount;

		if (forwardAmount > 0)
		{
			currentSpeed += forwardAmount * accelerationRate * Time.fixedDeltaTime;
		}

		foreach (AxleInfo axleInfo in axleInfos)
		{
			if (axleInfo.steering)
			{
				float adjustedSteering = steering;

				if (currentSpeed > maxMotorTorque)
				{
					adjustedSteering /= understeerEffect;
				}
				else if (currentSpeed < maxMotorTorque)
				{
					adjustedSteering *= oversteerEffect;
				}

				axleInfo.leftWheel.steerAngle = adjustedSteering;
				axleInfo.rightWheel.steerAngle = adjustedSteering;
			}

			if (axleInfo.motor)
			{
				if (forwardAmount == 0)
				{
					axleInfo.leftWheel.brakeTorque = brake;
					axleInfo.rightWheel.brakeTorque = brake;
				}
				else if (forwardAmount < 0)
				{
					axleInfo.leftWheel.brakeTorque = 0f;
					axleInfo.rightWheel.brakeTorque = 0f;

					axleInfo.leftWheel.motorTorque = motor / 10;
					axleInfo.rightWheel.motorTorque = motor / 10;
				}
				else
				{
					axleInfo.leftWheel.brakeTorque = 0f;
					axleInfo.rightWheel.brakeTorque = 0f;

					axleInfo.leftWheel.motorTorque = motor;
					axleInfo.rightWheel.motorTorque = motor;
				}
			}
			ApplyLocalPositionToVisuals(axleInfo.leftWheel);
			ApplyLocalPositionToVisuals(axleInfo.rightWheel);
		}
	}

	public void ApplyLocalPositionToVisuals(WheelCollider collider)
	{
		if (collider.transform.childCount == 0)
		{
			return;
		}

		Transform visualWheel = collider.transform.GetChild(0);

		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);

		visualWheel.transform.position = position;
		visualWheel.transform.rotation = rotation;
	}

	public void SetInputs(float forwardAmout, float turnAmount)
	{
		this.forwardAmount = forwardAmout;
		this.turnAmount = turnAmount;
	}
}

[System.Serializable]
public class AxleInfo
{
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor;
	public bool steering;
}
