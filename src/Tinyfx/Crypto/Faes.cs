/*
 * 由SharpDevelop创建。
 * 用户： zhix
 * 日期: 2018/5/11
 * 时间: 16:02
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tinyfx.Crypto
{
	/// <summary>
	/// Description of Faes.
	/// </summary>
	public class Faes
	{
		private byte[] _Key = null;

		private byte[] _IV = null;

		public byte[] Key { 
			get { 
				return _Key; 
			}
		}
        
		public byte[] IV { 
			get {
				return _IV;
			}
		}

		private RijndaelManaged rijndaelManaged;

		public Faes()
		{
			_Key = new byte[] {
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71,
				0x71
			};
			_IV = new byte[] {
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52,
				0x52
			};
			Init();
		}

		public Faes(byte[] key, byte[] iv)
		{
			_Key = key;
			_IV = iv;
			Init();
		}

		private void Init()
		{
			rijndaelManaged = new RijndaelManaged {
				Key = Key,
				IV = IV,
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7,
			};
		}

		public string Encrypt(string text)
		{
			byte[] src = Encoding.UTF8.GetBytes(text);
			byte[] srcEnc = Encrypt(src);
			return Convert.ToBase64String(srcEnc);
		}

		public string Decrypt(string text)
		{
			byte[] src = Convert.FromBase64String(text);
			byte[] srcDec = Decrypt(src);
			return Encoding.UTF8.GetString(srcDec);
		}

		public byte[] Encrypt(byte[] input)
		{
			return rijndaelManaged.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
		}

		public byte[] Decrypt(byte[] input)
		{
			return rijndaelManaged.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
		}

		public void Encrypt(Stream input, Stream output)
		{
			using (var cryptoStream = new CryptoStream(input, rijndaelManaged.CreateEncryptor(), CryptoStreamMode.Read)) {
				cryptoStream.CopyTo(output);
			}
		}

		public void Decrypt(Stream input, Stream output)
		{
			using (var cryptoStream = new CryptoStream(input, rijndaelManaged.CreateDecryptor(), CryptoStreamMode.Read)) {
				cryptoStream.CopyTo(output);
			}
		}

		public override string ToString()
		{
			return rijndaelManaged.KeySize + "/" + rijndaelManaged.Mode.ToString() + "/" + rijndaelManaged.Padding.ToString();
		}

	}
}
