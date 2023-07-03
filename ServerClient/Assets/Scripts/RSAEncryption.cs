using System.Security.Cryptography;
using System;


class RSAEncryption
{
    public static string pubKeyString;

    public static void SetPubKeyString(string _pubKeyString)
    {
        pubKeyString = _pubKeyString;
    }
    public static string Encrypt(string plainTextData)
    {
        //we have a public key ... let's get a new csp and load that key
        var csp = new RSACryptoServiceProvider();
        csp.ImportParameters(ConvertStringToPublicKey(pubKeyString));
        //for encryption, always handle bytes...
        var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);

        //apply pkcs#1.5 padding and encrypt our data 
        var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

        //we might want a string representation of our cypher text... base64 will do
        var cypherText = Convert.ToBase64String(bytesCypherText);

        return cypherText;
    }

    private static RSAParameters ConvertStringToPublicKey(string stringPublicKey)
    {
        //converting it back
        //get a stream from the string
        var sr = new System.IO.StringReader(stringPublicKey);
        //we need a deserializer
        var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        //get the object back from the stream
        var publicKey = (RSAParameters)xs.Deserialize(sr);
        return publicKey;
    }
}
