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
                    Console.WriteLine("Uso: CertsClient tipos patente chasis usuario password [ssl]");
                    Console.WriteLine("Tipos: MERCOSUR,LOCAL");
                    Console.WriteLine("ssl: para evitar ell chequedo de certificado");
                    Console.WriteLine("Ejemplo: CertsClient MERCO NTR686 \"\" mercantil marsh");
                    return;
                }
                /* esta parte es hasta que MARSH actualice su certificado */
                if (args.Length > 5)
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, error) =>
                   {
                       return true;
                   };
                }
                var tipos = args[0];
                var patente = args[1];
                var chasis = args[2];
                var userName = args[3];
                var password = args[4];
                var client = new CertServiceClient();
                client.ClientCredentials.UserName.UserName = userName;
                client.ClientCredentials.UserName.Password = password;
                
                Certificado[] certificados = client.GetCertificado(tipos.Split(','), patente, chasis);
                foreach(Certificado certificado in certificados)
                {
                    var writer = new BinaryWriter(new FileStream(Path.Combine(Directory.GetCurrentDirectory(), $"{certificado.tipo}.pdf"), FileMode.Create));
                    writer.Write(certificado.archivo);
                    writer.Close();
                    Console.WriteLine($"Todo bien. Archivo en {Path.Combine(Directory.GetCurrentDirectory(), $"{certificado.tipo}.pdf")}");
                }                
                client.Close();
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"Todo mal. {e.Message}");
            }
            finally
            {

            }
            Console.WriteLine("<ENTER> para salir");
            Console.ReadLine();
        }
    }
}
