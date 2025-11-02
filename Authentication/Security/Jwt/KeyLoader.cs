using System.Security.Cryptography;

namespace Authentication.Security;

public static class KeyLoader
{
    public static ECDsa LoadKey(string relativePath)
    {
        var basePath = AppContext.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(basePath, @"..\..\..\.."));
        var fullPath = Path.Combine(projectRoot, relativePath);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException($"Key file not found at {fullPath}");

        var pem = File.ReadAllText(fullPath);
        var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(pem);
        return ecdsa;
    }
}