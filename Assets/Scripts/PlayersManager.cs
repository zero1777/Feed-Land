using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour
{
    public int numberOfPlayers;
    public GameObject[] playerPrefabs;
    public string[] playerNames;
    public string[] playerControlSchemes;
    public Vector3[] playerInitialPositions;
    public Vector3[] playerInitialRotations;

    void Start()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            PlayerInput player = PlayerInput.Instantiate(
                playerPrefabs[i],
                controlScheme: playerControlSchemes[i],
                pairWithDevice: Keyboard.current
            );

            player.name = playerNames[i];
            player.transform.position = playerInitialPositions[i];
            player.transform.Rotate(playerInitialRotations[i], Space.World);
        }
    }

    void Update()
    {
    }
}
