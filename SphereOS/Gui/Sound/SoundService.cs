using Cosmos.HAL;
using Cosmos.HAL.Audio;
using Cosmos.HAL.Drivers.PCI.Audio;
using Cosmos.System;
using Cosmos.System.Audio;
using Cosmos.System.Audio.IO;
using SphereOS.Core;
using SphereOS.Logging;
using System;

namespace SphereOS.Gui.Sound
{
    internal class SoundService : Process
    {
        public SoundService() : base("SoundService", ProcessType.Service)
        {
        }

        internal AudioMixer Mixer { get; private set; }
        internal AudioDriver Driver { get; private set; }
        internal AudioManager AudioManager { get; private set; }

        internal bool DriverReady { get; private set; } = false;

        private const int bufferSize = 4096;

        private static class SystemSounds
        {
            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Sounds.Login.wav")]
            private static byte[] _soundBytes_Login;
            internal static MemoryAudioStream Sound_Login = MemoryAudioStream.FromWave(_soundBytes_Login);

            [IL2CPU.API.Attribs.ManifestResourceStream(ResourceName = "SphereOS.Gui.Resources.Sounds.Alert.wav")]
            private static byte[] _soundBytes_Alert;
            internal static MemoryAudioStream Sound_Alert = MemoryAudioStream.FromWave(_soundBytes_Alert);
        }

        internal void PlaySystemSound(SystemSound systemSound)
        {
            if (!DriverReady) return;

            switch (systemSound)
            {
                case SystemSound.None:
                    return;
                case SystemSound.Login:
                    Mixer.Streams.Add(SystemSounds.Sound_Login);
                    break;
                case SystemSound.Alert:
                    Mixer.Streams.Add(SystemSounds.Sound_Alert);
                    break;
                default:
                    throw new Exception("Unknown system sound.");
            }
        }

        #region Process
        internal override void Start()
        {
            base.Start();
            Log.Info("SoundService", "Starting SoundService.");

            for (int i = 0; i < Cosmos.HAL.PCI.Count; i++)
            {
                var device = Cosmos.HAL.PCI.Devices[i];

                /* AC97 */
                if (device.ClassCode == (byte)ClassID.MultimediaDevice &&
                    device.Subclass == 0x01
                    && !VMTools.IsVMWare)
                {
                    Log.Info("SoundService", "Initialising AC97 driver.");
                    Driver = AC97.Initialize(bufferSize);
                }
            }

            if (Driver != null)
            {
                DriverReady = true;

                Mixer = new AudioMixer();
                AudioManager = new AudioManager()
                {
                    Stream = Mixer,
                    Output = Driver
                };
                AudioManager.Enable();

                Log.Info("SoundService", "SoundService ready.");
            }
            else
            {
                Log.Warning("SoundService", "No driver available.");
            }
        }

        internal override void Run()
        {
        }

        internal override void Stop()
        {
            base.Stop();
            if (DriverReady)
            {
                Log.Info("SoundService", "Driver disabling.");
                AudioManager.Disable();
            }
        }
        #endregion Process
    }
}
