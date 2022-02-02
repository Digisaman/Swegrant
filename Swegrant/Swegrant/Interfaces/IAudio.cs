using Swegrant.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swegrant.Interfaces
{
    public interface IAudio
    {
        void PrepareAudioFile(Language currentLanguage, string fileName);

        void PlayAudioFile(Language currentLanguage);

        void StopAudioFile();
    }
}
