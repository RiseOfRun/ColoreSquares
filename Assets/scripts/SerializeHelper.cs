using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public static class SerializeHelper
{
    public static string Serialize<T>(this T toSerialize)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringWriter writer = new StringWriter();
        serializer.Serialize(writer, toSerialize);
        
        return writer.ToString();
    }

    public static T DeSerialize<T>(this string toDeSerialize)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringReader reader = new StringReader(toDeSerialize);
        return (T) serializer.Deserialize(reader);
    }
    
}
