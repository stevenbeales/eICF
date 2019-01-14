Imports System.IO
Imports System.Security
Imports System.Security.Cryptography
Imports System.Text

''' <summary>
''' Class to encrypt strings such as passwords
''' Used to encrypt passwords in eICF.config
''' </summary>
'''<example>   
''' To encrypt a string - Encrypter.EncryptString(Encrypter.ToSecureString("Welcome!1"))
''' To decrypt a string - Encrypter.DecryptString(Encrypter.ToInSecureString("[EncryptedString]"))
''' </example>
Public Class Encrypter

    ' Salt used to see our encryption. Do not change.
    Private Const Salt As String = "eICFSalty"
    'Salt our encryption
    Shared ReadOnly Entropy As Byte() = Encoding.Unicode.GetBytes(Salt)

    ''' <summary>
    ''' Encrypt String Symmetrically using Salt
    ''' </summary>
    ''' <param name="input">Secure String</param>
    ''' <returns>String</returns>
    Public Shared Function EncryptString(input As SecureString) As String
        Dim encryptedData As Byte() = ProtectedData.Protect(Encoding.Unicode.GetBytes(ToInsecureString(input)), entropy, DataProtectionScope.CurrentUser)
        Return Convert.ToBase64String(encryptedData)
    End Function

    ''' <summary>
    ''' Decrypt String encrypted with above encryption algorithm using above salt
    ''' </summary>
    ''' <param name="encryptedData">Data to decrypt</param>
    ''' <returns>A secure string</returns>
    Public Shared Function DecryptString(encryptedData As String) As SecureString
        Try
            Dim decryptedData As Byte() = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), entropy, DataProtectionScope.CurrentUser)
            Return ToSecureString(Encoding.Unicode.GetString(decryptedData))
        Catch
            Return New SecureString()
        End Try
    End Function

    ''' <summary>
    ''' Creates a secure in-memory string from a string
    ''' Note secure strings are limited to about 32K in memory
    ''' </summary>
    ''' <param name="input">String to encrypt</param>
    ''' <returns>Encrypted string.</returns>
    Public Shared Function ToSecureString(input As String) As SecureString
        Dim secure As New SecureString()
        For Each c As Char In input
            secure.AppendChar(c)
        Next
        secure.MakeReadOnly()
        Return secure
    End Function


    ''' <summary>
    ''' Creates a string from a secure string
    ''' </summary>
    ''' <param name="input">The secure string to decrypt</param>
    ''' <returns>The decrypted string</returns>
    Public Shared Function ToInsecureString(input As SecureString) As String
        Dim returnValue = ""
        Dim ptr As IntPtr = Runtime.InteropServices.Marshal.SecureStringToBSTR(input)
        Try
            returnValue = Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr)
        Finally
            Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr)
        End Try
        Return returnValue
    End Function

    ''' <summary>
    ''' AES Fast Encryption
    ''' </summary>
    Public Shared Function Encrypt(text As String) As String
        ' AesCryptoServiceProvider
        Dim aes As New AesCryptoServiceProvider()
        aes.BlockSize = 128
        aes.KeySize = 128
        aes.IV = Encoding.UTF8.GetBytes(AesIV)
        aes.Key = Encoding.UTF8.GetBytes(AesKey)
        aes.Mode = CipherMode.CBC
        aes.Padding = PaddingMode.PKCS7

        ' Convert string to byte array
        Dim src As Byte() = Encoding.Unicode.GetBytes(text)

        ' encryption
        Using enc As ICryptoTransform = aes.CreateEncryptor()
            Dim dest As Byte() = enc.TransformFinalBlock(src, 0, src.Length)

            ' Convert byte array to Base64 strings
            Return Convert.ToBase64String(dest)
        End Using
    End Function

    ''' <summary>
    ''' AES Fast decryption
    ''' </summary>
    Public Shared Function Decrypt(text As String) As String
        ' AesCryptoServiceProvider
        Dim aes As New AesCryptoServiceProvider()
        aes.BlockSize = 128
        aes.KeySize = 128
        aes.IV = Encoding.UTF8.GetBytes(AesIV)
        aes.Key = Encoding.UTF8.GetBytes(AesKey)
        aes.Mode = CipherMode.CBC
        aes.Padding = PaddingMode.PKCS7

        ' Convert Base64 strings to byte array
        Dim src As Byte() = Convert.FromBase64String(text)

        ' decryption
        Using dec As ICryptoTransform = aes.CreateDecryptor()
            Dim dest As Byte() = dec.TransformFinalBlock(src, 0, src.Length)
            Return Encoding.Unicode.GetString(dest)
        End Using
    End Function

    ''' <summary>
    ''' EncryptXml
    ''' </summary>
    ''' <param name="xml">XML to encrypt</param>
    ''' <param name="backupPath">Backup folder</param>
    ''' <param name="backupFileName">Backup file name</param>
    Public Shared Sub EncryptXml(xml As String, backupPath As String, backupFileName As String)
        Dim encryptedxml As String = Encrypt(xml)
        Dim encryptedfile As String = Path.Combine(backupPath, Path.GetFileName(backupFileName))
        If File.Exists(encryptedfile) Then
            File.Delete(encryptedfile)
        End If
        Using sw As StreamWriter = File.CreateText(encryptedfile)
            sw.Write(encryptedxml)
        End Using
    End Sub

    Public Shared Sub RemoveXmlFile(backupPath As String, backupFileName As String)
        Dim encryptedfile As String = Path.Combine(backupPath, Path.GetFileName(backupFileName))
        If File.Exists(encryptedfile) Then
            File.Delete(encryptedfile)
        End If
    End Sub

    ' 128bit(16byte)IV and Key
    Private Const AesIV As String = "!QAZ2WSX#EDC4RFV"
    Private Const AesKey As String = "5TGB&YHN7UJM(IK<"

End Class

