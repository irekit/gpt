using UnityEngine;
using System.Collections;

public class lens : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
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
