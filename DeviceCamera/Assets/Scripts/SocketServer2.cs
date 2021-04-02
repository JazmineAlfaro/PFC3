using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;


/// <summary>
/// Server class shows how to implement and use TcpListener in Unity.
/// </summary>
public class SocketServer2 : MonoBehaviour
{
    #region Public Variables
    [Header("Network")]
    public string ipAdress;
    public int port;
    public float waitingMessagesFrequency = 2;
    #endregion

    #region  Private m_Variables
    private TcpListener m_Server = null;
    private TcpClient m_Client = null;
    private NetworkStream m_NetStream = null;
    private byte[] m_Buffer = new byte[4000000];
    private int m_BytesReceived = 0;
    private string m_ReceivedMessage = "";
    private IEnumerator m_ListenClientMsgsCoroutine = null;
    #endregion

    #region Delegate Variables
    protected Action OnServerStarted = null;    //Delegate triggered when server start
    protected Action OnServerClosed = null;     //Delegate triggered when server close
    protected Action OnClientConnected = null;  //Delegate triggered when the server stablish connection with client
    #endregion

    public bool sendMessage;
    private int counter;


    //Start server and wait for clients
    public void Start()
    {
        //Set and enable Server 
        IPAddress ip = IPAddress.Parse(ipAdress);
        m_Server = new TcpListener(ip, port);
        m_Server.Start();
        ServerLog("Server Started", Color.green);
        print("Server Started");
        //Wait for async client connection 
        m_Server.BeginAcceptTcpClient(ClientConnected, null);
        OnServerStarted?.Invoke();
        print(ipAdress);
        print(port);
        sendMessage = false;
        counter = 0;

    }

    public void changeState()
    {
        counter += 1;
        sendMessage = true;
        print("Se tomó una foto");
    }



    //Check if any client trys to connect
    private void Update()
    {

        //If some client stablish connection
        if (m_Client != null && m_ListenClientMsgsCoroutine == null)
        {
            m_ListenClientMsgsCoroutine = ListenClientMessages();
            StartCoroutine(m_ListenClientMsgsCoroutine);
        }
        if (m_Client != null && sendMessage)
        {
            //SendMessageToClient("1");
            WebCamTexture _CamTex = Variables.Texture;
            Texture2D snap = new Texture2D(_CamTex.width, _CamTex.height);
            snap.SetPixels(_CamTex.GetPixels());
            print(_CamTex.GetPixels());
            snap.Apply();

            string bytes = System.Convert.ToBase64String(snap.EncodeToPNG());

            SendMessageToClient(bytes);

            print(bytes.Length);

            byte[] b = Convert.FromBase64String(bytes);


            Texture2D tex = new Texture2D(720, 1280);
            tex.LoadImage(b);
            tex.SetPixels(_CamTex.GetPixels());
            tex.Apply();

            System.IO.File.WriteAllBytes(Application.dataPath + "/../Images/imagenunity-" + counter.ToString() + ".png", tex.EncodeToPNG() );
            sendMessage = false;


        }
    }

    //Callback called when "BeginAcceptTcpClient" detects new client connection
    private void ClientConnected(IAsyncResult res)
    {
        //set the client reference
        m_Client = m_Server.EndAcceptTcpClient(res);
        OnClientConnected?.Invoke();
    }

    #region Communication Server<->Client
    //Coroutine waiting client messages while client is connected to the server
    private IEnumerator ListenClientMessages()
    {
        //Restart values in case there are more than one client connections
        m_BytesReceived = 0;
        m_Buffer = new byte[4000000];

        //Stablish Client NetworkStream information
        m_NetStream = m_Client.GetStream();

        //While there is a connection with the client, await for messages
        do
        {
            //ServerLog("Server is listening client msg...", Color.yellow);
            //Start Async Reading from Client and manage the response on MessageReceived function
            m_NetStream.BeginRead(m_Buffer, 0, m_Buffer.Length, MessageReceived, m_NetStream);

            //If there is any msg, do something
            if (m_BytesReceived > 0)
            {
                OnMessageReceived(m_ReceivedMessage);
                m_BytesReceived = 0;
            }

            yield return new WaitForSeconds(waitingMessagesFrequency);

        } while (m_BytesReceived >= 0 && m_NetStream != null);
        //The communication is over
        //CloseClientConnection();
    }

    //What to do with the received message on server
    protected virtual void OnMessageReceived(string receivedMessage)
    {
        //ServerLog("Msg recived on Server: " + "<b>" + receivedMessage + "</b>", Color.green);
        switch (receivedMessage)
        {

            case "Close":
                //In this case we send "Close" to shut down client
                SendMessageToClient("Close");
                //Close client connection
                CloseClientConnection();
                break;
            default:
                ServerLog("Received message " + receivedMessage + ", has no special behaviuor", Color.red);
                break;
        }
    }

    //Send custom string msg to client
    protected void SendMessageToClient(string sendMsg)
    {
        //early out if there is nothing connected       
        if (m_NetStream == null)
        {
            ServerLog("Socket Error: Start at least one client first", Color.red);
            return;
        }

        //Build message to client        
        byte[] msgOut = Encoding.ASCII.GetBytes(sendMsg); //Encode message as bytes
        //Start Sync Writing
        m_NetStream.Write(msgOut, 0, msgOut.Length);
        ServerLog("Msg sended to Client: " + "<b>" + sendMsg + "</b>", Color.blue);
    }

    //AsyncCallback called when "BeginRead" is ended, waiting the message response from client
    private void MessageReceived(IAsyncResult result)
    {
        if (result.IsCompleted && m_Client.Connected)
        {
            //build message received from client
            m_BytesReceived = m_NetStream.EndRead(result);                              //End async reading
            m_ReceivedMessage = Encoding.ASCII.GetString(m_Buffer, 0, m_BytesReceived); //De-encode message as string
        }
    }
    #endregion    

    #region Close Server/ClientConnection
    //Close client connection and disables the server
    protected virtual void CloseServer()
    {
        ServerLog("Server Closed", Color.red);
        //Close client connection
        if (m_Client != null)
        {
            m_NetStream.Close();
            m_NetStream = null;
            m_Client.Close();
            m_Client = null;
        }
        //Close server connection
        if (m_Server != null)
        {
            m_Server.Stop();
            m_Server = null;
        }

        OnServerClosed?.Invoke();
    }

    //Close connection with the client
    protected virtual void CloseClientConnection()
    {
        ServerLog("Close Connection with Client", Color.red);
        //Reset everything to defaults
        StopCoroutine(m_ListenClientMsgsCoroutine);
        m_ListenClientMsgsCoroutine = null;
        m_Client.Close();
        m_Client = null;

        //Waiting to Accept a new Client
        m_Server.BeginAcceptTcpClient(ClientConnected, null);
    }
    #endregion

    #region ServerLog
    //Custom Server Log - With Text Color
    protected virtual void ServerLog(string msg, Color color)
    {
        print("<b>Server:</b> " + msg);
    }
    //Custom Server Log - Without Text Color
    protected virtual void ServerLog(string msg)
    {
        print("<b>Server:</b> " + msg);
    }
    #endregion

}
