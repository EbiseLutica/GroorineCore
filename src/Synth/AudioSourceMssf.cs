﻿using System;
using Groorine.DataModel;
using Groorine.Helpers;

namespace Groorine.Synth
{

	public class AudioSourceMssf : IAudioSource
	{
		/// <summary>
		/// 波形データを取得します．
		/// </summary>
		public short[] Wave { get; }

		/// <summary>
		/// エンベロープデータを取得します．
		/// </summary>
		public Envelope Envelope { get; }

		/// <summary>
		/// -256 ～ 255 の範囲をとるパンポットを取得します。
		/// </summary>
		public int Pan { get; }

		internal AudioSourceMssf(short[] wave, Envelope envelope, int pan)
		{
			if (wave == null)
				throw new ArgumentNullException(nameof(wave));
			if (wave.Length != 32)
				throw new ArgumentException("Wavetable size should be 32.");
			Wave = wave;
			Envelope = envelope;
			Pan = pan;
			
		}


		public (short, short) GetSample(int index, double sampleRate, Tone t)
		{
			var time = MidiTimingConverter.GetTime(index, (int)sampleRate);


			var i = (int)((index % (100)) * (32 * 0.01));
			if (i > 31)
				i -= 32;
			var o = Wave[i];
			float vol;
			EnvelopeFlag flag = EnvelopeFlag.Attack;
			if (time > Envelope.A)
				flag = EnvelopeFlag.Decay;
			if (time > Envelope.A + Envelope.D)
				flag = EnvelopeFlag.Sustain;
			switch (flag)
			{
				case EnvelopeFlag.Attack:
					vol = (float)MathHelper.Linear(time, 0, Envelope.A, 0, 1);
					break;
				case EnvelopeFlag.Decay:
					vol = (float)MathHelper.Linear(time, Envelope.A, Envelope.A + Envelope.D, 1, Envelope.S * 0.0039);
					break;
				case EnvelopeFlag.Sustain:
					vol = Envelope.S * 0.0039f;
					break;
				case EnvelopeFlag.Release:
				case EnvelopeFlag.None:
				default:
					vol = 0;
					break;
			}
			if (t != null) t.EnvVolume = vol;
			short a = (short)(Math.Min(short.MaxValue, Math.Max(short.MinValue, o * vol)));
			return (a, a);

		}
	}
}