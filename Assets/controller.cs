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
    float veloc = 0;
    float xrot = 0;
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
            StartCoroutine(Twirl());
        }
        trol.Move((wasd.x * transform.right + Vector3.up * veloc * Time.deltaTime + wasd.y * transform.forward) * Time.deltaTime * speed);
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
