using UnityEngine;
using Atavism;

public class AICharacterBrain : MonoBehaviour
{
    // Reference to the character game object
    public GameObject character;

    // Reference to the Atavism entity
    public AtavismEntity entity;

    // AI decision-making variables
    private float movementSpeed = 5.0f;
    private float attackRange = 10.0f;

    void Start()
    {
        // Initialize the character's movement and rotation
        character.transform.position = entity.position;
        character.transform.rotation = entity.rotation;
    }

    void Update()
    {
        // Simple AI decision-making: move the character towards a random target
        character.transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);

        // Check for nearby enemies and attack if within range
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider enemy in enemies)
        {
            // Attack the enemy if it's within range
            if (enemy.gameObject.CompareTag("Enemy"))
            {
                // TO DO: implement attack logic
            }
        }
    }
}