using Microsoft.Extensions.DiagnosticAdapter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class HttpRequestDiagnosticsListener : IObserver<DiagnosticListener>, IObserver<KeyValuePair<string, object>>
    {
        private readonly List<IDisposable> _subscriptions;

        public HttpRequestDiagnosticsListener()
        {
            _subscriptions = new List<IDisposable>();
        }

        void IObserver<DiagnosticListener>.OnNext(DiagnosticListener diagnosticListener)
        {
            if (diagnosticListener.Name == "HttpHandlerDiagnosticListener")
            {
                var subscription = diagnosticListener.SubscribeWithAdapter(this, (s)=>true); // No HttpRequestOut events ((
                //var subscription = diagnosticListener.Subscribe(this); // Works OK
                _subscriptions.Add(subscription);
            }
        }

        void IObserver<DiagnosticListener>.OnError(Exception error)
        { }

        void IObserver<DiagnosticListener>.OnCompleted()
        {
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions.Clear();
        }


        void IObserver<KeyValuePair<string, object>>.OnNext(KeyValuePair<string, object> pair)
        {
            Write(pair.Key, pair.Value);
        }

        void IObserver<KeyValuePair<string, object>>.OnError(Exception error)
        { }

        void IObserver<KeyValuePair<string, object>>.OnCompleted()
        { }

        private void Write(string name, object value)
        {
            Console.WriteLine(name);
            Console.WriteLine(value);
            Console.WriteLine();
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Start")]
        public virtual void OnHttpRequestOutStart(System.Net.Http.HttpRequestMessage request)
        {
            Console.WriteLine("OnHttpRequestOutStart");
        }

        [DiagnosticName("System.Net.Http.HttpRequestOut.Stop")]
        public virtual void OnHttpRequestOutStop(System.Net.Http.HttpRequestMessage request, System.Net.Http.HttpResponseMessage response, TaskStatus requestTaskStatus)
        {
            Console.WriteLine("OnHttpRequestOutStop");
        }

        [DiagnosticName("System.Net.Http.Request")]
        public virtual void OnRequest(System.Net.Http.HttpRequestMessage request)
        {
            Console.WriteLine("OnRequest");
        }

        [DiagnosticName("System.Net.Http.Response")]
        public virtual void OnResponse(System.Net.Http.HttpResponseMessage response)
        {
            Console.WriteLine("OnResponse");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start!");

            DiagnosticListener.AllListeners.Subscribe(new HttpRequestDiagnosticsListener());

            var client = new HttpClient();
            var r = client.GetAsync("http://www.google.com").Result;

            Console.WriteLine("Hello World!");
        }
    }
}
