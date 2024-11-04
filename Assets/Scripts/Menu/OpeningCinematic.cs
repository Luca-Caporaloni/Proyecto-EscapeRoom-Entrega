using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OpeningCinematic : MonoBehaviour
{
    public TextMeshProUGUI cinematicText; // Referencia al TextMeshPro
    public GameObject cinematicPanel; // Referencia al Panel negro
    public Image panelImage; // Referencia a la imagen del Panel
    public float typingSpeed = 0.05f; // Velocidad de aparici�n del texto
    public float fadeDuration = 1.5f; // Duraci�n del fade-out
    public FirstPersonMovement movementScript; // Referencia al script de movimiento del jugador
    public FirstPersonLook lookScript; // Referencia al script de mirada del jugador

    public TextMeshProUGUI timerText; // Referencia al TextMeshPro del temporizador
    public float timerDuration = 3600f; // Duraci�n del temporizador en segundos (60 minutos)
    public Animator timerAnimator; // Referencia al Animator del temporizador

    public PauseMenu pauseMenuScript; // Referencia al script de pausa
    public InGameMenu inGameMenuScript; // Referencia al script de pausa

    public Rigidbody playerRigidbody;
    private RigidbodyConstraints originalConstraints;

    public string mainMenuSceneName = "TimeOutScene";
    public string caughtSceneName = "CaughtScene"; // Nombre de la escena de atrapado

    [SerializeField] private AudioSource tippingAudioSource; // Referencia al AudioSource de la puerta
    [SerializeField] private AudioClip tippingSound; // Sonido de apertura de la puerta

    public Animator doctorAnimator; // Referencia al Animator del doctor
    public Animator doorAnimator; // Referencia al Animator de la puerta

    private string story = "Despertaste en una habitaci�n desconocida, con la extra�a sensaci�n de haber estado all� antes. \n" +
                           "De a poco, los recuerdos vuelven: la electricidad, las quemaduras, horas frente a una proyecci�n sin parpadear, solo una parte de las torturas sufridas.\n" +
                           "A�n cegado por las luces, escuchas que alguien se retirar� para almorzar. Tienes ese tiempo para descubrir el horror que te rodea y escapar.\n" +
                           "Encuentra la forma de salir antes de que sea demasiado tarde.";

    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // Liberar el cursor
        Cursor.visible = true; // Hacer el cursor visible
        pauseMenuScript.enabled = false; // Desactivar el script de pausa
        inGameMenuScript.enabled = false;

        originalConstraints = playerRigidbody.constraints;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeAll;

        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        tippingAudioSource.PlayOneShot(tippingSound);

        Cursor.lockState = CursorLockMode.None; // Liberar el cursor
        Cursor.visible = true; // Hacer el cursor visible

        cinematicText.text = "";
        foreach (char letter in story.ToCharArray())
        {
            cinematicText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        yield return new WaitForSeconds(2f); // Retraso antes de mostrar el temporizador
        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        timerText.gameObject.SetActive(true); // Mostrar el temporizador
        StartCoroutine(FadeOutPanelAndText()); // Iniciar el fade-out del panel y el texto

        float currentTime = timerDuration;
        bool doctorAnimationStarted = false;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Inicia la animaci�n del doctor cuando queden 5 minutos
            if (currentTime <= 20 && !doctorAnimationStarted)
            {
                doctorAnimationStarted = true;
                StartDoctorAnimation();
            }

            yield return null;
        }

        // Reproduce la animaci�n del temporizador al final del conteo
        timerAnimator.SetBool("Timer", true);

        // Espera a que la animaci�n termine
        yield return new WaitForSeconds(fadeDuration);

        // Cambiar a la escena de que te atraparon
        SceneManager.LoadScene(caughtSceneName, LoadSceneMode.Single);
    }

    void StartDoctorAnimation()
    {
        // Activa la animaci�n del doctor caminando
        if (doctorAnimator != null)
        {
            doctorAnimator.SetBool("doctorwalk", true);
        }
    }

    // Esta funci�n ser� llamada al final de la animaci�n de caminar del doctor
    public void OnDoctorArrived()
    {
        // Activa la animaci�n de apertura de la puerta
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("opendoor", true);
        }
    }

    IEnumerator FadeOutPanelAndText()
    {
        float elapsedTime = 0f;
        Color panelColor = panelImage.color;
        Color textColor = cinematicText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            panelImage.color = new Color(panelColor.r, panelColor.g, panelColor.b, alpha);
            cinematicText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }

        cinematicPanel.SetActive(false); // Ocultar el panel de la cinem�tica
        playerRigidbody.constraints = originalConstraints;

        timerAnimator.SetBool("Timer", true);

        pauseMenuScript.enabled = true; // Reactivar el script de pausa
        inGameMenuScript.enabled = true;
        Cursor.lockState = CursorLockMode.Locked; // Bloquear el cursor
        Cursor.visible = false;
    }
}
