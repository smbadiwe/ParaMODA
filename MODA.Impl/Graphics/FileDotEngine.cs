using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System;
using System.IO;

namespace MODA.Impl.Graphics
{
    public sealed class FileDotEngine : IDotEngine
    {
        private string _dotProgramLocation;
        public FileDotEngine(string dotProgramLocation)
        {
            if (string.IsNullOrWhiteSpace(dotProgramLocation))
            {
                dotProgramLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Graphviz2.38\\bin", "dot.exe");
                //Or do I throw an exception?
            }
            _dotProgramLocation = dotProgramLocation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageType"></param>
        /// <param name="dot">The dot-representation of the graph in question</param>
        /// <param name="outputFileName">The output file</param>
        /// <returns></returns>
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
            //var dotProgLocation = System.Configuration.ConfigurationSettings.AppSettings.Get("dotProgLocation");
            //var dotProgLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Graphviz2.38\\bin", "dot.exe");
            try
            {
                System.Diagnostics.Process.Start(_dotProgramLocation, args);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"Could not find dot.exe program in path: {_dotProgramLocation}", ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion
            return outputFileName;
        }
    }
}
