using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

enum GameState{
    WaitingForConnection,
    Started
}

public class GameManagement : NetworkBehaviour
{
    [SerializeField] private GameState currentGameState;
    [SerializeField] private int maxPlayers;

    // Text for displaying to all users
    [SerializeField] private TextMeshProUGUI gameText;

    private List<GameObject> players = new List<GameObject>();

    private void Start()
    {
        gameText.text = "Waiting for Player " + (players.Count+1);
    }

    public void PlayerConnected(GameObject player) 
    {
        players.Add(player);
        gameText.text = "Waiting for Player " + (players.Count + 1);

        if (players.Count == maxPlayers) 
        {
            StartCoroutine(OnAllPlayersSpawned());
        }
    }

    IEnumerator OnAllPlayersSpawned()
    {
        // All players are in the lobby
        for (int i = 3; i > 0; i--)
        {
            gameText.text = "" + i;
            yield return new WaitForSeconds(1f);
        }

        gameText.text = "Fight!";
        yield return new WaitForSeconds(1f);
        gameText.text = "";

        // Enable players to fight
        foreach (GameObject player in players)
            player.GetComponent<PlayerCommands>().canPlay = true;

        currentGameState = GameState.Started;
    }
}
