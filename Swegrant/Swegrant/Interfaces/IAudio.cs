using System;
using System.Collections.Generic;
using System.Text;

namespace Swegrant.Interfaces
{
    public interface IAudio
    {
        void PrepareAudioFile(string fileName);

        void PlayAudioFile();

        void StopAudioFile();
    }
}
