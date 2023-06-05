using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System;

public class Client : MonoBehaviour
{
    public InputField IPInput, PortInput, NickInput;
    string clientName;

    bool socketReady;
    TcpClient socket;
    NetworkStream stream;//데이터 흐름
    StreamWriter writer;
    StreamReader reader;

    public void ConnectedToServer()
    {
        //이미 연결되었다면 함수 무시
        if (socketReady) return;

        //기본 호스트, 포트 번호
        string ip = IPInput.text == "" ? "127.0.0.1" : IPInput.text;
        int port = PortInput.text == "" ? 8000 : int.Parse(PortInput.text);

        //소켓 생성
        try
        {
            socket = new TcpClient(ip, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }catch(Exception e)
        {
            Chat.instance.ShowMessage($"소켓에러 : {e.Message}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(socketReady && stream.DataAvailable)//읽을 수 있다.
        {
            string data = reader.ReadLine();
            if(data != null)
            {
                OnIncomingData(data);
            }
        }
    }
    void OnIncomingData(string data)
    {
        if (data == "%NAME")
        {
            clientName = NickInput.text == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : NickInput.text;
            Send($"&NAME|{clientName}");
            return;
        }
        Chat.instance.ShowMessage(data);//메세지 보내기
    }
    //서버에 보냄
    void Send(string data)
    {
        if (!socketReady) return;

        writer.WriteLine(data);
        writer.Flush();//강제로 내보냄
    }

    //메세지를 작성하고 서버에 보냄
    public void OnSendButton(InputField SendInput)
    {
        if (!Input.GetButtonDown("Submit")) return;
        SendInput.ActivateInputField();//포커스 조절
        if (SendInput.text.Trim() == "") return;//메세지를 보내지 않았을 경우

        string message = SendInput.text;
        SendInput.text = "";
        Send(message);
    }
    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    //소켓 닫기
    void CloseSocket()
    {
        if (!socketReady) return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}
