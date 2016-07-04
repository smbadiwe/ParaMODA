using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Graphics
{
    public sealed class FileDotEngine : IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            if (!outputFileName.EndsWith(".dot", StringComparison.InvariantCultureIgnoreCase))
            {
                outputFileName = outputFileName + ".dot";
            }
            using (StreamWriter writer = new StreamWriter(outputFileName))
            {
                writer.Write(dot);
            }

            #region Run dot.exe program
            var args = string.Format(@"{0} -Tpng -o {0}.png", outputFileName);
            var dotProgLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Graphviz2.38\\bin", "dot.exe");
            try
            {
                System.Diagnostics.Process.Start(dotProgLocation, args);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"Could not find dot.exe program in path: {dotProgLocation}", ex);
            }
            catch (Exception ex)
            {
                throw;
            } 
            #endregion
            return outputFileName;
        }
    }
}
