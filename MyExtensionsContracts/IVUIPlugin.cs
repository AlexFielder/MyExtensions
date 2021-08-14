using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventor;

namespace MyExtensionsContracts
{
    public interface IVUIPlugin : IVPlugin
    {
        Inventor.Color color { get; }
        string ClientGraphicsCollectionName { get; }
        ApplicationEvents GenericAppEventHandler { get; set; }
        UserInterfaceEvents GenericUserInteractionEventHandler { get; set; }
        UserInputEvents GenericUserInputEventHandler { get; set; }
        FileAccessEvents GenericFileAccessEventHandler { get; set; }
        Document GenericActiveDocument { get; set; }
        SelectEvents m_SelectEvents { get; set; }
        MouseEvents m_MouseEvents { get; set; }
        TriadEvents m_TriadEvents { get; set; }
    }
}
