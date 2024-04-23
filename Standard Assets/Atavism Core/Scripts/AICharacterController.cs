using UnityEngine;
using Atavism;

public class AICharacterController : MonoBehaviour
{
    // Reference to the character game object
    public GameObject character;

    // Reference to the Atavism entity
    public AtavismEntity entity;

    // AI decision-making variables
    private float movementSpeed = 5.0f;
    private float rotationSpeed = 2.0f;

    void Start()
    {
        // Initialize the character's movement and rotation
        character.transform.position = entity.position;
        character.transform.rotation = entity.rotation;
    }

    void Update()
    {
        // Simple AI decision-making: move the character forward
        character.transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);

        // Rotate the character towards a random target (for now)
        character.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}