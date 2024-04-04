using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Encryption
{
    internal static class Program
    {
        static void Main()
        {
            Console.WriteLine("Welcome to the encryptor");

            string plainText = "";

            while (true)
            {
                Console.Write("\tEnter text to be encrypted\n\t> ");

                plainText = Console.ReadLine();

                if (IsValidInput(plainText))
                    break;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\tInvalid input\n");
                Console.ResetColor();
            }

            string encryptedText = EncryptString(plainText);
            plainText = null;


            if (!WriteToFile(encryptedText))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\tFailed to write to file\n");
                Console.ResetColor();
                Console.Write("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n\tSuccessfully wrote encrypted text to file");
            Console.ResetColor();

            encryptedText = null;
            encryptedText = ReadFromFile();

            if (encryptedText == string.Empty)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\tFailed to read from file\n");
                Console.ResetColor();
                Console.Write("\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\tSuccessfully read encrypted text from file\n");
            Console.ResetColor();

            Console.WriteLine($"Encrypted string: {encryptedText}");

            string decryptedText = DecryptString(encryptedText);
            encryptedText = null;

            Console.WriteLine($"Decrypted string: {decryptedText}");
            decryptedText = null;

            Console.Write("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static bool IsValidInput(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt))
                return false;

            if (txt.Length > 50)
                return false;

            return true;
        }

        private static string EncryptString(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Secret.EncryptKey;
                aesAlg.IV = Secret.EncryptIv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private static string DecryptString(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Secret.EncryptKey;
                aesAlg.IV = Secret.EncryptIv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        private static bool WriteToFile(string txt)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("../../encrypted.txt"))
                {
                    sw.WriteLine(txt);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string ReadFromFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader("../../encrypted.txt"))
                {
                    return sr.ReadLine();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}
