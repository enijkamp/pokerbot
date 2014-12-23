using System;
using System.Security.Cryptography;

namespace PokerBot
{	
	public class HashImage : Image {

		private readonly int hashCode;
		
		public HashImage(Image image) : base(image)
		{
			this.hashCode = computeHashCode(image);
		}
		
		private int computeHashCode(Image image) 
		{
			HashAlgorithm md5 = new MD5CryptoServiceProvider();
			byte[] bytes = new byte[pixels.Length * 4];
			int current = 0;
			foreach(int pixel in pixels) 
			{
				byte[] fourBytes = convertIntToByteArray(pixel);
				bytes[current++] = fourBytes[0];
				bytes[current++] = fourBytes[1];
				bytes[current++] = fourBytes[2];
				bytes[current++] = fourBytes[3];
			}
			byte[] hashBytes = md5.ComputeHash(bytes);
			string hashValue = Convert.ToBase64String(hashBytes);
			return hashValue.GetHashCode();
		}
		
		private static byte[] convertIntToByteArray(int val) 
		{
	   		byte[] buffer = new byte[4];   
	   		buffer[0] = (byte) (val >> 24);
	   		buffer[1] = (byte) (val >> 16);
	   		buffer[2] = (byte) (val >> 8);
	   		buffer[3] = (byte) val;
	   		return buffer;
	   	}
		
		public override bool Equals(object obj)
        {
            Image other = obj as Image;
            if (other == null) return false;
            return this.GetHashCode().Equals(other.GetHashCode());
        }
		
		public override int GetHashCode ()
		{
			return hashCode;
		}
	}

}