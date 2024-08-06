using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace G7_WebApi.Utilities
{
    public static class Helper
    {
        public static string Encrypt(string text) //Metodo con el que se encriptan las passwd
        {
            string secretKey = ConfigurationManager.AppSettings["Encrypt-Key"]; //Extrae una clave para realizar la encriptacion
            byte[] iv = new byte[16]; //genera los arreglos de bytes
            byte[] array;

            using(Aes cifrado = Aes.Create()) //Importa la libreria necesaria para realizar el cifrado
            {
                cifrado.Key = Encoding.UTF8.GetBytes(secretKey); //Le carga la clave para utilizar en el cifrado
                cifrado.IV = iv;

                ICryptoTransform encryptor = cifrado.CreateEncryptor(cifrado.Key, cifrado.IV); //crea el encriptador

                using(MemoryStream memoryStream = new MemoryStream()) //Importa las biliotecas necesarias
                {
                    using(CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) //Lo coloca en modo de escritura
                    {
                        using(StreamWriter writer = new StreamWriter(cryptoStream))
                        {
                            writer.Write(text); //Encripta la passwd
                        }

                        array = memoryStream.ToArray(); //Lo convierte en el array de bytes
                    }
                }
            }
            return Convert.ToBase64String(array); //Lo devuelve como un string
        }

        public static string Decrypt(string text) 
        {
            string secretKey = ConfigurationManager.AppSettings["Encrypt-Key"]; //Extrae la clave para encriptar
            byte[] iv = new byte[16]; //Crea arreglos necesarios
            byte[] array = Convert.FromBase64String(text); //le pasamos el texto que queremos desencriptar

            using (Aes cifrado = Aes.Create()) //Incluye biblioteca necesaria para el cifrado
            {
                cifrado.Key = Encoding.UTF8.GetBytes(secretKey); //Le carga la informacion necesaria
                cifrado.IV = iv;

                ICryptoTransform decryptor = cifrado.CreateDecryptor(cifrado.Key, cifrado.IV); //Crea el desincriptador

                using (MemoryStream memoryStream = new MemoryStream(array)) //Importa el resto de bibliotecas
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)) //Esta vez en modo de lectura
                    {
                        using (StreamReader reader = new StreamReader(cryptoStream))
                        {
                            return reader.ReadToEnd(); //Lee la palabra encriptada y la retorna en lenguaje natural
                        }

                    }
                }
            }
        }

        public static string GenerateTempPassword() //Genera una passwd aleatoria para las temporales
        {
            Random random = new Random(); //Importa la biblioteca de random
            string caracteres = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890qwertyuiopasdfghjklzxcvbnm"; //crea una variable que almacene los caracteres de interes

            char[] temp = new char[8]; //crea un arreglo con la cantidad de caracteres deseada

            for (int i = 0; i < temp.Length; i++) //recorre el tama;o solicitado
            {
                temp[i] = caracteres[random.Next(temp.Length)]; //va agregando los caracteres uno a uno
            }

            return new string(temp); //lo convierte en string y lo retorna
        }

        public static void SendEmail(string Recipient, string Subject, string Message) //Recibe 3 parametros, a quien se le envia, el asunto, mensaje
        {
            string mailSMTP = ConfigurationManager.AppSettings["mailSMTP"]; //extrae el dominio de donde se envia el correo
            string passSMTP = ConfigurationManager.AppSettings["passSMTP"]; //Extrae la clave proporcionada por el api del servidor alojado

            var template = HttpContext.Current.Server.MapPath("~/Layouts/LayoutNewUser.html"); //Cargamos la plantilla html del correo
            var email = File.ReadAllText(template); //Lee la plantilla como si fuera un string
            var emailBody = email.Replace("{Tittle}", Subject).Replace("{Message}",Message); //reemplaza las partes seleccionadas

            MailMessage msg = new MailMessage(); 
            msg.To.Add(new MailAddress(Recipient)); //Agrega a quien va destinado
            msg.From = new MailAddress(mailSMTP); //Agrega quien lo envia
            msg.Subject = Subject; //Agrega el asunto
            msg.Body = emailBody; //Agrega el mensaje  | En este caso el html de la plantilla
            msg.IsBodyHtml = true; //Valida que el mensaje se envie de forma html ^ Gracias a esta funcion se puede personalizar el cuerpo del correo

            SmtpClient client = new SmtpClient(); //Crea el cliente
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(mailSMTP, passSMTP); //Incluye las credenciales
            client.Port = 2525; //Carga el puerto del servidor
            client.Host = "smtp.elasticemail.com"; //Ingresa el url del servidor
            client.DeliveryMethod = SmtpDeliveryMethod.Network; //El metodo mediante el cual se envia el mensaje
            client.EnableSsl = true; //Habilita la seguridad
            client.Send(msg); //Finalmente envia el correo

        }
    }
}