using UnityEngine;

public class LiftableObject : Interactable
{


    public Transform player; // Referencia al jugador
    public Transform carryPosition; // Posici�n donde el objeto ser� llevado
    private bool isBeingCarried = false;

    void Start()
    {
        interactionText = "Agarrar"; // Texto espec�fico para este objeto
    }

    void Update()
    {
        if (isBeingCarried && Input.GetMouseButtonUp(0)) // Soltar el objeto al soltar el bot�n izquierdo del rat�n
        {
            isBeingCarried = false;
            transform.parent = null;
        }

        if (isBeingCarried)
        {
            transform.position = carryPosition.position;
        }
    }

    public override void Interact()
    {
        if (!isBeingCarried)
        {
            isBeingCarried = true;
            transform.parent = carryPosition; // Hacer que el objeto siga la posici�n de carryPosition
        }
    }
}
