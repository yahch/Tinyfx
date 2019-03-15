/*
 * 由SharpDevelop创建。
 * 用户： zhix
 * 日期: 2018/4/25
 * 时间: 17:12
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Tinyfx.Cores;

namespace Tinyfx.Common
{
    /// <summary>
    /// Description of Class2.
    /// </summary>
    public class XmlSerializor : ISerializor
    {
        #region ISerializor implementation
        public string SerializorToString<T>(T obj)
        {
            string output = "";
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                output = writer.ToString();
            }
            return output;
        }
        public byte[] SerializorToBytes<T>(T obj)
        {
            byte[] output = null;
            var serializer = new XmlSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(ms, obj);
                output = ms.ToArray();
            }
            return output;
        }
        public Stream SerializorToStream<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            var ms = new MemoryStream();
            serializer.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
        public T DeserializorFromString<T>(string text)
        {
            T output = default(T);
            using (var reader = new StringReader(text))
            {
                var serializer = new XmlSerializer(typeof(T));
                var obj = serializer.Deserialize(reader);
                output = (T)obj;
            }
            return output;
        }
        public T DeserializorFromBytes<T>(byte[] data)
        {
            T output = default(T);
            using (var ms = new MemoryStream(data))
            {
                var serializer = new XmlSerializer(typeof(T));
                var obj = serializer.Deserialize(ms);
                output = (T)obj;
            }
            return output;
        }
        public T DeserializorFromStream<T>(System.IO.Stream stream)
        {
            T output = default(T);
            var serializer = new XmlSerializer(typeof(T));
            var obj = serializer.Deserialize(stream);
            output = (T)obj;
            return output;
        }
        #endregion

    }
}
