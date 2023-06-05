using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class Server : MonoBehaviour
{
    public InputField PortInput;

    List<ServerClient> clients;
    List<ServerClient> disconnectList;

    TcpListener server;//받는 서버
    bool serverStarted;//서버가 열렸는가?

    public void ServerCreate()//서버 열기
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            int port = PortInput.text == "" ? 8000 : int.Parse(PortInput.text);
            //IPAddress.Any는 자기 자신의 컴퓨터
            server = new TcpListener(IPAddress.Any, port);//하나의 포트로 서버가 열림
            server.Start();//서버 열림

            StartListening();
            serverStarted = true;
            Chat.instance.ShowMessage($"서버가 {port}에서 시작되었습니다.");
        }catch(Exception e)//예외처리
        {
            Chat.instance.ShowMessage($"Socket Error : {e.Message}");
        }
    }
    void StartListening()//듣기 시작(비동기), 동기면 실행중 멈춤
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);//콜백 함수
    }
    //연결요청시 실행
    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));//ar을 서버에 받아줌
        StartListening();

        //메세지를 연결된 모두에게 보냄
        Broadcast("%NAME", new List<ServerClient>() { clients[clients.Count-1] });
    }
    void OnIncomindData(ServerClient c,string data)
    {
        if (data.Contains("&NAME"))
        {
            c.clientName = data.Split('|')[1];
            Broadcast($"{c.clientName}이 연결되었습니다.", clients);
            return;
        }
        Broadcast($"{c.clientName} : {data}", clients);//모든 클라리언트에게 보냄
    }
    void Broadcast(string data,List<ServerClient> cl)
    {
        foreach(var c in cl)//클라이언트 리스트 순회
        {
            try
            {
                //쓰기 모드 활성화
                StreamWriter writer = new StreamWriter(c.tcp.GetStream());
                writer.WriteLine(data);//모든 사람에게 쓰기
                writer.Flush();//강제 내보냄
            }catch(Exception e)
            {
                Chat.instance.ShowMessage($"쓰기 에러 : {e.Message}를 클라이언트에게 {c.clientName}");
            }
        }
    }
    bool IsConnected(TcpClient c)//연결 여부
    {
        try
        {
            if(c != null && c.Client != null && c.Client.Connected)
            {
                //클라이언트에게 연결되었는지 테스트(1바이트 데이터 보냄)
                if (c.Client.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!serverStarted) return;

        foreach(ServerClient c in clients)//클라이언트 리스트 순회
        {
            //클라이언트가 여전히 연결되었나?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();//소켓 닫음
                disconnectList.Add(c);
                continue;
            }
            else//클라이언트로부터 체크 메세지를 받는다.
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    //스트림 리더를 통해 데이터를 읽음
                    string data = new StreamReader(s, true).ReadLine();
                    if (data != null)
                    {
                        OnIncomindData(c, data);
                    }
                }
            }
        }
        for(int i = 0; i < disconnectList.Count - 1; i++)
        {
            Broadcast($"{disconnectList[i].clientName} 연결이 끊어졌습니다.",clients);

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }
}

public class ServerClient
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)//생성자
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
