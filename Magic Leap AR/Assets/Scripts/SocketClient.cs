using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Client class shows how to implement and use TcpClient in Unity.
/// </summary>
public class SocketClient : MonoBehaviour
{
    #region Public Variables
    [Header("Network")]
    public string ipAddress;
    public int port;
    public float waitingMessagesFrequency = 2;
    #endregion

    #region Private m_Variables
    private TcpClient m_Client;
    private NetworkStream m_NetStream = null;
    private byte[] m_Buffer = new byte[10];
    private int m_BytesReceived = 0;
    private string m_ReceivedMessage = "";
    private IEnumerator m_ListenServerMsgsCoroutine = null;
    #endregion

    #region Delegate Variables
    protected Action OnClientStarted = null;    //Delegate triggered when client start
    protected Action OnClientClosed = null;    //Delegate triggered when client close
    #endregion

    private int xPos;
    List<Vector3> list = new List<Vector3>();
    private int count;
    public Material m;
    public Vector3 position2;
    public Vector3[] positions = new Vector3[100];
    private GameObject[] spheres = new GameObject[100];
    private GameObject s;

    //Start client and stablish connection with server
    public void Start()
    {
        count = 0;
        //Early out
        if (m_Client != null)
        {
            ClientLog("There is already a runing client", Color.red);
            
            return;
        }

        try
        {
            //Create new client
            m_Client = new TcpClient();
            //Set and enable client
            m_Client.Connect(ipAddress, port);
            ClientLog("Client Started", Color.green);
            print("Client Started");
            OnClientStarted?.Invoke();

            //Start Listening Server Messages coroutine
            m_ListenServerMsgsCoroutine = ListenServerMessages();
            StartCoroutine(m_ListenServerMsgsCoroutine);
            print(ipAddress);
            print(port);
        }
        catch (SocketException)
        {
            ClientLog("Socket Exception: Start Server first", Color.red);
            CloseClient();
        }
    }

    #region Communication Client<->Server
    //Coroutine waiting server messages
    private IEnumerator ListenServerMessages()
    {
        //early out if there is nothing connected       
        if (!m_Client.Connected)
            yield break;

        //Stablish Client NetworkStream information
        m_NetStream = m_Client.GetStream();

        //Start Async Reading from Server and manage the response on MessageReceived function
        do
        {
            //ClientLog("Client is listening server msg...", Color.yellow);
            //Start Async Reading from Server and manage the response on MessageReceived function
            m_NetStream.BeginRead(m_Buffer, 0, m_Buffer.Length, MessageReceived, null);

            if (m_BytesReceived > 0)
            {
                OnMessageReceived(m_ReceivedMessage);
                m_BytesReceived = 0;
            }

            yield return new WaitForSeconds(waitingMessagesFrequency);

        } while (m_BytesReceived >= 0 && m_NetStream != null);
        //The communication is over
        //CloseClient();
    }

    private void displayTrace()
    {
        for (int i = 0; i < count; ++i)
        {

            spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spheres[i].transform.position = positions[i];
            spheres[i].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            spheres[i].GetComponent<Renderer>().material = m;
        }
    }

    //What to do with the received message on client
    protected virtual void OnMessageReceived(string receivedMessage)
    {
        ClientLog("Msg: " + "<b>" + receivedMessage + "</b>", Color.green);
        switch (m_ReceivedMessage)
        {
            case "1":
                xPos = int.Parse(m_ReceivedMessage);
                break;
            case "Close":
                CloseClient();
                break;
            default:
                ClientLog("Received message " + receivedMessage + ", has no special behaviuor", Color.red);
                break;
        }
    }

    //Send custom string msg to server
    protected void SendMessageToServer(string sendMsg)
    {
        //early out if there is nothing connected       
        if (!m_Client.Connected)
        {
            ClientLog("Socket Error: Stablish Server connection first", Color.red);
            return;
        }

        //Build message to server
        byte[] msg = Encoding.ASCII.GetBytes(sendMsg); //Encode message as bytes
        //Start Sync Writing
        m_NetStream.Write(msg, 0, msg.Length);
        ClientLog("Msg sended to Server: " + "<b>" + sendMsg + "</b>", Color.blue);
    }

    //AsyncCallback called when "BeginRead" is ended, waiting the message response from server
    private void MessageReceived(IAsyncResult result)
    {
        if (result.IsCompleted && m_Client.Connected)
        {
            //build message received from server
            m_BytesReceived = m_NetStream.EndRead(result);
            m_ReceivedMessage = Encoding.ASCII.GetString(m_Buffer, 0, m_BytesReceived);

            if (m_ReceivedMessage == "1")
            {
                print("Esfera");
                /*
                list.Add(GameObject.FindGameObjectWithTag("Marcador2").transform.position);
                s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                s.transform.position = list[list.Count - 1];
                s.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                s.GetComponent<Renderer>().material = m;*/
            }
        }
    }
    #endregion

    #region Close Client
    //Close client connection
    private void CloseClient()
    {
        ClientLog("Client Closed", Color.red);

        //Reset everything to defaults        
        if (m_Client.Connected)
            m_Client.Close();

        if (m_Client != null)
            m_Client = null;

        OnClientClosed?.Invoke();
    }
    #endregion

    #region ClientLog
    //Custom Client Log - With Text Color
    protected virtual void ClientLog(string msg, Color color)
    {
        print("<b>Client:</b> " + msg);
    }
    //Custom Client Log - Without Text Color
    protected virtual void ClientLog(string msg)
    {
        print("<b>Client:</b> " + msg);
    }
    #endregion

    private void Update()
    {
        if (xPos == 1)
        {
            xPos = 0;
            //positions[count] = GameObject.FindGameObjectWithTag("Sphere2").transform.position;
            //count += 1;
            //displayTrace();
            
            list.Add(GameObject.FindGameObjectWithTag("Marcador2").transform.position);
            s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.position = list[list.Count - 1];
            s.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            s.GetComponent<Renderer>().material = m;
        }
    }
}