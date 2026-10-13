using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class EncryptedLocalStorage : ISaveStorage
{
    const byte FormatVersion = 1;
    const int IvSize = 16;
    const int MacSize = 32;

    const string Secret = "dsjfkla#!sdfsj(2u523$#sal!d124@fdkjgk%#@4alk2";

    readonly LocalFileStorage file;
    readonly byte[] encryptionKey;
    readonly byte[] macKey;

    public string MainPath => file.MainPath;

    public EncryptedLocalStorage(string fileName = "save.dat") 
    {
        file = new LocalFileStorage(fileName);

        using SHA256 sha = SHA256.Create();
        encryptionKey = sha.ComputeHash(Encoding.UTF8.GetBytes(Secret + "_enc"));
        macKey = sha.ComputeHash(Encoding.UTF8.GetBytes(Secret + "_mac"));
    }

    public bool Exists => file.Exists;

    public bool TryLoadMain(out string content) 
    {
        content = null;
        return file.TryLoadMain(out string raw) && TryDecode(raw, "최신", out content);
    }

    public bool TryLoadBackup(out string content) 
    {
        content = null;
        return file.TryLoadBackup(out string raw) && TryDecode(raw, "백업", out content);
    }

    public void Save(string content) => file.Save(Encode(content));

    public void Delete() => file.Delete();

    // ---------------- 암호화 + 서명 ----------------
    string Encode(string plainText) 
    {
        byte[] plain = Encoding.UTF8.GetBytes(plainText);

        using Aes aes = Aes.Create();
        aes.Key = encryptionKey;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.GenerateIV(); // 저장할 때마다 새 IV

        byte[] cipher;
        using (ICryptoTransform encryptor = aes.CreateEncryptor())
            cipher = encryptor.TransformFinalBlock(plain, 0, plain.Length);

        //버전 IV 암호문
        byte[] body = new byte[1 + IvSize + cipher.Length];
        body[0] = FormatVersion;
        Buffer.BlockCopy(aes.IV, 0, body, 1, IvSize);
        Buffer.BlockCopy(cipher, 0, body, 1 + IvSize, cipher.Length);

        //서명
        byte[] mac;
        using (HMACSHA256 hmac = new HMACSHA256(macKey))
            mac = hmac.ComputeHash(body);

        byte[] all = new byte[body.Length + MacSize];
        Buffer.BlockCopy(body, 0, all, 0, body.Length);
        Buffer.BlockCopy(mac, 0, all, body.Length, MacSize);

        return Convert.ToBase64String(all);
    }

    // ---------------- 서명 확인 + 복호화 ----------------
    bool TryDecode(string raw, string label, out string plainText) 
    {
        plainText = null;
        try 
        {
            byte[] all = Convert.FromBase64String(raw.Trim());
            if (all.Length < 1 + IvSize + 16 + MacSize)
                throw new CryptographicException("파일이 너무 짧음");

            int bodyLength = all.Length - MacSize;

            byte[] expectedMac;
            using (HMACSHA256 hmac = new HMACSHA256(macKey))
                expectedMac = hmac.ComputeHash(all, 0, bodyLength);

            if (!FixedTimeEquals(all, bodyLength, expectedMac)) 
            {
                Debug.LogWarning($"Save {label} 저장 파일의 서명이 맞지 않음 (조작 또는 손상)");
                return false;
            }

            if (all[0] != FormatVersion)
                throw new CryptographicException($"알 수 없는 형식 버전 {all[0]}");

            byte[] iv = new byte[IvSize];
            Buffer.BlockCopy(all, 1, iv, 0, IvSize);
            int cipherLength = bodyLength - 1 - IvSize;

            using Aes aes = Aes.Create();
            aes.Key = encryptionKey;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform decryptor = aes.CreateDecryptor();
            byte[] plain = decryptor.TransformFinalBlock(all, 1 + IvSize, cipherLength);
            plainText = Encoding.UTF8.GetString(plain);
            return true;
        }
        catch (Exception e) 
        {
            Debug.LogWarning($"Save {label} 저장 파일 해독 실패: {e.Message}");
            return false;
        }
    }

    static bool FixedTimeEquals(byte[] data, int offset, byte[] expected) 
    {
        if (data.Length - offset != expected.Length) return false;
        
        for (int i = 0; i < expected.Length; i++) 
        {
            if (data[offset + i] != expected[i]) return false;
        }
        return true;
    }
}
