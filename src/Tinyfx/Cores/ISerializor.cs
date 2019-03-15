/*
 * 由SharpDevelop创建。
 * 用户： zhix
 * 日期: 2018/4/25
 * 时间: 16:54
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Text;

namespace Tinyfx.Cores
{
    /// <summary>
    /// Description of Class3.
    /// </summary>
    public interface ISerializor
    {
        string SerializorToString<T>(T obj);

        byte[] SerializorToBytes<T>(T obj);

        Stream SerializorToStream<T>(T obj);

        T DeserializorFromString<T>(string text);

        T DeserializorFromBytes<T>(byte[] data);

        T DeserializorFromStream<T>(Stream stream);
    }
}
