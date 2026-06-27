using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class custscenecontroller : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController[] conts;
    private Animator anim;
    [SerializeField] private int next_scene_index = 0;
    [SerializeField] private GameObject potject;
    [SerializeField] private bool firs = false;
    [SerializeField] private GameObject firsject;
    IEnumerator Start()
    {
        anim = GetComponent<Animator>();
        for(int i = 0; i < conts.Length; i++)
        {
            potject.SetActive(false);
            if (firs)
            {
                firsject.SetActive(false);
            }
            anim.runtimeAnimatorController = conts[i];
            yield return new WaitForSeconds(conts[i].animationClips[0].length);
            potject.SetActive(true);
            if (firs && i == 0)
            {
                firsject.SetActive(true);
            }
            while (!Keyboard.current.enterKey.wasPressedThisFrame)
            {
                yield return null;
            }
        }
        SceneManager.LoadScene(next_scene_index);
    }
    void Update()
    {
        if (Keyboard.current.shiftKey.isPressed)
        {
            anim.speed = 2;
        }
        else
        {
            anim.speed = 1;
        }
    }
}
