using UnityEngine;
using UnityEngine.UI;

public class InspectObject : MonoBehaviour
{
    public float interactionDistance = 1f; // Distancia m�xima para interactuar
    public Transform inspectPosition; // Posici�n donde el objeto ser� inspeccionado
    public Transform player; // Referencia al objeto del jugador
    public FirstPersonMovement movementScript; // Referencia al script de movimiento del jugador
    public FirstPersonLook lookScript; // Referencia al script de mirada del jugador
    public Text inspectText; // Referencia al texto de inspecci�n
    public float rotationSpeed = 300f; // Velocidad de rotaci�n del objeto

    private Camera playerCamera;
    private GameObject currentObject = null;
    private bool isInspecting = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody playerRigidbody;
    private RigidbodyConstraints originalConstraints;

    void Start()
    {
        playerCamera = Camera.main;
        playerRigidbody = player.GetComponent<Rigidbody>();
        originalConstraints = playerRigidbody.constraints;
        inspectText.gameObject.SetActive(false); // Aseg�rate de que el texto est� inicialmente desactivado
    }

    void Update()
    {
        if (isInspecting)
        {
            if (Input.GetKeyDown(KeyCode.F)) // Presiona F para dejar de inspeccionar
            {
                ReleaseObject();
            }
            if (Input.GetMouseButton(0))
            {
                RotateObject();
            }
        }
        else
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                if (hit.collider.gameObject.CompareTag("Inspectable"))
                {
                    inspectText.gameObject.SetActive(true); // Mostrar texto "Inspeccionar"
                    inspectText.text = "(F) Inspeccionar"; // Actualizar el texto
                    if (Input.GetKeyDown(KeyCode.F)) // Presiona F para interactuar
                    {
                        TryPickObject(hit.collider.gameObject);
                    }
                }
                else
                {
                    inspectText.gameObject.SetActive(false); // Ocultar texto si no est� mirando un objeto inspeccionable
                }
            }
            else
            {
                inspectText.gameObject.SetActive(false); // Ocultar texto si no est� mirando ning�n objeto
            }
        }
    }

    void TryPickObject(GameObject obj)
    {
        currentObject = obj;
        originalPosition = currentObject.transform.position;
        originalRotation = currentObject.transform.rotation;
        currentObject.GetComponent<Rigidbody>().isKinematic = true;
        currentObject.transform.position = inspectPosition.position;
        currentObject.transform.rotation = inspectPosition.rotation;
        isInspecting = true;
        movementScript.enabled = false; // Desactivar el movimiento del jugador
        lookScript.enabled = false; // Desactivar la rotaci�n de la c�mara del jugador
        Cursor.lockState = CursorLockMode.None; // Liberar el cursor para inspecci�n
        Cursor.visible = true; // Hacer el cursor visible

        // Desactivar el movimiento y la rotaci�n del jugador
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        inspectText.gameObject.SetActive(false); // Ocultar texto cuando se empieza a inspeccionar
    }

    void ReleaseObject()
    {
        if (currentObject != null)
        {
            currentObject.GetComponent<Rigidbody>().isKinematic = false;
            currentObject.transform.position = originalPosition;
            currentObject.transform.rotation = originalRotation;
            currentObject = null;
            isInspecting = false;
            movementScript.enabled = true; // Reactivar el movimiento del jugador
            lookScript.enabled = true; // Reactivar la rotaci�n de la c�mara del jugador
            Cursor.lockState = CursorLockMode.Locked; // Bloquear el cursor de nuevo
            Cursor.visible = false; // Ocultar el cursor

            // Restaurar las restricciones originales del jugador
            playerRigidbody.constraints = originalConstraints;
        }
    }

    void RotateObject()
    {
        float rotateX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float rotateY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        currentObject.transform.Rotate(playerCamera.transform.up, -rotateX, Space.World);
        currentObject.transform.Rotate(playerCamera.transform.right, rotateY, Space.World);
    }
}
