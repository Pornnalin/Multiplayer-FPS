using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint;
    public float mouseSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;

    public float moveSpeed = 5f, runSpeed = 10f;
    private Vector3 moveDir;
    private Vector3 movement;
    private float activeMoveSpeed;

    public CharacterController charCon;

    private Camera cam;
    public float jumpFore = 12f, gravityMod = 2.5f;

    public Transform groundCheckPoint;
    public bool isGround;
    public LayerMask groundLayers;
    [Space]
    public GameObject bulletImpact;
    public float timeBetweenShot = .1f;
    public float shorCounter;

    public float maxHeat = 10f, heatPerShot = 1f, coolRate = 4f, overheatCoolRate = 5f;
    [SerializeField] private float heatCounter;
    [SerializeField] private bool overHeated;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        verticalRotStore += mouseInput.y;
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);//กำหนดให้ไม่เกิน -60f 60f

        if (invertLook)
        {
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }
        else
        {
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }

        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //activeMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = moveSpeed;
        }

        float yVel = movement.y;
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
        movement.y = yVel;

        if (charCon.isGrounded)
        {
            movement.y = 0f;
        }
        isGround = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayers);

        if (Input.GetButtonDown("Jump") && isGround)
        {
            movement.y = jumpFore;
        }
        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;
        charCon.Move(movement * Time.deltaTime);

        if (!overHeated)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
            if (Input.GetMouseButton(0))
            {
                shorCounter -= Time.deltaTime;

                if (shorCounter <= 0)
                {
                    Shoot();
                }

            }
            heatCounter -= coolRate * Time.deltaTime;

        }
        else
        {
            heatCounter -= overheatCoolRate * Time.deltaTime;
            if (heatCounter <= 0)
            {
                // heatCounter = 0;
                overHeated = false;
            }
        }
        if (heatCounter < 0)
        {
            heatCounter = 0;
        }


        CursorUpdate();
    }

    private static void CursorUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None && Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cursor.lockState = CursorLockMode.Locked;

        }
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("We hit" + hit.collider);
            GameObject bulletImapactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(bulletImapactObject, 10f);
            //hit.normal: เป็นเวกเตอร์ที่ตั้งฉากกับพื้นผิวที่ชน นี่มักถูกใช้เพื่อหาทิศทางของการชน
        }
        shorCounter = timeBetweenShot;
        heatCounter += heatPerShot;
        if (heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;
            overHeated = true;
        }

    }
    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }
}
