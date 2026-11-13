// Ce script est adaptation du code du cours de l'enseignant Alexandre Ouellet :

using UnityEngine;
using UnityEngine.InputSystem;

public class DeplacementJoueur : MonoBehaviour
{
    [SerializeField, Min(0f), Tooltip("Vitesse de déplacement du joueur en m/s")]
    private float vitesse = 4f;

    [SerializeField, Min(0f), Tooltip("Vitesse de rotation du joueur")] 
    private float vitesseRotation = 120f;


    // Variables privées pour la gestion du déplacement
    /// <summary>
    /// Le deplacement actuel du joueur
    /// </summary>
    private Vector2 deplacement;

    // La rotation actuel du joueur
    private float rotation;

    // Référence au Rigidbody pour gérer le déplacement physique.
    private Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Reliee a Player/Deplacer dans les Unity Events du Player Input.
    public void Deplacer(InputAction.CallbackContext contexte)
    {
        if (!isActiveAndEnabled) return;

        // performed actualise les axes quand une deuxieme touche change.
        if (contexte.started || contexte.performed)
        {
            deplacement = contexte.ReadValue<Vector2>();
        }

        if (contexte.canceled)
        {
            deplacement = Vector2.zero;
        }
    }

    // Reliee a Player/RotationJoueur dans les Unity Events du Player Input.
    public void Tourner(InputAction.CallbackContext contexte)
    {
        if (!isActiveAndEnabled) return;

        if (contexte.started || contexte.performed)
        {
            rotation = contexte.ReadValue<float>();
        }

        if (contexte.canceled)
        {
            rotation = 0f;
        }
    }


    /// <summary>
    /// Réinitialise toutes les valeurs de mouvement pour éviter que le joueur ne continue
    /// de bouger ou de tourner lors de sa réactivation.
    /// </summary>
    private void OnDisable()
    {
        deplacement = Vector2.zero;
        rotation = 0f;
    }


    /// <summary>
    /// Applique continuellement le déplacement et la rotation à partir des valeurs reçues.
    /// </summary>
    private void FixedUpdate()
    {
        Vector3 direction = transform.right * deplacement.x
                          + transform.forward * deplacement.y;

        // Vitesse souhaitée, identique en diagonale.
        Vector3 vitesseSouhaitee = direction.normalized * vitesse;

        // On ajuste seulement la vitesse horizontale mais la vitesse verticale reste gérée par la gravité.
        Vector3 vitesseActuelle = rigidbody.linearVelocity;
        Vector3 changementVitesse = new Vector3(
            vitesseSouhaitee.x - vitesseActuelle.x,
            0f,
            vitesseSouhaitee.z - vitesseActuelle.z
        );

        rigidbody.AddForce(changementVitesse, ForceMode.VelocityChange);

        // Rotation du joueur autour de l'axe Y.
        Quaternion changementRotation = Quaternion.Euler(
            0f,
            rotation * vitesseRotation * Time.fixedDeltaTime,
            0f
        );

        rigidbody.MoveRotation(rigidbody.rotation * changementRotation);
    }
}
