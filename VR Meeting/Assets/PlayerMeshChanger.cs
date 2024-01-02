using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMeshChanger : MonoBehaviour
{
    [SerializeField] private GameObject newMeshPrefab; // Reference to the new mesh prefab in the project

    private Animator animator;
    private Transform leftEye;
    private Transform rightEye;

    private const string FULL_BODY_LEFT_EYE_BONE_NAME = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/LeftEye";
    private const string FULL_BODY_RIGHT_EYE_BONE_NAME = "Armature/Hips/Spine/Spine1/Spine2/Neck/Head/RightEye";

    public event System.Action OnPlayerLoadComplete;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        leftEye = transform.Find(FULL_BODY_LEFT_EYE_BONE_NAME);
        rightEye = transform.Find(FULL_BODY_RIGHT_EYE_BONE_NAME);
    }

    private void Start()
    {
        // Load and replace the hierarchy with the new mesh, excluding a specific child
        LoadNewMesh();
    }

    private void LoadNewMesh()
    {
        if (newMeshPrefab != null)
        {
            // Instantiate the new mesh prefab
            GameObject newMeshInstance = Instantiate(newMeshPrefab, transform.position, transform.rotation);

            // Optionally, update eye positions if needed
            if (leftEye != null && rightEye != null)
            {
                leftEye.localPosition = newMeshInstance.transform.Find(FULL_BODY_LEFT_EYE_BONE_NAME).localPosition;
                rightEye.localPosition = newMeshInstance.transform.Find(FULL_BODY_RIGHT_EYE_BONE_NAME).localPosition;
            }

            // Get SkinnedMeshRenderer components from the new mesh instance
            SkinnedMeshRenderer[] newMeshRenderers = newMeshInstance.GetComponentsInChildren<SkinnedMeshRenderer>();

            // Iterate through existing SkinnedMeshRenderer components and transform or add them
            foreach (SkinnedMeshRenderer newMeshRenderer in newMeshRenderers)
            {
                // Find the corresponding SkinnedMeshRenderer in the player GameObject with the same name
                SkinnedMeshRenderer playerMeshRenderer = GetSkinnedMeshRendererByName(newMeshRenderer.name);

                if (playerMeshRenderer != null)
                {
                    // Transform the existing SkinnedMeshRenderer
                    playerMeshRenderer.sharedMesh = newMeshRenderer.sharedMesh;
                    playerMeshRenderer.sharedMaterials = newMeshRenderer.sharedMaterials;
                }
                else
                {
                    // If not found, add the SkinnedMeshRenderer to the player GameObject
                    SkinnedMeshRenderer newPlayerMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
                    newPlayerMeshRenderer.sharedMesh = newMeshRenderer.sharedMesh;
                    newPlayerMeshRenderer.sharedMaterials = newMeshRenderer.sharedMaterials;
                }
            }

            // Destroy the temporary new mesh instance
            Destroy(newMeshInstance);

            OnPlayerLoadComplete?.Invoke();
        }
        else
        {
            Debug.LogError("New mesh prefab not assigned!");
        }
    }

    // Helper method to find a SkinnedMeshRenderer by name
    private SkinnedMeshRenderer GetSkinnedMeshRendererByName(string name)
    {
        return GetComponentsInChildren<SkinnedMeshRenderer>().FirstOrDefault(renderer => renderer.name == name);
    }

    public void ChangeMeshPrefab(GameObject NewnewMeshPrefab)
    {
        newMeshPrefab = NewnewMeshPrefab;
    }
}