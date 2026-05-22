# Plisky.Nuke.Fusion

## Why

This is a dotnet tool which serves as a wrapper around the tools that support build pipelines in the plisky family of tools.  It is there to make working with them in nuke simpler.



## Release Notes

### 0.3.9

Note 0.3.9 had the issues where the major number could not increment the pre-release therefore failed builds bumped the number so 3.6/3.7/3.8 were not real releases but failed builds. 

Added .net 10 into the package but no functional changes, this should do a full release with the features required to support the non zero exit code.

### 0.3.6 

#### Pre0.1

There was a change made to versonify which meant that failed file updates triggered a non zero exit code which the build treated as a failure.  When using the two version file approach to pre-release versioning the current pipelines do a dummy update to trigger updating the version number in the vstore for the pre-release version.   This caused all release pipelines to fail as it was not possible to do a release version and pass the pipeline.  

As a quick fix this version was released to add the zero return code support into PNF to allow a release version to take place.  

### 0.3.4

This version is unlisted, it is not know why it is unlisted.  

### 0.3

Fix added to support Nuke 9.0.4 multiple pre-releases done.

### 0.2.0 

Support added for Versonify and MollyCoddle



### Previous

Prior versions not captured.

