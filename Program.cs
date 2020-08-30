using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Services3.Security.Tokens;
using CertsClient.CertServer;
using System.IO;
using System.Net;

namespace CertsClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length < 5)
                {
                    Console.WriteLine("Uso: CertsClient tipo patente chasis usuario password [ssl]");
                    Console.WriteLine("Tipo: MERCO o LOCAL");
                    Console.WriteLine("ssl: para evitar ell chequedo de certificado");
                    Console.WriteLine("Ejemplo: CertsClient MERCO NTR686 \"\" mercantil marsh");
                    return;
                }
                if (args.Length > 5)
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, error) =>
                   {
                       return true;
                   };
                }
                var tipo = args[0];
                var patente = args[1];
                var chasis = args[2];
                var userName = args[3];
                var password = args[4];
                var client = new CertServiceClient();
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = password;
                var writer = new BinaryWriter(new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "certificado.pdf"), FileMode.Create));
                if (tipo.Equals("MERCO"))
                {
                    writer.Write(client.GetCertificadoMercosur(patente, chasis));
                }
                else
                {
                    writer.Write(client.GetCertificadoLocal(patente, chasis));
                }
                writer.Close();
                client.Close();
                Console.WriteLine($"Todo bien. Archivo en {Path.Combine(Directory.GetCurrentDirectory(), "certificado.pdf")}  <ENTER> para salir");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Todo mal. {e.Message} <ENTER> para salir");
            }
            finally
            {

            }
            Console.ReadLine();
        }
    }
}
