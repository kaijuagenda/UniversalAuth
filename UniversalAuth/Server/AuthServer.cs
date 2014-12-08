﻿using System;
using System.Collections.Generic;
using System.Net;

namespace UniversalAuth.Server
{
    using Data;
    using Network;

    public abstract class AuthServer
    {
        public LengthedSocket Socket { get; private set; }
        public Random Random { get; private set; }

        public List<Client> Clients = new List<Client>();

        protected AuthServer()
        {
            Socket = null;
            Random = new Random();
        }

        ~AuthServer()
        {
            if (Socket != null)
                Socket.Close();
        }

        public void Start(IPAddress bindAddress, Int32 port)
        {
            Socket = new LengthedSocket(LengthedSocket.SizeType.Word);
            Socket.Bind(new IPEndPoint(bindAddress, port));
            Socket.Listen(100);

            Socket.BeginAccept(EndAccept);
        }

        public void Stop()
        {
            Socket.Close();
            Socket = null;
        }

        private void EndAccept(IAsyncResult result)
        {
            Clients.Add(new Client(Socket.EndAccept(result), this));

            Socket.BeginAccept(EndAccept);
        }

        public void Remove(Client client)
        {
            if (Clients.Contains(client))
                Clients.Remove(client);
        }

        public abstract Boolean ValidateServer(Byte serverId);
        public abstract Boolean ValidateLogin(String user, String password, UInt32 subscription, UInt16 cdkey);
        public abstract Boolean GetServerInfos(out List<ServerInfoEx> servers);

        public void GenerateData(out UInt32 oneTimeKey, out UInt32 sessionId1, out UInt32 sessionId2)
        {
            var buff = new Byte[12];
            Random.NextBytes(buff);

            oneTimeKey = BitConverter.ToUInt32(buff, 0);
            sessionId1 = BitConverter.ToUInt32(buff, 4);
            sessionId2 = BitConverter.ToUInt32(buff, 8);
        }
    }
}