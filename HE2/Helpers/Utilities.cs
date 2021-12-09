using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Research.SEAL;

namespace HE2.ClientApp.Helpers
{
    public static class Utilities
    {
        public static string ULongToString(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public static string Serialize<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        public static string CiphertextToBase64String(Ciphertext ciphertext)
        {
            using (var ms = new MemoryStream())
            {
                ciphertext.Save(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string DoubleToBase64String(double value)
        {
            using (var ms = new MemoryStream(ASCIIEncoding.Default.GetBytes(value.ToString())))
            {
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static int PlaintextToInt(Plaintext pt)
        {
            
     
            var val = $"0x{pt}";
            Console.WriteLine(val);
            
            var result = Convert.ToInt32(pt.ToString(), 16);
            return result;
        }

        public static Ciphertext BuildCiphertextFromBase64String(string base64, SEALContext context)
        {
            var payload = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(payload))
            {
                var ciphertext = new Ciphertext();
                ciphertext.Load(context, ms);

                return ciphertext;
            }
        }

        public static Ciphertext CreateCiphertextFromInt(int val, Encryptor encryptor)
        {
            ulong value = Convert.ToUInt64(val);
            var plaintext = new Plaintext(ULongToString(value));
            var ciphertext = new Ciphertext();
            encryptor.Encrypt(plaintext, ciphertext);
            return ciphertext;
        }

        public static PublicKey BuildPublicKeyFromBase64String(string base64, SEALContext context)
        {
            var payload = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(payload))
            {
                var publicKey = new PublicKey();
                publicKey.Load(context, ms);

                return publicKey;
            }
        }

        public static SecretKey BuildSecretKeyFromBase64String(string base64, SEALContext context)
        {
            var payload = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(payload))
            {
                var secretKey = new SecretKey();
                secretKey.Load(context, ms);

                return secretKey;
            }
        }

        public static string SecretKeyToBase64String(SecretKey secretKey)
        {
            using (var ms = new MemoryStream())
            {
                secretKey.Save(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string PublicKeyToBase64String(PublicKey publicKey)
        {
            using (var ms = new MemoryStream())
            {
                publicKey.Save(ms);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static SEALContext GetContext(SchemeType schemetype  = SchemeType.BFV)
        {
            ulong mod = 8192;
            var encryptionParameters = new EncryptionParameters(schemetype);
            if (schemetype == SchemeType.BFV)
            {
                encryptionParameters.PolyModulusDegree = mod;
                encryptionParameters.CoeffModulus = CoeffModulus.BFVDefault(mod);
                encryptionParameters.PlainModulus = new Modulus(758824777);
            } else if (schemetype == SchemeType.CKKS)
            {
                encryptionParameters.PolyModulusDegree = mod;
                encryptionParameters.CoeffModulus = CoeffModulus.Create(
                    encryptionParameters.PolyModulusDegree, new int[]{ 40, 40, 40, 40, 40 });
            }
        
            Debug.WriteLine("[COMMON]: Successfully created context");

            Console.WriteLine("Set encryption parameters and print");

            var context = new SEALContext(encryptionParameters);
            PrintParameters(context);
            return context;
        }

        public static void PrintParameters(SEALContext context)
        {
            // Verify parameters
            if (null == context)
            {
                throw new ArgumentNullException("context is not set");
            }

            SEALContext.ContextData contextData = context.KeyContextData;

            /*
            Which scheme are we using?
            */
            string schemeName = null;
            switch (contextData.Parms.Scheme)
            {
                case SchemeType.BFV:
                    schemeName = "BFV";
                    break;
                case SchemeType.CKKS:
                    schemeName = "CKKS";
                    break;
                default:
                    throw new ArgumentException("unsupported scheme");
            }

            Console.WriteLine("/");
            Console.WriteLine("| Encryption parameters:");
            Console.WriteLine($"|   Scheme: {schemeName}");
            Console.WriteLine("|   PolyModulusDegree: {0}",
                contextData.Parms.PolyModulusDegree);

            /*
            Print the size of the true (product) coefficient modulus.
            */
            Console.Write("|   CoeffModulus size: {0} (",
                contextData.TotalCoeffModulusBitCount);
            List<Modulus> coeffModulus =
                (List<Modulus>)contextData.Parms.CoeffModulus;
            for (int i = 0; i < coeffModulus.Count - 1; i++)
            {
                Console.Write($"{coeffModulus[i].BitCount} + ");
            }

            Console.WriteLine($"{coeffModulus.Last().BitCount}) bits");

            /*
            For the BFV scheme print the PlainModulus parameter.
            */
            if (contextData.Parms.Scheme == SchemeType.BFV)
            {
                Console.WriteLine("|   PlainModulus: {0}",
                    contextData.Parms.PlainModulus.Value);
            }

            Console.WriteLine("\\");
        }
        
        public static void PrintVector<T>(
            IEnumerable<T> vec, int printSize = 4, int prec = 3)
        {
            string numFormat = string.Format("{{0:N{0}}}", prec);
            T[] veca = vec.ToArray();
            int slotCount = veca.Length;
            Console.WriteLine();
            if (slotCount <= 2 * printSize)
            {
                Console.Write("    [");
                for (int i = 0; i < slotCount; i++)
                {
                    Console.Write(" " + string.Format(numFormat, veca[i]));
                    if (i != (slotCount - 1))
                        Console.Write(",");
                    else
                        Console.Write(" ]");
                }
                Console.WriteLine();
            }
            else
            {
                Console.Write("    [");
                for (int i = 0; i < printSize; i++)
                {
                    Console.Write(" "+ string.Format(numFormat, veca[i]) + ", ");
                }
                if (veca.Length > 2 * printSize)
                {
                    Console.Write(" ...");
                }
                for (int i = slotCount - printSize; i < slotCount; i++)
                {
                    Console.Write(", " + string.Format(numFormat, veca[i]));
                }
                Console.WriteLine(" ]");
            }
            Console.WriteLine();
        }
    }
}