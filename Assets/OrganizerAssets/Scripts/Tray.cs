using System.Collections;
using UnityEngine;

public class Tray : MonoBehaviour
{
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 endPos;
    [SerializeField] private float speed;
    private Coroutine moveRoutine;
    [HideInInspector] public bool isMoving;

    public void StartSubmit()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(SubmitSolution());
    }

    public void StartReset()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(ResetPosition());
    }
    
    public IEnumerator SubmitSolution()
    {
        isMoving = true;
        
        while (Vector2.Distance(transform.position, endPos) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
    }

    public IEnumerator ResetPosition()
    {
        isMoving = true;
        while (Vector2.Distance(transform.position, startPos) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = startPos;
        isMoving = false;
    }
}
