using System;
using System.IO.Ports;

namespace AOKT_Laba4CSharp
{
    public class Port
    {
        public string Name = string.Empty;
        public bool Continue;
        public SerialPort MyPort = new SerialPort(), PartnerPort = new SerialPort();

        public void Read(object sender, SerialDataReceivedEventArgs e)
        {
            while (Continue)
            {
                try
                {
                    var message = MyPort.ReadLine();
                    Console.Beep();
                    Console.CursorLeft = 0;
                    Console.Write(message + string.Format("\n<{0}>: ", Name));
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void Send(string message)
        {
            PartnerPort.WriteLine(message);
        }

        public void Initialize()
        {
            MyPort.PortName = SetPortName(MyPort.PortName);
            MyPort.BaudRate = SetPortBaudRate(MyPort.BaudRate);
            MyPort.Parity = SetPortParity(MyPort.Parity);
            MyPort.DataBits = SetPortDataBits(MyPort.DataBits);
            MyPort.StopBits = SetPortStopBits(MyPort.StopBits);

            MyPort.DataReceived += Read;
            MyPort.ErrorReceived += Error;

            MyPort.ReadTimeout = 500;
            MyPort.WriteTimeout = 500;

            MyPort.Open();
            Continue = true;

            Console.Clear();

            while (Equals(Name, string.Empty))
            {
                Console.Write("Enter your nickname: ");
                Name = Console.ReadLine();
            }

            InitializePartner();
        }

        public void InitializePartner()
        {
            Console.WriteLine("\nChoose Partner Port!");
            PartnerPort.PortName = SetPortName(PartnerPort.PortName);
            if (Equals(PartnerPort.PortName, MyPort.PortName)) { PartnerPort = MyPort; return; }
            PartnerPort.Open();
        }

        private static void Error(object sender, SerialErrorReceivedEventArgs e)
        {
            Console.WriteLine("Error!");
        }

        public string SetPortName(string defaultPortName)
        {
            Console.WriteLine("Available Ports:");
            foreach (var s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
            var portName = Console.ReadLine();

            // ReSharper disable once PossibleNullReferenceException
            if (portName == "") return defaultPortName;
            // ReSharper disable once PossibleNullReferenceException
            if (portName.ToLower().StartsWith("com")) return portName.ToUpper();
            byte i;
            if (byte.TryParse(portName, out i)) return "COM" + portName;
            return defaultPortName;
        }

        public int SetPortBaudRate(int defaultPortBaudRate)
        {
            Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
            var baudRate = Console.ReadLine();

            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return int.Parse(baudRate);
        }

        public Parity SetPortParity(Parity defaultPortParity)
        {
            Console.WriteLine("Available Parity options:");
            foreach (var s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
            var parity = Console.ReadLine();

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return (Parity)Enum.Parse(typeof(Parity), parity, true);
        }

        public int SetPortDataBits(int defaultPortDataBits)
        {
            Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
            var dataBits = Console.ReadLine();

            if (dataBits == "")
            {
                dataBits = defaultPortDataBits.ToString();
            }

            // ReSharper disable once PossibleNullReferenceException
            return int.Parse(dataBits.ToUpperInvariant());
        }

        public StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            Console.WriteLine("Available StopBits options:");
            foreach (var s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter StopBits value (None is not supported and \n" +
                          "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
            var stopBits = Console.ReadLine();

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
        }

        public void Close()
        {
            Continue = false;
            PartnerPort.WriteLine(string.Format("<{0}>: Close Conection.", Name));
            MyPort.Close();
            PartnerPort.Close();
        }
    }

    class Program
    {
        public static void Main()
        {
            var myPort = new Port();

            myPort.Initialize();
            Console.Clear();
            Console.Write("Enter \"bye\" to exit" + string.Format("\n<{0}>: ", myPort.Name));
            while (myPort.Continue)
            {
                var message = Console.ReadLine();
                // ReSharper disable once PossibleNullReferenceException
                if (Equals(message.ToLower(), "bye"))
                {
                    myPort.Close();
                    return;
                }
                Console.Write("<{0}>: ", myPort.Name);
                myPort.Send(string.Format("<{0}>: {1}", myPort.Name, message));
            }
        }
    }
}
