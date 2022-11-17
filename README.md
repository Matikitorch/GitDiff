# GitDiff

## Objective
Analyze open source GitHub projects for code smells, by iterating through their commit history and parsing every line of code. Currently C# is the only language that is supported.

## Instructions
1. Download GitDiff
2. Navigate to the executable directory ..\GitDiff\bin\Debug\net6.0
3. Open GitDiff.ini with any text editor
   - ProjectDirectory
     - Directory of the project to analyze for code smells
   - ResultsDirectory
     - Directory of where to store any code smell information, such as a CSV file
   - CommitLayers
     - Analyze up to this many commits into the past for code smells
   - FileExtensionFilter
     - Only analyze for code smells within these files
       - Currently only C# files are supported
4. Run GitDiff.exe
5. Results should be printed into the console and a CSV file should also been generated in the ResultsDirectory
