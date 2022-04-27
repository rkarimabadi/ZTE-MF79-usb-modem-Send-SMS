using System;

namespace ZTEModemTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IModem modem = new Modem();
            modem.Login()
                .SendSms("<phonenumber>", "<message>")
                .Logout();
            Console.ReadLine();

        }
    }
}
