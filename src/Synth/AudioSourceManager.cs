using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Groorine.Api;
using Groorine.Helpers;

namespace Groorine.Synth
{
	public class AudioSourceManager
	{	
		public IInstrument[] Instruments { get; } = new IInstrument[128];
		public InstrumentList Drumset { get; } = new InstrumentList();

        public static AudioSourceManager Instance { get; } = new AudioSourceManager();

		private AudioSourceManager() 
        { 
            InternalInitialize();
        }

		private void AddInstrument(byte ch, IInstrument inst)
		{
			if (inst == null)
				return;
			if (ch >= 128)
				throw new ArgumentOutOfRangeException(nameof(ch));
			Instruments[ch] = inst;
		}


        private void InternalInitialize()
        {
            var asm = GetType().Assembly;
            var allResources = asm.GetManifestResourceNames();

            // Load All Sources
            for (byte i = 0; i < 128; i++)
            {
                // Instruments
                {
                    if (allResources.FirstOrDefault(n => n.StartsWith($"GroorineCore.Presets.Inst.{i}")) is string name)
                    {
                        var src = ReadAudioSourceStream(name, asm.GetManifestResourceStream(name));
                        AddInstrument(i, new Instrument(i, src));
                    }
                    else
                    {
                        AddInstrument(i, new Instrument(i, new AudioSourceSine()));
                    }
                }
                // Drum
                {
                    if (allResources.FirstOrDefault(n => n.StartsWith($"GroorineCore.Presets.Drum.{i}")) is string name)
                    {
                        var src = ReadAudioSourceStream(name, asm.GetManifestResourceStream(name));
                        Drumset?.Add(new Instrument(i, src));
                    }
                }
            }
        }

        private static IAudioSource ReadAudioSourceStream(string name, Stream stream)
        {
			// 現状拡張子のみで判断しているけどもっと良い方法ないかな
			switch (Path.GetExtension(name).ToLowerInvariant())
			{
				case ".mssf":    // Music Sheet Sound File
					return FileUtility.LoadMssf(stream);

				case ".gsef":    // Groorine Sound Effect File
					throw new NotImplementedException("GSEF ファイルはまだサポートされていません。");

				case ".wav":
				case ".wave":// Wave
					return new AudioSourceWav(stream);

				default:
					throw new InvalidOperationException("サポートされていないファイルです。");
			}
        }
	}
}
