using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using System.Collections.Generic;
public class controller : MonoBehaviour
{
    [SerializeField] private InputActionAsset inp;
    private InputAction look;
    private InputAction move;
    void Start()
    {
        trol = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        look = inp.FindAction("Player/Look");
        move = inp.FindAction("Player/Move");
        look.Enable();
        move.Enable();
    }
    private CharacterController trol;
    [SerializeField] private float speed;
    [SerializeField] private float sens;
    [SerializeField] private float gravity;
    [SerializeField] private float jum;
    [SerializeField] private GameObject cam;
    [SerializeField] private GameObject gun;
    [SerializeField] private GameObject gunhead;
    [SerializeField] private int max_bul;
    int bullets = 10;
    float veloc = 0;
    float xrot = 0;
    float coold = 0;
    // Update is called once per frame
    void Update()
    {
        Vector2 delt = look.ReadValue<Vector2>();
        transform.Rotate(Vector3.up * delt.x * sens * Time.deltaTime);
        xrot -= delt.y * sens * Time.deltaTime;
        xrot = Mathf.Clamp(xrot, -90, 90);
        cam.transform.localRotation = Quaternion.Euler(xrot, 0, 0);
        Vector2 wasd = move.ReadValue<Vector2>();
        veloc -= gravity * Time.deltaTime;
        if (Keyboard.current.spaceKey.isPressed && trol.isGrounded)
        {
            veloc = jum;
        }
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if(coold <= 0)
            {
                StartCoroutine(Twirl());

            }
        }
        if (Mouse.current.leftButton.wasPressedThisFrame && coold < 10)
        {
            StartCoroutine(Shoot());
            coold = 0.2f;
        }
        if (Mouse.current.leftButton.isPressed)
        {
            if (coold <= 0)
            {
                coold = 0.1f;
                StartCoroutine(Shoot());
            }
        }
        if(coold > 0)
        {
            coold -= Time.deltaTime;
        }
        trol.Move((wasd.x * transform.right + Vector3.up * veloc * Time.deltaTime + wasd.y * transform.forward) * Time.deltaTime * speed);
    }
    IEnumerator Shoot()
    {
        bullets -= 1;
        if(bullets <= 0)
        {
            bullets = max_bul;
            coold = 100000;
            Quaternion lsr = gun.transform.localRotation;
            Vector3 lsp = gun.transform.localPosition;
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.01f);
                gunhead.transform.Translate(-0.1f, 0, 0);
            }
            for (int i = 0; i < 5; i++)
            {
                gun.transform.Translate(-0.03f, -0.1f, 0);
                gun.transform.Rotate(-6f, 0, 0);
                yield return new WaitForSeconds(0.015f);
            }
            yield return new WaitForSeconds(0.2f);
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.01f);
                gun.transform.Translate(0, 0.01f, 0);
            }
            yield return new WaitForSeconds(0.05f);
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.01f);
                gun.transform.Translate(0, -0.01f, 0);
            }
            yield return new WaitForSeconds(0.05f);
            for (int i = 0; i < 5; i++)
            {
                gun.transform.Translate(0.03f, 0.1f, 0);
                gun.transform.Rotate(6f, 0, 0);
                yield return new WaitForSeconds(0.015f);
            }
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.01f);
                gunhead.transform.Translate(0.1f, 0, 0);
            }
            gun.transform.localRotation = lsr;
            gun.transform.localPosition = lsp;

            yield return new WaitForSeconds(0.2f);
            coold = 0.1f;
        }
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {

        }
        for(int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.015f);
            gunhead.transform.Translate(-0.1f, 0, 0);
            gun.transform.Translate(-0.02f, 0, 0);
        }
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.015f);
            gunhead.transform.Translate(0.1f, 0, 0);
            gun.transform.Translate(0.02f, 0, 0);
        }
    }
    IEnumerator Twirl()
    {
        float wai = -24.5f;
        for(int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(0.02f);
            wai += 1f;
            gun.transform.Rotate(0, 0, wai);
        }
        for (int i = 0; i < 9; i++)
        {
            yield return new WaitForSeconds(0.02f);
            wai -= 1f;
            gun.transform.Rotate(0, 0, wai);
        }
    }
}
