using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyExtensionsContracts
{
    /// <summary>
    /// Concept copied from this post: http://stackoverflow.com/a/3007754/572634
    /// 
    /// Description added based on this: https://www.codeproject.com/Articles/13821/Adding-Descriptions-to-your-Enumerations
    /// </summary>
    public class EnvironmentRibbonName
    {
        EnvironmentRibbonName(string display) { this.display = display; }

        string display;

        public override string ToString() { return display; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Part, Assembly, Drawing & ZeroDoc</remarks>
        [Description("Part, Assembly, Drawing & ZeroDoc")]
        public static readonly EnvironmentRibbonName All = new EnvironmentRibbonName("All");
        [Description("Part, Assembly, Drawing & ZeroDoc")]
        public static readonly EnvironmentRibbonName Main = new EnvironmentRibbonName("Main");
        [Description("Part, Assembly, Drawing & ZeroDoc")]
        public static readonly EnvironmentRibbonName Part = new EnvironmentRibbonName("Part");
        [Description("Assembly Only")]
        public static readonly EnvironmentRibbonName Assembly = new EnvironmentRibbonName("Assembly");
        [Description("Drawing Only")]
        public static readonly EnvironmentRibbonName Drawing = new EnvironmentRibbonName("Drawing");
        [Description("Presetnation Only")]
        public static readonly EnvironmentRibbonName Presentation = new EnvironmentRibbonName("Presentation");
        [Description("ZeroDoc - means when no files are open.")]
        public static readonly EnvironmentRibbonName ZeroDoc = new EnvironmentRibbonName("ZeroDoc");
        [Description("Parts and Assembly Only")]
        public static readonly EnvironmentRibbonName Modelling = new EnvironmentRibbonName("Modelling");

    }
}
