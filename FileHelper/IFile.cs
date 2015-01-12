using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileHelper
{
    public interface IFile
    {
        string TypeName { get; }
        string Name { get; }

    }
}
