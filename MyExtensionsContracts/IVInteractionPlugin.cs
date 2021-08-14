using Inventor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExtensionsContracts
{
    public interface IVInteractionPlugin : IVPlugin
    {
        InteractionEvents m_InteractionEvents { get; set; }

        //SelectEvents object
        SelectEvents m_SelectEvents { get; set; }

        //MouseEvents object
        MouseEvents m_MouseEvents { get; set; }

        //TriadEvents object
        TriadEvents m_TriadEvents { get; set; }

    }
}
