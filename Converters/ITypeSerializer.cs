using System;

namespace dodSON.Core.Converters
{
    /// <summary>
    /// Defines methods to convert any, serializable, <see cref="Type"/> to, and from, strings, bytes arrays, streams, and xml formatted string.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> to convert. <b>Note:</b> This <see cref="Type"/> must be serializable.</typeparam>
    public interface ITypeSerializer<T>
    {
        /// <summary>
        /// Converts the source instance, of type <typeparamref name="T"/>, into a string.
        /// </summary>
        /// <param name="source">The source instance, of type <typeparamref name="T"/>, to convert.</param>
        /// <returns>A <see cref="string"/> containing the converted contents of <paramref name="source"/>.</returns>
        /// <remarks>The generic type <typeparamref name="T"/> must be serializable.</remarks>
        string ToString(T source);
        /// <summary>
        /// Converts the source instance, of type <typeparamref name="T"/>, into a byte array.
        /// </summary>
        /// <param name="source">The source <see cref="Type"/> to convert.</param>
        /// <returns>A byte array containing the converted contents of <paramref name="source"/>.</returns>
        byte[] ToByteArray(T source);
        /// <summary>
        /// Converts the source instance, of type <typeparamref name="T"/>, into a byte array and writes it into the <paramref name="stream"/>.
        /// </summary>
        /// <param name="source">The source instance, of type <typeparamref name="T"/>, to convert.</param>
        /// <param name="stream">The stream to write the converted <paramref name="source"/> into.</param>
        /// <returns>The number of bytes written into the <paramref name="stream"/>.</returns>
        int ToStream(T source, System.IO.Stream stream);
        /// <summary>
        /// Converts the source instance, of type <typeparamref name="T"/>, into an xml formatted string.
        /// </summary>
        /// <param name="source">The source <see cref="Type"/> to convert.</param>
        /// <returns>An xml <see cref="string"/> containing the converted contents of <paramref name="source"/>.</returns>
        string ToXmlString(T source);

        // ********

        /// <summary>
        /// Converts the <paramref name="content"/> from a string into a type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="content">The string to convert into the type <typeparamref name="T"/>.</param>
        /// <returns>A type <typeparamref name="T"/> which represents the converted string <paramref name="content"/>.</returns>
        T FromString(string content);
        /// <summary>
        /// Converts the <paramref name="content"/> from a byte array into a type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="content">The byte array to convert into the type <typeparamref name="T"/>.</param>
        /// <returns>A type <typeparamref name="T"/> which represents the converted byte array <paramref name="content"/>.</returns>
        T FromByteArray(byte[] content);
        /// <summary>
        /// Converts the <paramref name="content"/> from a entire stream into a type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="content">The stream to convert into the type <typeparamref name="T"/>.</param>
        /// <returns>A type <typeparamref name="T"/> which represents the converted stream <paramref name="content"/>.</returns>
        T FromStream(System.IO.Stream content);
        /// <summary>
        /// Converts the next <paramref name="length"/> of bytes in the <paramref name="content"/> stream into a type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="content">The stream to convert into the type <typeparamref name="T"/>.</param>
        /// <param name="length">The number of bytes to read from the stream.</param>
        /// <returns>A type <typeparamref name="T"/> which represents the converted stream <paramref name="content"/>.</returns>
        T FromStream(System.IO.Stream content, int length);
        /// <summary>
        /// Converts the <paramref name="content"/> from a xml formatted string into a type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="content">The xml formatted string to convert into the type <typeparamref name="T"/>.</param>
        /// <returns>A type <typeparamref name="T"/> which represents the converted xml string <paramref name="content"/>.</returns>
        T FromXmlString(string content);
    }
}
