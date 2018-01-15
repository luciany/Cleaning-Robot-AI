﻿using UnityEngine;
using System.Collections.Generic;

public class Robot : MonoBehaviour
{
    private LearningSceneManager manager;

    private float speed;
    private float rotationSpeed;

    private Dictionary<string, bool> sensors = new Dictionary<string, bool>();

    private float absoluteRotationDeg = 0;
    private float robotWidth;
    private bool dead = false;
    private CleanedSpaceMap map;
    private Perceptron brain;

    void Awake()
    {
        sensors.Add("Left Sensor", false);
        sensors.Add("Front Sensor", false);
        sensors.Add("Right Sensor", false);
        sensors.Add("Left Button", false);
        sensors.Add("Front Button", false);
        sensors.Add("Right Button", false);
        sensors.Add("Right Wall Sensor", false);

        sensors.Add("Left Grid Cleaned", false);
        sensors.Add("Front Grid Cleaned", false);
        sensors.Add("Right Grid Cleaned", false);
    }

    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<LearningSceneManager>();

        robotWidth = GetComponent<Renderer>().bounds.size.x;
        map = new CleanedSpaceMap(robotWidth);
    }

    void Update()
    {
        sensors["Left Grid Cleaned"] = map.IsLeftGridCleaned();
        sensors["Front Grid Cleaned"] = map.IsFrontGridCleaned();
        sensors["Right Grid Cleaned"] = map.IsRightGridCleaned();

        bool[] sensorsArray = new bool[sensors.Count];
        sensors.Values.CopyTo(sensorsArray, 0);

        float[] outputValues = brain.Guess(sensorsArray);

        speed = outputValues[0];
        rotationSpeed = outputValues[1] * 2 - 1;

        Rotate();
        Move();
    }

    public void setADN()
    {
        brain = new Perceptron();
    }

    public void setADN(float[] ADN) {
        brain = new Perceptron(ADN);
    }

    private void Move()
    {
        float displacement = speed * Time.deltaTime;
        transform.Translate(Vector3.forward * displacement);
        map.MovePlayerPosition(displacement);
    }
    private void Rotate()
    {
        float rotation = rotationSpeed * Time.deltaTime * 60;
        transform.Rotate(new Vector3(0, rotation, 0));
        map.UpdateDirectionAngle(rotation);
    }

    void OnCollisionEnter()
    {
        Die();
    }
    public void Die()
    {
        if (dead) return;
        dead = true;
        float[] ADN = brain.GetPerceptronADN();
        int fitness = map.GetNumCleanedPositions();
        manager.RobotDied(ADN, fitness);
        Destroy(gameObject);
    }



    public void UpdateSensorState(string name, bool value)
    {
        sensors[name] = value;
    }
}
