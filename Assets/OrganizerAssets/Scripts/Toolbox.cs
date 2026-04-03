using System;
using UnityEngine;

public class Toolbox : MonoBehaviour
{
    [SerializeField] private Vector2 point1;
    [SerializeField] private Vector2 point2;
    [SerializeField] private float slideSpeed;
    private Vector2 targetPos;
    private bool atPoint1;

    private bool isClicked;
    private float timer = 0;
    [SerializeField] private GameObject outline;

    void Start()
    {
        targetPos = point1;
        atPoint1 = true;
        transform.position = point1;
    }

    void Update()
    {
        transform.position = Vector2.Lerp(transform.position, targetPos, Time.deltaTime * slideSpeed);

        if (!isClicked)
        {
            if (timer <= 1)
                timer += Time.deltaTime;
            else
            {
                timer = 0;
                outline.SetActive(!outline.activeSelf);
            }
        }
    }
    
    private void OnMouseDown()
    {
        if (atPoint1)
            targetPos = point2;
        else 
            targetPos = point1;

        atPoint1 = !atPoint1;

        if (!isClicked)
        {
            isClicked = true;
            outline.SetActive(false);
        }
    }
    
}
