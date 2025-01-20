using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace B62Compression;

public static class CompressionUtils
{
    public static byte[] CompressGzip(this byte[] data)
    {
        using var source = new MemoryStream(data);
        using var destination = new MemoryStream();
        using (var compressor = new GZipStream(destination, CompressionMode.Compress))
        {
            source.CopyTo(compressor);
        }
        return destination.ToArray();
    }

    /// <summary>
    /// </summary>
    /// <param name="s"></param>
    /// <param name="encoding">UTF8 will be used if left null</param>
    /// <returns></returns>
    public static byte[] GzipString(this string s, Encoding? encoding = null)
    {
        var activeEncoder = encoding ?? Encoding.UTF8;
        var asBytes = activeEncoder.GetBytes(s);
        return CompressGzip(asBytes);
    }

    public static byte[] DecompressGzip(this byte[] data)
    {
        using var source = new MemoryStream(data);
        using var destination = new MemoryStream();
        using (var decompressor = new GZipStream(source, CompressionMode.Decompress))
        {
            decompressor.CopyTo(destination);
        }
        return destination.ToArray();
    }

    public static string DecompressGzipString(this byte[] bytes, Encoding? encoding = null)
    {
        var activeEncoder = encoding ?? Encoding.UTF8;
        var decompressedBytes = bytes.DecompressGzip();
        return activeEncoder.GetString(decompressedBytes);
    }

    public static byte[]? ToJsonSerializedGzipBytes<T>(this T value, JsonSerializerOptions jsonOpts)
    {
        if (value is null)
        {
            return default;
        }

        if (jsonOpts is null)
        {
            throw new NullReferenceException(nameof(jsonOpts));
        }

        var jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(value, jsonOpts);
        using var dest = new MemoryStream();
        using var gz = new GZipStream(dest, CompressionMode.Compress);
        gz.Write(jsonUtf8Bytes, 0, jsonUtf8Bytes.Length);
        gz.Close();
        return dest.ToArray();
    }

    public static T? FromJsonSerializedGzipBytes<T>(this byte[]? jsonSerializedGzipBytes, JsonSerializerOptions jsonOpts)
    {
        if (jsonSerializedGzipBytes is null || jsonSerializedGzipBytes.Length == 0)
        {
            return default;
        }

        if (jsonOpts is null)
        {
            throw new NullReferenceException(nameof(jsonOpts));
        }

        using var compressed = new MemoryStream(jsonSerializedGzipBytes);
        return FromJsonSerializedGzipBytes<T>(compressed, jsonOpts);
    }

    public static T? FromJsonSerializedGzipBytes<T>(this Stream? jsonSerializedGzipBytes, JsonSerializerOptions jsonOpts)
    {
        if (jsonSerializedGzipBytes is null || jsonSerializedGzipBytes.Length == 0)
        {
            return default;
        }

        if (jsonOpts is null)
        {
            throw new NullReferenceException(nameof(jsonOpts));
        }

        using var gz = new GZipStream(jsonSerializedGzipBytes, CompressionMode.Decompress);
        using var dest = new MemoryStream();
        gz.CopyTo(dest);
        var jsonBytes = dest.ToArray();
        var jsonReader = new Utf8JsonReader(jsonBytes);
        var deserialized = JsonSerializer.Deserialize<T>(ref jsonReader, jsonOpts);
        return deserialized;
    }
}