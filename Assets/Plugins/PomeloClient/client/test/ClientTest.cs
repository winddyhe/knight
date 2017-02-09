using System;
using Core.WindJson;

namespace Pomelo.DotNetClient.Test
{
    public class ClientTest
    {
        public static PomeloClient pc = null;

        public static void loginTest(string host, int port)
        {
            pc = new PomeloClient();

            pc.NetWorkStateChangedEvent += (state) =>
            {
                Console.WriteLine(state);
            };


            pc.initClient(host, port, () =>
            {
                pc.connect(null, data =>
                {

                    Console.WriteLine("on data back" + data.ToString());
                    JsonNode msg = new JsonClass();
                    msg["uid"] = new JsonData(111);
                    pc.request("gate.gateHandler.queryEntry", msg, OnQuery);
                });
            });
        }

        public static void OnQuery(JsonNode result)
        {
            if (Convert.ToInt32(result["code"]) == 200)
            {
                pc.disconnect();

                string host = result["host"].AsString;
                int port = Convert.ToInt32(result["port"]);
                pc = new PomeloClient();

                pc.NetWorkStateChangedEvent += (state) =>
                {
                    Console.WriteLine(state);
                };

                pc.initClient(host, port, () =>
                {
                    pc.connect(null, (data) =>
                    {
                        //JsonObject userMessage = new JsonObject();
                        Console.WriteLine("on connect to connector!");

                        //Login
                        JsonNode msg = new JsonClass();
                        msg["username"] = new JsonData("test");
                        msg["rid"] = new JsonData("pomelo");

                        pc.request("connector.entryHandler.enter", msg, OnEnter);
                    });
                });
            }
        }

        public static void OnEnter(JsonNode result)
        {
            Console.WriteLine("on login " + result.ToString());
        }

        public static void onDisconnect(JsonNode result)
        {
            Console.WriteLine("on sockect disconnected!");
        }

        public static void Run()
        {
            string host = "192.168.0.156";
            int port = 3014;

            loginTest(host, port);
        }
    }
}