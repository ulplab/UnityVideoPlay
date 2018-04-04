using System.Collections;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System.Threading;
using System.Net.Sockets;
using System;
using System.Text;


public class BCardScript : MonoBehaviour, IVirtualButtonEventHandler {
    public GameObject ulpgo;
    private TextMesh testMesh;
    private TCPTestClient tcpClient;
    private Info info;

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        if (vb.VirtualButtonName == "UlpVB") {
            ulpgo.SetActive(true);
        }
        if (vb.VirtualButtonName == "TurismoVB")
        {
            ulpgo.SetActive(false);
            
        }
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        
    }

    // Use this for initialization
    void Start () {
        info = new Info();
        tcpClient = new TCPTestClient(info);
        tcpClient.ConnectToTcpServer();
        VirtualButtonBehaviour[] vrb = GetComponentsInChildren<VirtualButtonBehaviour>();
        for (int i = 0; i < vrb.Length; i++) {
            vrb[i].RegisterEventHandler(this);
        }
        ulpgo.SetActive(false);
        testMesh = GetComponentInChildren<TextMesh>();//
        testMesh.text = "ULP";
    }
	
	// Update is called once per frame
	void Update () {
        tcpClient.SendMessage(Time.frameCount.ToString());
        //testMesh = GetComponentInChildren<TextMesh>();//
        testMesh.text = info.Mensaje;

    }
}


public class Info {
    public Info()
    {
        Mensaje = "Iniciando...";
    }
    public String Mensaje {get; set;}
}


public class TCPTestClient
{
    #region private members 	
    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    private Info info;
    #endregion

    public TCPTestClient(Info info)
    {
        this.info = info;
    }

    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    public void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (System.Exception e)
        {
            Debug.Log("On client connect exception " + e);
        }
    }
    /// <summary> 	
    /// Runs in background clientReceiveThread; Listens for incomming data. 	
    /// </summary>     
    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("localhost", 11000);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                // Get a stream object for reading 				
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    // Read incomming stream into byte arrary. 					
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        // Convert byte array to string message. 						
                        string serverMessage = Encoding.ASCII.GetString(incommingData);
                        info.Mensaje = serverMessage;
                        Debug.Log("server message received as: " + serverMessage);
                    }
                }
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
    /// <summary> 	
    /// Send message to server using socket connection. 	
    /// </summary> 	
    public void SendMessage(string msg)
    {
        if (socketConnection == null)
        {
            return;
        }
        try
        {
            // Get a stream object for writing. 			
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string clientMessage = msg;
                // Convert string message to byte array.                 
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage);
                // Write byte array to socketConnection stream.                 
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                Debug.Log("Client sent his message - should be received by server");
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}