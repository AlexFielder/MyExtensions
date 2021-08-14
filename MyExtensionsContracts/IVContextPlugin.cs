using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExtensionsContracts
{
    public interface IVContextPlugin : IVPlugin
    {

        bool DisplayBigIcon { get; }
        bool DisplayText { get; }
        string CommandDisplayName { get; }
        string CommandInternalName { get; }

    }
}
