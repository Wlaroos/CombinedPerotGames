using System.Collections;
using UnityEngine;

public class BackAndForthAnim : MonoBehaviour
{
    private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos = Vector3.zero;

    [Header("Animation")]
    [SerializeField] private float _duration = 1.5f;
    [SerializeField] private bool _bounce = false;
    [SerializeField] private bool _useAnimCurve = false;
    [SerializeField] private AnimationCurve _animCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private void Start()
    {
        _startPos = transform.localPosition;
        StartCoroutine(BackAndForth());
    }

    private IEnumerator BackAndForth()
    {
        while (true) // Loop indefinitely
        {
            float elapsedTime = 0f;

            // Move to target
            while (elapsedTime < _duration)
            {
                float t = Mathf.Clamp01(elapsedTime / _duration);
                if (_useAnimCurve && _animCurve != null)
                {
                    t = _animCurve.Evaluate(t);
                }

                transform.localPosition = Vector3.Lerp(_startPos, _endPos, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure exact final position
            transform.localPosition = _endPos;

            // Swap start and end positions to go back and forth
            if(_bounce)
            {
                Vector3 temp = _startPos;
                _startPos = _endPos;
                _endPos = temp;
            }
        }
    }
}
