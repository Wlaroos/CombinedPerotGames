using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class TapIcon : MonoBehaviour
{
    [SerializeField] private Sprite[] _tapIcons; // Array of tap icons to choose from
    private SpriteRenderer _sr;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(AnimateTapIcon());
        StartCoroutine(StartFadeOut());
    }

    private IEnumerator AnimateTapIcon()
    {
        int index = 0;
        while (true)
        {
            if (_sr != null)
            _sr.sprite = _tapIcons[index];
            
            index = (index + 1) % _tapIcons.Length;
            yield return new WaitForSeconds(0.33f);
        }
    }

    private IEnumerator StartFadeOut()
    {
        yield return new WaitForSeconds(5);

        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            if (_sr != null)
            {
                Color c = _sr.color;
                c.a = alpha;
                _sr.color = c;
            }
            yield return null;
        }
    }
}
