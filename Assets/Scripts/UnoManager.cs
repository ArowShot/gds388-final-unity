using System;
using System.Collections;
using System.Collections.Generic;
using SocketIO;
using TMPro;
using UnityEngine;

public class Player
{
    public string name;
    public TMP_Text lobbyPlayerText;
    public TMP_Text gamePlayerText;
    public int numCards;
}

public class UnoManager : MonoBehaviour
{
    private SocketIOComponent socket;
    public string AuthToken;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_InputField matchCodeInput;

    public TMP_Text matchCode;

    public UnoCard tableCard;
    public GameObject lobbyPlayerList;
    public GameObject gamePlayerList;
    public GameObject playerCards;
    public UnoCard cardPrefab;
    public TMP_Text currentPlayer;
    public TMP_Text playerTextPrefab;
    public GameObject errorPrefab;
    
    public Animator gameControl;

    private Dictionary<string, Player> players = new Dictionary<string, Player>();
    
    void Start()
    {
        gameControl = GetComponent<Animator>();
        socket = GetComponent<SocketIOComponent>();
        
        socket.On("open", null);
        socket.On("authenticated", OnAuthenticated);
        socket.On("matchjoin", OnMatchJoin);
        socket.On("playerjoin", OnPlayerJoin);
        socket.On("playerleave", OnPlayerLeave);
        socket.On("matchstart", OnMatchStart);
        socket.On("matchend", OnMatchEnd);
        socket.On("updatehand", OnUpdateHand);
        socket.On("turn", OnTurnChange);
        socket.On("servererror", OnServerError);
        
        socket.Connect();
    }

    private void OnTurnChange(SocketIOEvent e)
    {
        var name = e.data["player"].str;
        currentPlayer.text = name;
    }

    private void OnServerError(SocketIOEvent e)
    {
        var error = Instantiate(errorPrefab, FindObjectOfType<Canvas>().transform);
        error.GetComponentInChildren<TMP_Text>().text = e.data["message"].str;
    }

    private void OnMatchJoin(SocketIOEvent e)
    {
        gameControl.SetTrigger("GameLobby");
        matchCode.text = e.data["code"].str;
    }
    
    private void OnUpdateHand(SocketIOEvent e)
    {
        Debug.Log($"[{e.name}]: {e.data}");
        var isTable = e.data.keys.Contains("table");
        if (isTable)
        {
            Debug.Log("I want to set the card to " + e.data["hand"].str);
            tableCard.SetCard(e.data["hand"].str);
        }
        else
        {
            if (e.data.keys.Contains("hand"))
            {
                foreach (Transform child in playerCards.transform)
                {
                    Destroy(child.gameObject);
                }
                
                for (int i = 0; i < e.data["hand"].list.Count; i++)
                {
                    var unoCard = Instantiate(cardPrefab, playerCards.transform);
                    unoCard.SetCard(e.data["hand"].list[i].str);
                    var index = i;
                    unoCard.playButton.onClick.AddListener(() =>
                    {
                        socket.Emit("playcard", new JSONObject(new Dictionary<string, JSONObject>
                        {
                            {"card", new JSONObject(index)}
                        }));
                    });
                }
            }
            else
            {
                var name = e.data["player"].str;
                if (players.ContainsKey(name))
                {
                    players[name].gamePlayerText.text = name + ": " + (int) e.data["handSize"].n;
                }
            }
        }
    }

    private void OnMatchEnd(SocketIOEvent obj)
    {
        Debug.Log($"{obj.name}: {obj.data}");
    }

    private void OnMatchStart(SocketIOEvent obj)
    {
        gameControl.SetTrigger("Game");
    }

    private void OnPlayerLeave(SocketIOEvent e)
    {
        var name = e.data["name"].str;
        if (players.ContainsKey(name))
        {
            Destroy(players[name].gamePlayerText.gameObject);
            Destroy(players[name].lobbyPlayerText.gameObject);
            players.Remove(name);
        }
    }

    private void OnPlayerJoin(SocketIOEvent e)
    {
        var name = e.data["name"].str;
        if (players.ContainsKey(name))
        {
            players.Remove(name);
        }
        
        var player = new Player();
        player.name = name;
        player.lobbyPlayerText = Instantiate(playerTextPrefab, lobbyPlayerList.transform);
        player.lobbyPlayerText.text = name;
        player.gamePlayerText = Instantiate(playerTextPrefab, gamePlayerList.transform);
        player.gamePlayerText.text = name + ": 0";
        
        players.Add(name, player);
    }

    private void OnAuthenticated(SocketIOEvent e)
    {
        Debug.Log($"{e.name}: {e.data}");
        if (e.data.keys.Contains("token"))
        {
            AuthToken = e.data["token"].ToString();
            gameControl.SetTrigger("Lobby");
        }
    }

    public void CreateMatch()
    {
        socket.Emit("creategame", new JSONObject());
    }

    public void StartMatch()
    {
        socket.Emit("startgame", new JSONObject());
    }
    
    public void DrawCard()
    {
        socket.Emit("drawcard", new JSONObject());
    }

    public void JoinMatch()
    {
        socket.Emit("joingame", new JSONObject(new Dictionary<string, string>
        {
            {"code", matchCodeInput.text}
        }));
    }
    
    public void Login()
    {
        socket.Emit("auth", new JSONObject(new Dictionary<string, string>
        {
            {"username", usernameInput.text},
            {"password", passwordInput.text}
        }));
    }
}
