using System.Security.Cryptography;
using System.IO;

class Program
{
    static void Main()
    {

        string keysPath = @"C:\Users\zaid\Desktop\Authentication\Keys";
        
        Directory.CreateDirectory(keysPath);
        
        Console.WriteLine($" Keys folder: {keysPath}");

        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        string privateKeyPath = Path.Combine(keysPath, "private-key.pem");
        string publicKeyPath = Path.Combine(keysPath, "public-key.pem");

        var privateKey = ecdsa.ExportECPrivateKeyPem();
        File.WriteAllText(privateKeyPath, privateKey);

        var publicKey = ecdsa.ExportSubjectPublicKeyInfoPem();
        File.WriteAllText(publicKeyPath, publicKey);

        Console.WriteLine($"Private key: {privateKeyPath}");
        Console.WriteLine($"Public key:  {publicKeyPath}");
        Console.WriteLine();
        Console.WriteLine("Keys generated successfully!");
    }
}