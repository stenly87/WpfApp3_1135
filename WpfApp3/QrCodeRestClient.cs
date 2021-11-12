using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp3
{
    public static class QrCodeRestClient
    {
        static string url = "http://localhost:42394/QRCode";

        public static QRCode Post(QRCode code)
        {
            return ExecuteRequest<QRCode, QRCode>("POST", code);
        }

        public static bool Put(QRCode code)
        {
            return ExecuteRequest<bool, QRCode>("PUT", code);
        }

        public static bool Delete(QRCode code)
        {
            return ExecuteRequest<bool, QRCode>("DELETE", code);
        }

        public static IEnumerable<QRCode> GetAllQRCodes()
        {
            return ExecuteRequest<List<QRCode>, string>("GET", null);
        }

        static T ExecuteRequest<T, V>(string type, V v) 
        {
            var request = WebRequest.Create(url);
            DataContractJsonSerializer serializerT = new DataContractJsonSerializer(typeof(T));
            request.Method = type;
            request.ContentType = "application/json";
            if (v != null)
            {
                DataContractJsonSerializer serializerV = new DataContractJsonSerializer(typeof(V));
                using (var stream = request.GetRequestStream())
                    serializerV.WriteObject(stream, v);
            }
            T result = (T)Activator.CreateInstance(typeof(T));
            try
            {
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                    result = (T)serializerT.ReadObject(stream);
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.Message);
            }
            return result;
        }
    }
}
