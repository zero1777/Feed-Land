using UnityEngine;
using UnityEngine.InputSystem;

public class PlayersManager : MonoBehaviour
{
    // public int numberOfPlayers;
    public GameObject[] playerPrefabs;
    public string[] playerNames;
    public string[] playerControlSchemes;
    public Vector3[] playerInitialPositions;
    public Vector3[] playerInitialRotations;

    void Start()
    {
    }

    void Update()
    {
    }

    public void GeneratePlayer(int playerNum, int playerAvatar)
    {
        print("Generate player:" + playerNum + ", player avatar:" + playerAvatar);
        PlayerInput player = PlayerInput.Instantiate(
            playerPrefabs[playerAvatar],
            controlScheme: playerControlSchemes[playerNum],
            pairWithDevice: Keyboard.current
        );

        player.name = playerNames[playerNum];
        player.transform.position = playerInitialPositions[playerNum];
        player.transform.Rotate(playerInitialRotations[playerNum], Space.World);
    }
}
