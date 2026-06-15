using System.Text;

namespace DerekNotifications;

public abstract class CryptUtil
{
    private const string MyChars = "ABCDEFGHIJKMNPQRSTUVWXYZ0123456789abcdefghijkmnpqrstuvwxyz";
    private const int Encrypt = 0;
    private const int Decrypt = 1;
    
    /// <summary>
    /// Creates a registration key from a registration number
    /// </summary>
    /// <param name="registrationNumber">Registration Number returned from CreateRegistrationNumber()</param>
    /// <returns>registration key</returns>    
    public static string CreateKey(string registrationNumber)
    {
        // Decrypt the serial number
        var decrypted = Crypt(registrationNumber, Decrypt);
        
        // Re-encrypt the registration number just so it will have different values when we send it back
        var encrypted = Crypt(decrypted, Encrypt);
        
        // Append encrypted 'RSI' to string
        encrypted = encrypted + "-" + Crypt("RSI", Encrypt);
        
        return encrypted;
    }

    private static string Crypt(string inputString, int action)
    {
        // Initial setup for encryption or decryption
        (string processedInput, int offset, int startIndex) = 
            action == Encrypt ? SetupEncryption(inputString) : SetupDecryption(inputString);

        var output = new StringBuilder(); // Using StringBuilder for better performance when concatenating strings
        output.Append(action == Encrypt ? GetChar(offset) : string.Empty);

        // Perform encryption or decryption
        for (int index = startIndex; index <= processedInput.Length; index++)
        {
            int dynamicOffset = action == Encrypt ? index + 1 : -index;
            int transformedValue = GetNumber(processedInput[index - 1].ToString()) + offset + dynamicOffset;
            output.Append(GetChar(transformedValue));
        }

        return output.ToString();
    }

    // Helper method for encryption setup
    private static (string processedInput, int offset, int startIndex) SetupEncryption(string input)
    {
        string trimmedInput = input.Trim();
        int offset = GetRandomKey();
        return (trimmedInput, offset, 1);
    }

    // Helper method for decryption setup
    private static (string processedInput, int offset, int startIndex) SetupDecryption(string input)
    {
        string trimmedInput = input.Trim();
        int offset = -GetNumber(trimmedInput[0].ToString());
        return (trimmedInput, offset, 2); // Start at 2 because the first char is the offset
    }    
    
    /// <summary>
    /// Gets the numerical position of a character in the MyChars string
    /// </summary>
    /// <param name="tcChar">Character to find position of</param>
    /// <returns>Position in MyChars minus 1</returns>
    private static int GetNumber(string tcChar)
    {
        return MyChars.IndexOf(tcChar, StringComparison.Ordinal);
    }

    /// <summary>
    /// Gets the character at a given position in the MyChars string
    /// </summary>
    /// <param name="tnIndex">Position to get character from</param>
    /// <returns>Character at the specified position</returns>
    private static string GetChar(int tnIndex)
    {
        // Handle negative indices by using absolute value
        int index = Math.Abs(tnIndex) % MyChars.Length;
        return MyChars[index].ToString();
    }

    /// <summary>
    /// Gets a random integer between 1 and 26
    /// </summary>
    /// <returns>Random integer between 1 and 26</returns>
    private static int GetRandomKey()
    {
        var random = new Random();
        return random.Next(26);
    }    
}
