SYSTEM REQUIREMENTS
- Windows 7/8/10
- .NET Framework 4.5 or higher
- dot program (dot.exe) installed on the system

TO RUN PROGRAM
- To run the program, you'll need to
	- Launch Command Prompt and navigate to the folder containing the MODA.Console.exe file
	- Then type in the command:
		MODA.Console <graphFolder> <filename> <subGraphSize> <threshold> <getOnlyMappingCounts> <useModifiedGrochow
		where:
		- <graphFolder>: the (relative or absolute) folder the input graph file is.
		- <filename>: the input graph file name.
		- <subGraphSize>: the subgraph size you're interested in, or you want to query.
		- <threshold>: Frequency value, above which we can comsider the subgraph a "frequent subgraph"
		- <getOnlyMappingCounts>: Possible values: y or n (signifying yes or no). If yes, it means we only care about 
		                          how many mappings are found for each subgraph, not info about the mappings themselves.
		- <useModifiedGrochow>: If true, the program will use the newly proposed modified Grochow's algorithm (Algo 2)

- To run with the test data, simply run with the command:
	MODA.Console . Scere20141001CR_idx.txt 4

- Subgraph sizes (the last parameter in the command) below 3 and above 5 are not (yet) supported
- You will get the chance to provide sample file of the query graph (subgraph) you're interested in, if you have one

