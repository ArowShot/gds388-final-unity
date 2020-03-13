using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;

[System.Serializable]

public class UnoManager : MonoBehaviour
{
    private SocketIOComponent socket;
    public string AuthToken;
    
    
    void Start()
    {
        socket = GetComponent<SocketIOComponent>();
        socket.url = "ws://localhost";
        socket.autoConnect = true;
        
        socket.On("open", null);
        socket.On("authenticated", OnAuthenticated);
        socket.On("playerjoin", OnPlayerJoin);
        socket.On("playerleave", OnPlayerLeave);
        socket.On("matchstart", OnMatchStart);
        socket.On("matchend", OnMatchEnd);
        socket.On("updatehand", OnUpdateHand);
        socket.On("playcard", OnPlayCard);
        
        socket.Connect();
    }

    private void OnPlayCard(SocketIOEvent obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnUpdateHand(SocketIOEvent obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnMatchEnd(SocketIOEvent obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnMatchStart(SocketIOEvent obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnPlayerLeave(SocketIOEvent obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnPlayerJoin(SocketIOEvent obj)
    {
        throw new System.NotImplementedException();
    }

    private void OnAuthenticated(SocketIOEvent e)
    {
        if (bool.Parse(e.data["success"].ToString()))
        {
            AuthToken = e.data["token"].ToString();
        }
    }

    void Authenticate(string username, string password)
    {
        socket.Emit("auth", new JSONObject(new Dictionary<string, string>
        {
            {"username", username},
            {"password", password}
        }));
    }
}
