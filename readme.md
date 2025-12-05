# To compile executables for Nxtech benchmarking leaderboards, publish using the command 
# below. This will AOT compile every project, strip all symbols and diagnostics, and save 
# the executables in the 'Publish' folder in the solution root folder.
dotnet publish -r linux-x64 -c Release -o "./Publish"

# To compile executables for local benchmarking, publish using the command below.
# This will AOT compile every project, strip all symbols, but retain some diagnostics.
# Specifically, the execution time of calculating the puzzle solution will be printed, 
# and, if no external puzzle input is provided, a hardcoded puzzle input will be used. 
# With hardcoded input, the puzzle solution will be validated using an assert. As with
# the publish command above, the executables will be stored in the 'Publish' folder in 
# the solution root folder.
dotnet publish -r linux-x64 -c Release -p:DefineConstants=DEBUG -o "./Publish"

# When compiling for Nxtech benchmarking, each compiled program expects one or two input 
# arguments. The first input argument should be the path of the file that contains the 
# puzzle input to solve for. The second input is optional, and may be set to "1" to  
# force the program to perform a dry run. In a dry run, the puzzle input will be read 
# from the input file, but no additional logic is performed. This can be used to measure
# the I/O overhead of reading the puzzle input from file.
# 
# When compiling for local benchmarking, it is also possible to not pass any program 
# arguments, in which case the program will solve the puzzle using hardcoded puzzle
# input.