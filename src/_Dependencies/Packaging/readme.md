# 💥Plisky.Nuke.Fusion Readme.💥

## About

This is a support package for integration Plisky Code Craft tools with Nuke build engine.  It provides wrappers for Mollycoddle, Versonify and a Discord hook.  See the full documentation on the gitub pages https://itsey.github.io/version-index.html

## Key Features


## Usage.

See the full documentation on the gitub pages https://itsey.github.io/version-index.html
 
```csharp
var mc = new MollycoddleTasks();
mcs.SetRulesFile(/* Path to molly rules file*/);
mcs.SetPrimaryRoot(/* Path to primary root*/);
mcs.SetFormatter(/* Select Formatter */);
mcs.SetDirectory(/* Path to root folder */);

mc.PerformScan(mcs);
```
 
 ## Additional Documentation
 
 * Main Documentation: https://itsey.github.io/
 * Project Repository: https://github.com/Itsey/plisky-nuke-fusion

Longer term goal is to include this package within the main Nuke project.



