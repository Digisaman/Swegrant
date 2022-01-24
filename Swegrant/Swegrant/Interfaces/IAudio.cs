using System;
using System.Collections.Generic;
using System.Text;

namespace Swegrant.Interfaces
{
    public interface IAudio
    {
        void PlayAudioFile(string fileName);

        void StopAudioFile();
    }
}
