using UnityEngine;
using Atavism.Api;
using Atavism;

public class PlayerController : MonoBehaviour
{
    private AtavismApi atavismApi;
    private string username;

    private void Start()
    {
        // Initialize Atavism API connection
        atavismApi = new AtavismApi("YOUR_API_KEY", "YOUR_API_SECRET");
    }

    public void InitializePlayer(string username)
    {
        // Set the player's username
        this.username = username;
    }

    public void MovePlayer(float x, float z)
    {
        // Use Unity's physics engine to move the player
        GetComponent<Rigidbody>().AddForce(new Vector3(x, 0, z), ForceMode.VelocityChange);
    }

    public void AttackPlayer(string targetUsername)
    {
        // Use Atavism's API to handle combat logic
        atavismApi.AttackPlayer(username, targetUsername);
    }

    public void TakeDamage(int damage)
    {
        // Use Atavism's API to update the player's health
        atavismApi.UpdatePlayerHealth(username, damage);
    }
}