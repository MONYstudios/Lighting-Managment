using UnityEngine;

public class ForestTrigger : MonoBehaviour
{
    [Header("Settings - References")]
    [SerializeField] GodRayController godRayController;
    [SerializeField] FogController fogController;
    
    [Header("Settings")]
    [SerializeField] float smoothSpeed = 5f;

    void Start()
    {
        if (godRayController == null || fogController == null)
        {
            Debuger.instance.ErrorMessage("ForestTrigger: Controller wasn't found!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fogController.isInForest = true;
            Debuger.instance.DebugMessage("ForestTrigger: Player is in the forest");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fogController.isInForest = false;
            Debuger.instance.DebugMessage("ForestTrigger: Player isn't in the forest");
        }
    }
}
