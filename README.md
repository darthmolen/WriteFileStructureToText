# WriteFileStructureToText

Use in automation to list the files that are in a build folder for documentation purpose. This will read recursively.

## Command Line Parameters

```/subdir:``` The sub directory folder name you want the program to read and recurse

```/rf:``` or ```/relativefile:``` the name of the relative file you want to save the relative file paths to. default=relative.txt

```/ff:``` or ```/fullfile:``` the name of the full path file you want to save the full paths of files to. default=full.txt

```/cf:``` or ```/combinedfile:``` the name of the json file that you want the program to output both the full and relative path to for each file. default=combined.json
