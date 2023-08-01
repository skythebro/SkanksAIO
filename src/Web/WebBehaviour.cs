using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace SkanksAIO.Web;

class WebBehaviour : MonoBehaviour
{
    public event RequestReceived? OnRequestReceived;

    public static WebBehaviour? Instance { get; private set; }

    private Queue<HttpListenerContext> requestQueue = new();

    public WebBehaviour()
    {
        Instance = this;
    }

    public void QueueRequest(HttpListenerContext context)
    {
        requestQueue.Enqueue(context);
    }

    private void Update()
    {
        if (requestQueue.Count > 0)
        {
            var context = requestQueue.Dequeue();
            OnRequestReceived?.Invoke(context);
        }
    }
}