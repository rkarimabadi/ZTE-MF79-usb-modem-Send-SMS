using System.Threading.Tasks;

namespace ZTEModemTest
{
    internal interface IModem
    {
        IModemSendSms Login();
    }
}