// Ce script est adaptation du code du cours de l'enseignant Alexandre Ouellet :
// https://cours-alexandre-ouellet.github.io/jeux-3d/abc/gestion-camera/
// Les actions suivantes sont une adaption au contenu du cours : le VolumeCamera pour limiter le déplacement, WASD et les flèches déplacent la cible camèra,
// la molette pour zoomer et déplacer, Angle fixe sans rotation.

using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CibleCamera : MonoBehaviour
{
    [Header("Parametres de deplacement")]
    [SerializeField, Tooltip("Vitesse de déplacement en m/s")]
    private float vitesseDeplacement = 4f;
    [SerializeField] private float vitesseZoom = 0.02f;
    [SerializeField] private Vector2 limitesZoom = new Vector2(5f, 75f);

    [Header("References aux objets de jeu")]
    [SerializeField] private PlayerInput controles;
    [SerializeField] private BoxCollider volumeCamera;

    [SerializeField, Tooltip("La caméra qui suit la cible")]
    private CinemachineCamera cameraGeree;


    // Variables privées pour la gestion du déplacement
    /// <summary>
    /// Le deplacement actuel de la caméra
    /// </summary>
    private Vector2 deplacement;
    private float zoom;

    private void Start()
    {
        // L'action pour se déplacer
        InputAction actionDeplacement = controles.actions.FindAction("Player/DeplacerCamera", true);
        actionDeplacement.performed += CommencerDeplacement;
        actionDeplacement.canceled += TerminerDeplacement;

        InputAction actionZoom = controles.actions.FindAction("Player/ZoomerCamera", true);
        actionZoom.performed += CommencerZoom;
        actionZoom.canceled += TerminerZoom;
    }

    private void Update()
    {
        DeplacerCamera();
        ZoomerCamera();
    }

    private void OnDestroy()
    {
        if (controles == null || controles.actions == null) return;

        InputAction actionDeplacement = controles.actions.FindAction("Player/DeplacerCamera");
        if (actionDeplacement != null)
        {
            actionDeplacement.performed -= CommencerDeplacement;
            actionDeplacement.canceled -= TerminerDeplacement;
        }

        InputAction actionZoom = controles.actions.FindAction("Player/ZoomerCamera");
        if (actionZoom != null)
        {
            actionZoom.performed -= CommencerZoom;
            actionZoom.canceled -= TerminerZoom;
        }
    }

    #region Déplacement
    /// <summary>
    /// Commence le déplacement de la caméra
    /// </summary>
    /// <param name="contexte">Information du callback de l'action</param>
    private void CommencerDeplacement(InputAction.CallbackContext contexte)
    {
        deplacement = vitesseDeplacement * contexte.ReadValue<Vector2>().normalized;
    }

    /// <summary>
    /// Termine le déplacement de la caméra
    /// </summary>
    /// <param name="contexte">Information du callback de l'action</param>
    private void TerminerDeplacement(InputAction.CallbackContext contexte)
    {
        deplacement = Vector2.zero;
    }

    /// <summary>
    /// Gère le déplacement de la caméra en fonction de l'input du joueur et des limites du volume de la caméra
    /// </summary>
    private void DeplacerCamera()
    {
        if (deplacement.sqrMagnitude > 0f)
        {
            // La cible se deplace dans le meme repere que le joueur, meme apres un demi-tour.
            Vector3 prochainDeplacement = controles.transform.right * deplacement.x + controles.transform.forward * deplacement.y;
            prochainDeplacement = Vector3.Scale(prochainDeplacement, new Vector3(1f, 0f, 1f));
            Vector3 prochainePosition = transform.position + prochainDeplacement * Time.deltaTime;

            if (volumeCamera.bounds.Contains(prochainePosition))
                transform.position = prochainePosition;
        }
    }

    /// <summary>
    /// Commencer le zoom de la caméra
    /// </summary>
    /// <param name="contexte">Information du callback de l'action</param>
    private void CommencerZoom(InputAction.CallbackContext contexte)
    {
        zoom = vitesseZoom * contexte.ReadValue<float>();
    }


    /// <summary>
    /// Termine le zoom de la caméra
    /// </summary>
    /// <param name="contexte">Information du callback de l'action</param>
    private void TerminerZoom(InputAction.CallbackContext contexte)
    {
        zoom = 0f;
    }


    /// <summary>
    /// Ajuste la distance de zoom de la caméra Cinemachine en fonction de la position du joueur
    /// tout en respectant les limites minimales et maximales définies.
    /// </summary>
    private void ZoomerCamera()
    {
        CinemachinePositionComposer positionComposer = cameraGeree.GetComponent<CinemachinePositionComposer>();
        Vector3 offsetCamera = positionComposer.TargetOffset;
        float distanceCamera = offsetCamera.magnitude + zoom;

        if (distanceCamera >= limitesZoom.x && distanceCamera <= limitesZoom.y)
            positionComposer.TargetOffset = offsetCamera.normalized * distanceCamera;
    }
}
    #endregion