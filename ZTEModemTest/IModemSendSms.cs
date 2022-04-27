using System.Threading.Tasks;

namespace ZTEModemTest
{
    public interface IModemSendSms
    {
        IModemLogout SendSms(string phoneNumber, string message);
    }
}