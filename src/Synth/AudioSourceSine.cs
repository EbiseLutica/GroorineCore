﻿using System;
using Groorine.Helpers;

namespace Groorine.Synth
{

	/// <summary>
	/// 正弦波を出力する音源を表します。
	/// </summary>
	public class AudioSourceSine : AudioSourceWaveTable
	{
		
		public override (short, short) GetSample(int index)
		{
			var sample = (short)(Math.Sin(MathHelper.ToRadian(index * 360)) * 32767);
			return (sample, sample);
		}
	}


}