using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System;
using System.IO;
using System.Diagnostics;

namespace ParaMODA.Impl.Graphics
{
    public sealed class FileDotEngine : IDotEngine
    {
        private string _dotProgramLocation;
        public FileDotEngine(string dotProgramLocation)
        {
            if (string.IsNullOrWhiteSpace(dotProgramLocation))
            {
                throw new ArgumentNullException("dotProgramLocation");
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
            if (!outputFileName.EndsWith(".dot", StringComparison.CurrentCultureIgnoreCase))
            {
                outputFileName = outputFileName + ".dot";
            }
            using (var writer = File.CreateText(outputFileName))
            {
                writer.Write(dot);
            }

            #region Run dot.exe program
            var args = string.Format(@"{0} -Tpng -o {0}.png", outputFileName);
            try
            {
                Process.Start(_dotProgramLocation, args);
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
