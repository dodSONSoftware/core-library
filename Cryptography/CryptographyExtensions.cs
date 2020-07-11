using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Cryptography
{
    /// <summary>
    /// Provides extension methods to the <see cref="System.Security.SecureString"/> type.
    /// </summary>
    public static class CryptographyExtensions
    {
        #region Extension Methods
        /// <summary>
        /// Appends the <paramref name="password"/> byte array to the end of the current <see cref="System.Security.SecureString"/>.
        /// </summary>
        /// <param name="secureStr">This <see cref="System.Security.SecureString"/>.</param>
        /// <param name="password">The byte array to append to the end of the current <see cref="System.Security.SecureString"/>.</param>
        /// <param name="encoder">The <see cref="System.Text.Encoding"/> used to encode the string into a byte array.</param>
        /// <param name="autoClearIncomingByteArray">If <b>true</b>, will automatically clear the contents of the incoming byte array; <b>false</b>, will leave the contents of the incoming byte array as is.</param>
        /// <remarks>
        /// <para>
        /// <b>Important:</b> It is incumbent on the developer using this function to clear the incoming byte array after using it. 
        /// Otherwise, it will contain the characters of the password (as bytes) in the clear and it will remain in memory until the garbage collector frees the memory and the memory is overwritten. 
        /// Also note, the garbage collector freeing the memory does not clear the memory; the memory will remain as is until it is allocated and written over. See <see cref="Array.Clear"/> for more information.
        /// </para>
        /// </remarks>
        /// <returns>A <see cref="System.Security.SecureString"/> populated with the <paramref name="password"/>.</returns>
        public static System.Security.SecureString AppendBytes(this System.Security.SecureString secureStr,
                                       byte[] password,
                                       System.Text.Encoding encoder,
                                       bool autoClearIncomingByteArray)
        {
            if ((password == null) || (password.Length == 0)) { throw new ArgumentNullException("password"); }
            if (encoder == null) { throw new ArgumentNullException("encoder"); }
            char[] worker = null;
            try
            {
                worker = encoder.GetChars(password);
                foreach (var ch in worker)
                {
                    secureStr.AppendChar(ch);
                }
            }
            finally
            {
                if (worker != null) { Array.Clear(worker, 0, worker.Length); }
                if (autoClearIncomingByteArray) { Array.Clear(password, 0, password.Length); }
            }
            return secureStr;
        }
        /// <summary>
        /// Appends the <paramref name="password"/> char array to the end of the current <see cref="System.Security.SecureString"/>.
        /// </summary>
        /// <param name="secureStr">This <see cref="System.Security.SecureString"/>.</param>
        /// <param name="password">The char array to append to the end of the current <see cref="System.Security.SecureString"/>.</param>
        /// <param name="autoClearIncomingCharArray">If <b>true</b>, will automatically clear the contents of the incoming char array; <b>false</b>, will leave the contents of the incoming char array as is.</param>
        /// <remarks>
        /// <para>
        /// <b>Important:</b> It is incumbent on the developer using this function to clear the incoming char array after using it. 
        /// Otherwise, it will contain the characters of the password (as chars) in the clear and it will remain in memory until the garbage collector frees the memory and the memory is overwritten. 
        /// Also note, the garbage collector freeing the memory does not clear the memory; the memory will remain as is until it is allocated and written over. See <see cref="Array.Clear"/> for more information.
        /// </para>
        /// </remarks>
        /// <returns>A <see cref="System.Security.SecureString"/> populated with the <paramref name="password"/>.</returns>
        public static System.Security.SecureString AppendChars(this System.Security.SecureString secureStr,
                                       char[] password,
                                       bool autoClearIncomingCharArray)
        {
            if ((password == null) || (password.Length == 0)) { throw new ArgumentNullException("password"); }
            try
            {
                foreach (var ch in password)
                {
                    secureStr.AppendChar(ch);
                }
            }
            finally
            {
                if (autoClearIncomingCharArray) { Array.Clear(password, 0, password.Length); }
            }
            return secureStr;
        }
        #endregion
    }
}
