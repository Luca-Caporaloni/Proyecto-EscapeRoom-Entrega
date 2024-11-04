using UnityEngine;

public class DoorController : MonoBehaviour
{
    public string correctPassword = "123456"; // La contrase�a correcta
    public Animator doorAnimator; // El animator para abrir la puerta
    public GameObject passwordPanel; // El panel de la UI para ingresar la contrase�a

    private bool isUnlocked = false;

    // M�todo para ser llamado cuando se presione el bot�n de la UI
    public void CheckPassword(string inputPassword)
    {
        if (inputPassword == correctPassword)
        {
            UnlockDoor();
        }
        else
        {
            Debug.Log("Contrase�a incorrecta");
        }
    }

    private void UnlockDoor()
    {
        isUnlocked = true;
        doorAnimator.SetTrigger("OpenDoor"); // Asumiendo que tienes una animaci�n llamada "OpenDoor"
        passwordPanel.SetActive(false); // Ocultar el panel de la contrase�a
    }

    // M�todo para interactuar con la puerta
    public void Interact()
    {
        if (!isUnlocked)
        {
            passwordPanel.SetActive(true); // Mostrar el panel de la contrase�a
        }
    }
}
