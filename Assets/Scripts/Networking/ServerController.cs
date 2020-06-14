﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasNetworking
{
    public class ServerController : MonoBehaviour
    {
        public GameObject GamePrefab;
        public UIController UICtrl;
        public CardRepository CardRepo;

        private IPAddress ipAddress;
        private TcpListener listener;
        private List<ServerGame> games;
        private ServerGame currGame = null;

        private void Awake()
        {
            ipAddress = IPAddress.Parse("127.0.0.1");

            games = new List<ServerGame>();
            try
            {
                listener = new TcpListener(ipAddress, NetworkController.port);
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
            }

            //host on startup
            //Don't await, so execution continues instead of hanging waiting for connection
            Host();
        }

        public async void Host()
        {
            Debug.Log($"Hosting on {ipAddress.ToString()}");
            listener.Start();

            while (true)
            {
                if(currGame == null)
                {
                    currGame = Instantiate(GamePrefab).GetComponent<ServerGame>();
                    currGame.Init(UICtrl, CardRepo);
                    games.Add(currGame);
                }
                var client = await listener.AcceptTcpClientAsync();
                if(currGame.AddPlayer(client) >= 2) currGame = null;
            }
        }
    }
}
