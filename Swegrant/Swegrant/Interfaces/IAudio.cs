using Swegrant.Models;
using Swegrant.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swegrant.Interfaces
{
    public interface IAudio
    {
        void PrepareAudioFile(Language selectedLanguage, string fileName);

        void PlayAudioFile(Language selectedLanguage);

        void StopAudioFile();
    }
}
