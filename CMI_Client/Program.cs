using CMIGrpc.Protos;
using Grpc.Net.Client;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMI_Client
{
    class Program
    {

        public static async Task StartClient()
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            var client = new CMIProtoService.CMIProtoServiceClient(channel);
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = IPAddress.Parse("51.68.72.244");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5032);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Don't allow another socket to bind to this port.
                sender.ExclusiveAddressUse = true;

                // The socket will linger for 10 seconds after
                // Socket.Close is called.
                sender.LingerState = new LingerOption(true, 10);

                // Disable the Nagle Algorithm for this tcp socket.
                sender.NoDelay = true;

                // Set the receive buffer size to 8k
                sender.ReceiveBufferSize = 8192;

                // Set the timeout for synchronous receive methods to
                // 1 second (1000 milliseconds.)
                sender.ReceiveTimeout = 5000;


                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    do
                    {
                        if(!sender.Connected)
                        {
                            Console.WriteLine("Is not Connected");
                            sender.Connect(remoteEP);
                        }
                        

                        Console.WriteLine("Socket connected to {0}",
                            sender.RemoteEndPoint.ToString());



                        // Receive the response from the remote device.  
                        //int bytesRec = sender.Receive(bytes);
                        //string dateLS = Encoding.ASCII.GetString(bytes);

                        //string lS_EncryptValue = String.Concat(GetKeyValue(dateLS, "DA"), GetKeyValue(dateLS, "TI"));
                        //Console.WriteLine("LS Date : " + lS_EncryptValue);
                        //  byte[] msgLD = Encoding.ASCII.GetBytes(FIRST_LD(lS_EncryptValue));


                        //Console.WriteLine("Received message = {0}",
                        //   dateLS);






                        await getReponse(sender);
                        //int bytesRec2 = sender.Receive(bytes);
                        //Console.WriteLine("Received message = {0}",
                        //  Encoding.ASCII.GetString(bytes, 0, bytesRec2));






                        //Console.WriteLine("Received message = {0}",
                        // Encoding.ASCII.GetString(bytes, 0, bytesRec));

                        // Release the socket.  
                    } while (true);
                    //sender.Shutdown(SocketShutdown.Both);
                    //sender.Close();
                    //Console.WriteLine("Connection Closed");

                }
                catch (SocketException se)
                {
                    // Console.WriteLine("SocketException : No connection could be made");
                    Console.WriteLine(se.Message);
                    await StartClient();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : ");
                    await StartClient();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                await StartClient();
            }
        }
        
        private static string FIRST_LD(string data)
        {
            Hashtable date = testDate();
            Hashtable EncryptDate = CreyptDate(data);
            string ld_format = string.Concat(ASCII_COD()["STX"], "LD|DA", date["year"],
                date["month"], date["day"],
                "|TI", date["hour"], date["minute"], date["second"],
                "|IFEF|V#0.02|RT4|",
                "CGFidCryptAB;", EncryptDate["IV"], EncryptDate["LS_Date"], "|",
                ASCII_COD()["ETX"]);

            Console.WriteLine($"First LD : {ld_format}");
            return ld_format;
        }
        public static Hashtable testDate()
        {
            Hashtable dateValue = new Hashtable();
            DateTime date = DateTime.Now;

            int year = date.Year;
            dateValue.Add("year", year.ToString().Substring(2, 2));

            // Month gets 1 (January).
            int month = date.Month;
            dateValue.Add("month", month.ToString("00"));

            // Day gets 13.
            int day = date.Day;
            dateValue.Add("day", day.ToString("00"));

            // Hour get.
            int hour = date.Hour;
            dateValue.Add("hour", hour.ToString("00"));

            // Minute get.
            int minute = date.Minute;
            dateValue.Add("minute", minute.ToString("00"));

            // Second get.
            int second = date.Second;
            dateValue.Add("second", second.ToString("00"));

            return dateValue;
        }
        public static string LD(string INF)
        {
            Hashtable date = testDate();

            string ld_format = string.Concat(ASCII_COD()["STX"], "LD|DA", date["year"],
                date["month"], date["day"],
                "|TI", date["hour"], date["minute"], date["second"],
                "|V#1.01|IFEF|", ASCII_COD()["ETX"]);

            return ld_format;
        }
        public static string LA()
        {

            Hashtable date = testDate();
            string lA_format = string.Concat(ASCII_COD()["STX"], "LA|DA", date["year"],
                date["month"], date["day"],
                "|TI", date["hour"], date["minute"], date["second"], '|',
                ASCII_COD()["ETX"]);

            return lA_format;
        }
        public static async Task CheckReceivedMessageAsync(string reponse, string message, Socket sender)
        {
            if (reponse == "LS")
            {
                byte[] msgLR_K = Encoding.ASCII.GetBytes("LR|RI$K|FL$#$DG#$TS#WS$CPHASCT|");
                byte[] msgLR_G = Encoding.ASCII.GetBytes("LR|RI$G|FL$#$+$D$R$TG#ASCTS#TAWS$CPHKC|");
                byte[] msgLR_O = Encoding.ASCII.GetBytes("LR|RI$O|FL$#$2$D$RG#ASCTS#TAWS$CPHKC|");
                byte[] msgLR_P = Encoding.ASCII.GetBytes("LR|RI$P|FL$#$D$R$TG#ASCTS#TAWSPH|");
                byte[] msgLR1 = Encoding.ASCII.GetBytes("LR|RIGI|FLRNG#GNGLGVGGGSSF|");
                byte[] msgLR2 = Encoding.ASCII.GetBytes("LR|RIGC|FLRNG#GNGLGVGGGSRO|");
                byte[] msgLR3 = Encoding.ASCII.GetBytes("LR|RIGO|FLRNG#GSSF|");
                byte[] msgLA1 = Encoding.ASCII.GetBytes(LA());
                byte[] msgLD = Encoding.ASCII.GetBytes(LD("PB"));


                // Send the data through the socket.  

                int bytesSent1 = sender.Send(msgLD);
                Console.WriteLine("LD send");
                int bytesSent2 = sender.Send(msgLR1);
                Console.WriteLine(LD("PB"));
                Console.WriteLine("LR1 send");
                int bytesSent3 = sender.Send(msgLR2);
                Console.WriteLine("LR2 send");
                int bytesSent4 = sender.Send(msgLR3);
                Console.WriteLine("LR3 send");
                int bytesSent_K = sender.Send(msgLR_K);
                Console.WriteLine("K SEND");
                int bytesSent_G = sender.Send(msgLR_G);
                Console.WriteLine("G SEND");
                int bytesSent_O = sender.Send(msgLR_O);
                Console.WriteLine("O SEND");
                int bytesSent_P = sender.Send(msgLR_P);
                Console.WriteLine("P SEND");
                Thread.Sleep(3000);
                int bytesSent5 = sender.Send(msgLA1);
                Console.WriteLine("LA send");

            }
            else if (reponse == "LA")
            {
                

                byte[] msgLA1 = Encoding.ASCII.GetBytes(LA());
                int bytesSent5 = sender.Send(msgLA1);
                Console.WriteLine("### LA Send ###");
                //await getReponse(sender);
            }
            else if (reponse == "GI")
            {

                byte[] msgLA1 = Encoding.ASCII.GetBytes(ASCII_COD()["ACK"].ToString());
                int bytesSent5 = sender.Send(msgLA1);
                Console.WriteLine("### GI Reponse Send ###");
                //await getReponse(sender);
            }
            else if (reponse == "$K")
            {
                string SequenceNumber = GetKeyValue(message, "S#");
                string Reservation_Number = GetKeyValue(message, "G#");
                string Workstation_ID = GetKeyValue(message, "WS");
                string Hotel_ID = GetKeyValue(message, "PH");
                byte[] msgLR_K = Encoding.ASCII.GetBytes("$K|S#" + SequenceNumber + "|G#" + Reservation_Number + "|$#4905000001234|$D1224|$TVA|$C1|WS" + Workstation_ID + "|ASOK|CTCard acceptable|PH" + Hotel_ID + "|");
                int bytesSent5 = sender.Send(msgLR_K);
                string send_message = Encoding.ASCII.GetString(msgLR_K);
                Console.WriteLine("### K Reponse Send ### = {0}", send_message);
                //await getReponse(sender);
            }
            else if (reponse == "$P")
            {
                string SequenceNumber = GetKeyValue(message, "S#");
                string Reservation_Number = GetKeyValue(message, "G#");
                string Workstation_ID = GetKeyValue(message, "WS");
                string AMOUNT = GetKeyValue(message, "TA");

                Console.WriteLine(AMOUNT);

                GRPC_Connection GrpcConnect = new GRPC_Connection(AMOUNT);

                Task<string> downloading = GrpcConnect.Payment();
                string byteCreditCradInfo = await downloading;
                Console.WriteLine("Card Info : " + byteCreditCradInfo);

                byte[] msgLR_G = Encoding.ASCII.GetBytes("$P|S#"+ SequenceNumber + "|G#"+ Reservation_Number + "|ASOK|WS"+ Workstation_ID + "|$JF12345ASD13|$RA123|$#"+ byteCreditCradInfo + "|$D1205|$TVS");
                int bytesSent5 = sender.Send(msgLR_G);
                string send_message = Encoding.ASCII.GetString(msgLR_G);
                Console.WriteLine("### $P Reponse Send ### = {0}", send_message);


                //await getReponse(sender);
            }
            else if (reponse == "$G")
            {
                string SequenceNumber = GetKeyValue(message, "S#");
                string Reservation_Number = GetKeyValue(message, "G#");
                string Workstation_ID = GetKeyValue(message, "WS");
                string Hotel_ID = GetKeyValue(message, "PH");
                string AMOUNT = GetKeyValue(message, "TA");
                if (AMOUNT != null)
                {
                    Console.WriteLine(AMOUNT);

                    GRPC_Connection GrpcConnect = new GRPC_Connection(AMOUNT);

                    Task<string> downloading = GrpcConnect.PREAUTORISATIONAsync();
                    string byteCreditCradInfo = await downloading;
                    Console.WriteLine("Card Info : " + byteCreditCradInfo);

                    byte[] msgLR_G = Encoding.ASCII.GetBytes("$G|S#" + SequenceNumber + "|G#" + Reservation_Number + "|ASOK|$R0729|$#" + byteCreditCradInfo + "|$D1224|$TVA|WS" + Workstation_ID + "|CG376D67171490E3E7|");
                    int bytesSent5 = sender.Send(msgLR_G);
                    string send_message = Encoding.ASCII.GetString(msgLR_G);
                    Console.WriteLine("### $G Reponse Send ### = {0}", send_message);
                }
                else
                {
                    string SequenceNumber2 = GetKeyValue(message, "S#");
                    string Reservation_Number2 = GetKeyValue(message, "G#");
                    string Workstation_ID2 = GetKeyValue(message, "WS");
                    string Hotel_ID2 = GetKeyValue(message, "PH");
                    string Aproval_Code = GetKeyValue(message, "PH");
                    Console.WriteLine("### Avoid PreAuth #### ");
                    GRPC_Connection GrpcConnect = new GRPC_Connection("1000");

                    Task<string> downloading = GrpcConnect.avoidAsync();
                    string byteCreditCradInfo = await downloading;
                    Console.WriteLine("Avoid : " + byteCreditCradInfo);



                    byte[] msgLR_G = Encoding.ASCII.GetBytes("$G|S#" + SequenceNumber2 + "|G#" + Reservation_Number2 + "|ASOK|$R"+ Aproval_Code + "|$#" + byteCreditCradInfo + "|$D1224|$TVA|WS" + Workstation_ID2 + "|CG376D67171490E3E7|");
                    int bytesSent5 = sender.Send(msgLR_G);
                     string send_message = Encoding.ASCII.GetString(msgLR_G);
                     Console.WriteLine("### $G avoid Reponse Send ### = {0}", send_message);
                }
                //await getReponse(sender);
            }
            else if (reponse == "$O")
            {
                string SequenceNumber = GetKeyValue(message, "S#");
                string Reservation_Number = GetKeyValue(message, "G#");
                string Workstation_ID = GetKeyValue(message, "WS");
                string Hotel_ID = GetKeyValue(message, "PH");
                string AMOUNT = GetKeyValue(message, "TA");

                GRPC_Connection GrpcConnect = new GRPC_Connection(AMOUNT);

                Task<string> downloading = GrpcConnect.PREAUTORISATIONConfirmationAsync();
                string byteCreditCradInfo = await downloading;
                Console.WriteLine("Card Info : " + byteCreditCradInfo);


                byte[] msgLR_G = Encoding.ASCII.GetBytes("$O|S#" + SequenceNumber + "|G#" + Reservation_Number + "|ASOK|WS" + Workstation_ID + "|$JF12345ASD12|X1A0000000031010|X212/24|X3Visa Classic|X4'Signature notrequired'|X50815-4711|");
                int bytesSent5 = sender.Send(msgLR_G);
                string send_message = Encoding.ASCII.GetString(msgLR_G);
                Console.WriteLine("### $G Reponse Send ### = {0}", send_message);
                //await getReponse(sender);
            }

        }
        public static Hashtable ASCII_COD()
        {
            Hashtable Delimit = testDate();

            char character = (char)2;
            Delimit.Add("STX", character.ToString());

            character = (char)3;
            Delimit.Add("ETX", character.ToString());

            character = (char)6;
            Delimit.Add("ACK", character.ToString());

            return Delimit;
        }
        public static async Task getReponse(Socket sender)
        {
            byte[] bytes = new byte[1024];
            int bytesRec2 = sender.Receive(bytes);
            Console.WriteLine(bytesRec2.ToString());
            string reponse = Encoding.ASCII.GetString(bytes, 0, bytesRec2);
            Console.WriteLine("Received message = {0}",
            reponse);
            string reponseT = reponse.Substring(1, 2);

            await CheckReceivedMessageAsync(reponseT, reponse, sender);
        }
        private static string GetKeyValue(string message, string key)
        {
            string data = null;
            if (message.IndexOf(key) != -1)
            {
                data = message.Substring(message.IndexOf(key), message.Length - message.IndexOf(key));
                data = data.Substring(data.IndexOf(key), data.IndexOf("|"));
                data = data.Remove(0, 2);
            }

            return data;
        }
        private static Hashtable GetDateValue(string message, string key)
        {
            Hashtable date = new Hashtable();
            string data = GetKeyValue(message, key);
            int year = int.Parse(String.Concat("20", data.Substring(0, 2)));
            int month = int.Parse(data.Substring(2, 2));
            int day = int.Parse(data.Substring(4, 2));
            date.Add("year", year);
            date.Add("month", month);
            date.Add("day", day);
            return date;
        }
        public static Hashtable CreyptDate(string data)
        {
            var key = "8pFFmUuXJht3NkWk3ZWKAQ==";
            // var key = "GVDpVnl6qYlTQXQJZxXdbw==";
            var encryptedString = AesOperation.EncryptString(key, data);
            return encryptedString;

        }
        //public static string Start_End()
        static async Task Main(string[] args)
        {
            await StartClient();
            //await GetCardInfoAsync("1000.00");
            //ait GetCardInfoAsync("2000.00");


           // string DC3 = "A00B0233A00C006100160006111422000C0006099999002A00040009000B00040000003200012A00D0156A00E0045000F00067543210029000310000170012170905103511A00F0095001800328EF119BBB865D7901E7FA7057EDEAB7000190003300001A0012260521111422001B0016xxxxxxxxxxxx0195";
           
        }
        public static async Task GetCardInfoAsync(string AMOUNT)
        {
            GRPC_Connection GrpcConnect = new GRPC_Connection(AMOUNT);

            //Task<string> downloading = GrpcConnect.StartConnection();
            //string bytesLoaded = await downloading;
            //// string bytesLoaded2 = await downloading2;

            //Console.WriteLine(bytesLoaded);
            //  Console.WriteLine(bytesLoaded2);
        }

    }
}
