using UnityEngine;
using System.Collections;
public class lens : MonoBehaviour
{
    public bool started = false;
    public void StartIt()
    {
        started = true;
        StartCoroutine(Enddit());
    }
    IEnumerator Enddit()
    {
        yield return null;
        started = false;
    }
}
