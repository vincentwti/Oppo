using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ActionInvoker : MonoBehaviour
{
    public bool playOnEnable = false;
    public float delay;
    public UnityEvent callback;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            StartInvoke();    
        }
    }

    public void StartInvoke()
    {
        StartCoroutine(WaitForInvoke());
    }

    private IEnumerator WaitForInvoke()
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
