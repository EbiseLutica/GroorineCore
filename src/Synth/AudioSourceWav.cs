using System.IO;
using Groorine.Helpers;
using System;
using Groorine.DataModel;

namespace Groorine.Synth
{



	public class AudioSourceWav : IAudioSource
	{

		public (short, short)[] Samples;

		private int _sampleRate;



		public AudioSourceWav(Stream wavFile)
		{
			var br = new BinaryReader(wavFile);

			// RIFF ヘッダー
			if (br.ReadString(4) != "RIFF")
				throw new InvalidDataException("This file is not RIFF.");
			var size = br.ReadInt32();
			if (br.ReadString(4) != "WAVE")
				throw new InvalidDataException("This file is not WAVE.");

			// fmt チャンク
			if (br.ReadString(4) != "fmt ")
				throw new InvalidDataException("Invalid magic-number of fmt chunk.");
			var fmtsize = br.ReadInt32();
			var formatid = br.ReadInt16();
			var chCount = br.ReadInt16();
			if (chCount != 1 && chCount != 2)
				throw new InvalidDataException("Unsupported channels count. monoral and stereo .");
			_sampleRate = br.ReadInt32();
			var dataRate = br.ReadInt32();
			var blockSize = br.ReadInt16();
			var bitRate = br.ReadInt16();
			if (bitRate != 8 && bitRate != 16)
				throw new InvalidDataException("Unsupported bitrate.");
			bitRate /= 8;
			if (dataRate != _sampleRate * chCount * bitRate)
				throw new InvalidDataException("Data rate, sampling rate, bit rate and channels count are inconsistent.");
			if (blockSize != chCount * bitRate)
				throw new InvalidDataException("Block size, channels count and bit rate are inconsistent.");
			if (fmtsize >= 18)
			{
				var extSize = br.ReadInt16();
				br.ReadBytes(extSize);
			}

			var s = br.ReadString(4);

			if (s == "fact")
			{
				var factsize = br.ReadInt32();
				br.ReadBytes(factsize);
				s = br.ReadString(4);
			}

			if (s != "data")
				throw new InvalidDataException("Invalid magic-number of data chunk.");

			// dataチャンク
			var chunkSize = br.ReadInt32();
			var dataCount = chunkSize / bitRate / chCount;

			short l, r;
			Samples = new (short, short)[dataCount];
			for (var i = 0; i < dataCount; i++)
			{
				l = r = bitRate == 1 ? (short)((br.ReadByte() << 8) - 65536) : (br.ReadInt16());
				if (chCount == 2)
					r = bitRate == 1 ? (short)((br.ReadByte() << 8) - 65536) : (br.ReadInt16());
				Samples[i] = (l, r);
			}

		}


		public (short, short) GetSample(int index, double sampleRate, Tone t)
		{
			var i = index;
			return Samples.Length <= i ? (default, default) : Samples[i];
		}
	}


}