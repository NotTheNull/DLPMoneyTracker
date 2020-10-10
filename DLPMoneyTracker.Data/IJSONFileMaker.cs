using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.Data
{
    public interface IJSONFileMaker
    {
        string FilePath { get; }

        void LoadFromFile();
        void SaveToFile();
    }
}
