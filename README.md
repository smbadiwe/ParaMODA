# ParaMODA

This work improves a motif-centric tool which will enable researchers find subgraph instances in the input network for only the subgraphs of interest – called query graphs – as opposed to finding instances for all possible (non-isomorphic) k-graphs. 

The tool, called ParaMODA, incorporates the existing motif-centric algorithms - namely [Motif Discovery Algorithm (MODA)](http://www.ncbi.nlm.nih.gov/pubmed/20154426) and [Grochow-Kellis (GK) Algorithm)](http://compbio.mit.edu/publications/C04_Grochow_RECOMB_07.pdf) - as well as a new algorithm for carrying out the same task. The new algorithm is essentially a modification to GK's. The new algorithm performed better than GK's in all the cases tested, although the performance improvement varied with the shape of the query graphs.

More importantly the new algorithm allows for parallelization of huge chunks of the task – which will be helpful, especially for studying motifs of double-digit sizes in large networks.

The tool also (optionally) collect and store discovered instances on the disk for future retrieval and analysis.

## Running the Program (Windows)
To run the program, just fork or clone or download the source code here. Then open in Visual Studio and build the project called **ParaMODA**. It should build successfully without any problems. The successful build will generate an exe file which you can then run on command prompt. If you don't want to compile source, you can [download the already compiled code](https://github.com/smbadiwe/ParaMODA/releases/download/v1.0/ParaMODA_v1.0_bin_win.zip).

The command:`ParaMODA --help` will show you the available (verb) options, what they are for and how to use them.

## Can't Run On Windows Machine?
For users wanting to run program on non-Windows machine, all the source files for this project will work on .NET Core. It should be easy to create .NET Core projects, import the source files accordingly, build and run. This porting will be provided for in future.
