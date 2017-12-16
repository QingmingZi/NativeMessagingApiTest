using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace NativeMessagingApiTest
{
    class Program
    {
        httpHelper hh = new httpHelper();

        public static void Main(string[] args)
        {
            JObject data;
            while ((data = Read()) != null)
            {
                var processed = ProcessMessage(data);
                System.IO.File.WriteAllLines(@"log.txt", new string[] { "echo:",processed });
                Write(processed);
                if (processed == "exit")
                {
                    return;
                }
            }
        }

        public static string ProcessMessage(JObject data)
        {
            var message = data["text"].Value<string>();
            switch (message)
            {
                case "test":
                    return "testing!";
                case "exit":
                    return "exit";
                default:
                    try
                    {
                        message = httpHelper.Get("https://api.otcgo.cn/testnet/address/" + message, new Dictionary<string, string>());
                        JObject j = JObject.Parse(message);
                        message = JsonConvert.SerializeObject(j["balances"]);
                    }
                    catch
                    { }
 
                    
                    return message;
                    //return "echo: " + message;
            }
        }

        public static JObject Read()
        {
            var stdin = Console.OpenStandardInput();
            var length = 0;

            var lengthBytes = new byte[4];
            stdin.Read(lengthBytes, 0, 4);
            length = BitConverter.ToInt32(lengthBytes, 0);

            var buffer = new char[length];
            using (var reader = new StreamReader(stdin))
            {
                while (reader.Peek() >= 0)
                {
                    reader.Read(buffer, 0, buffer.Length);
                }
            }

            return (JObject)JsonConvert.DeserializeObject<JObject>(new string(buffer));
        }

        public static void Write(JToken data)
        {
            var json = new JObject();

            json["data"] = data;

            var bytes = System.Text.Encoding.UTF8.GetBytes(json.ToString(Formatting.None));

            var stdout = Console.OpenStandardOutput();
            stdout.WriteByte((byte)((bytes.Length >> 0) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 8) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 16) & 0xFF));
            stdout.WriteByte((byte)((bytes.Length >> 24) & 0xFF));
            stdout.Write(bytes, 0, bytes.Length);
            stdout.Flush();
        }
    }
}
