using System;
using System.Collections;
using UnityEngine;

public class HardnessDial : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float minAngle = -120f;
    public float maxAngle = 120f;
    public int steps = 10;

    [Header("Drag Settings")]
    public float dragSensitivity = 0.5f; // higher = faster movement

    private int currentStep = 1; // hardness value (1-10)
    private float currentAngle;
    
    private bool dragging;
    private Vector3 lastMousePosition;

    void Start()
    {
        SetDialFromStep();
    }
    
    void OnMouseDown() => dragging = true;
    void OnMouseUp()
    { 
        dragging = false;
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (!dragging) return;
        
        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        float horizontalMovement = mouseDelta.x * (dragSensitivity/100);

        if (Mathf.Abs(horizontalMovement) < 0.01f) return;

        if (horizontalMovement > 0f)
            IncreaseStep();
        else
            DecreaseStep();
    }
    
    void IncreaseStep()
    {
        if (currentStep >= steps) return; // HARD STOP at 10

        currentStep++;
        SetDialFromStep();
    }

    void DecreaseStep()
    {
        if (currentStep <= 1) return; // HARD STOP at 1

        currentStep--;
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
        StopAllCoroutines();
        StartCoroutine(RotateToStep(targetStep, speed));
    }

    private IEnumerator RotateToStep(int targetStep, float speed)
    {
        targetStep = Mathf.Clamp(targetStep, 1, steps);

        while (currentStep != targetStep)
        {
            if (currentStep < targetStep)
                currentStep++;
            else
                currentStep--;

            SetDialFromStep();
            
            yield return new WaitForSeconds(1f / speed);
        }
    }
}
