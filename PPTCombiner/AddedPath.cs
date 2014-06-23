using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PPTCombiner
{
    enum PathType
    {
        ValidFile,
        InvalidFile,
        Folder,
        EmptyFolder
    }

    sealed class AddedPath
    {
        string Path { get; private set; }
        PathType PathType { get; private set; }
        int FileCount { get; private set; }
    }
}
