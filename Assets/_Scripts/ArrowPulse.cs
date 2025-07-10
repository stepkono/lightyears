using UnityEngine;
using System.Collections;

public class ArrowPulse : MonoBehaviour
{
    [SerializeField] private float moveAmount = 30f;
    [SerializeField] private float duration = 1f;

    private Vector3 _startPos;

    void Start()
    {
        _startPos = transform.localPosition;
        float direction = gameObject.name == "Up" ? 1f : -1f;
        StartCoroutine(PulseLoop(direction));
    }

    private IEnumerator PulseLoop(float direction)
    {
        WaitForSeconds delay = new WaitForSeconds(3f);

        while (true)
        {
            yield return MoveArrow(_startPos, _startPos + Vector3.up * moveAmount * direction, duration);
            yield return MoveArrow(_startPos + Vector3.up * moveAmount * direction, _startPos, duration);
            yield return delay;
        }
    }

    private IEnumerator MoveArrow(Vector3 from, Vector3 to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            float t = elapsed / time;
            t = t * t * (3f - 2f * t); // smoothstep easing
            transform.localPosition = Vector3.Lerp(from, to, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = to;
    }
}