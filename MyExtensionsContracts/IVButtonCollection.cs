using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExtensionsContracts
{
    public interface IVButtonCollection : IVPlugin
    {
        List<IVButtonPlugin> ButtonCollection { get; set; }
    }
}
