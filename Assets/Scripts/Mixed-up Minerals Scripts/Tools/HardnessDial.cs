using System;
using System.Collections;
using UnityEngine;

public class HardnessDial : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float minAngle = -120f;
    public float maxAngle = 120f;
    public int steps = 10;

    [HideInInspector] public int currentStep = 1; // hardness value (1-10)
    private float currentAngle;
    
    public bool coroutineStarted;

    void Start()
    {
        SetDialFromStep();
    }
    
    void SetDialFromStep()
    {
        float t = (currentStep - 1f) / (steps - 1f);
        currentAngle = Mathf.Lerp(minAngle, maxAngle, 1f - t);

        transform.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }
    
    public int GetHardnessValue()
    {
        return currentStep;
    }

    public void SetStepSmooth(int targetStep, float speed)
    {
        StartCoroutine(RotateToStep(targetStep, speed));
    }

    private IEnumerator RotateToStep(int targetStep, float speed)
    {
        coroutineStarted = true;

        targetStep = Mathf.Clamp(targetStep, 1, steps);

        if (currentStep != targetStep)
        {
            if (currentStep < targetStep)
                currentStep++;
            else
                currentStep--;

            SetDialFromStep();
            
        }

        yield return new WaitForSeconds(speed);
        coroutineStarted = false;
    }
}
