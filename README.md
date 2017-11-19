# OPSM: Order Preserving Submatrix
Greedy OPSM algorithm implementation

Note: The code style & performance is cataclysmic - I wrote it in 2003-2004 as one of my first works in C# and it looks and feels that way.

## Input:
Tab delimitted ASCII file. Rows are tissues, columns are gene expressions. A row may contain a header column & their could be a header row. In ANY case the number of columns should be equal on all rows.

A header row & column may contain any string, all other values must contain numbers.

Matrix size is T (tissues) x G (gene expressions).

## Algorithms & flags
The algorithm is performing DFS vertical compare, which mean two gene expressions are compared.
- "Use in-core dual compare" - pre-build all pairs of expressions vectors, this would take GxGxT size in memory so make sure it can fit the memory.
- "Support" - The number of tissues that contains a pattern. Set the application support for this minimal number of tissues you want to contain the pattern you are seeking. Setting a very high number will yield very few patterns, very low number - and the mining might not finish (exponencial number of patterns).
- "Min Length", "Max Length" - The patterns size you are looking for. Set "Min Length" so the application will not return very short and not interesting patterns. Set "Max Length" to reduce running time.
- "Max Errors" - Only for OPSM with Errors. OPSM searches for exact patterns. OPSM with Errors count a tissue even if it contains an X number of genes that does not fit the pattern.
- "Max Group Len" - for OPSM with groups, the pattern is compared between groups of genes instead of single genes. For example the pattern will be: (0.4 0.6 0.4) (0.7 0.8) (0.9 0.95)
- "Max Layer Diff" - Don't remember...

## How to use OPSM with Layers algorithm
When setting to layers there should be a layers definition file with
the same name as the dataset file, appended with ".layers".
For example:
SaltDat.txt
SaltDat.txt.layers

File format - each line represent a layer. Each layer is a range of columns:
0-5
6-10
...
...
