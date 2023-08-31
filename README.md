# ManualWorkStation_AR_Display

Code for the Unity project MWS_AR_Display.

Complete Unity projects including assets can be found here: https://fh-aachen.sciebo.de/apps/files/?dir=/Wollert/Projekte_Daten/3_Demonstratoren/HAP/Unity&fileid=463557605


## Steps to upload a new build

Build Unity project in a separate folder. Then, copy the content of the "Build" folder into the "Build" folder of the repository.

## Add a new product

### Prepare CAD file for Unity

The CAD files must fulfill the following properties to ensure the programmed functionalities:

- Holders which are used to assemble the product must be named "AssemblyHolder" in order to be recognized.



In order to import the CAD files in Unity, they need to be in the .fbx format.

### Include CAD file in Unity project

- Copy the CAD assembly in the folder **Prefabs -> ProductAssemblies** and adjust the CAD file if required (e.g.
by adding colors)

- Create a new folder in **Prefabs -> Parts** with the Product-ID and move single parts for assembly there

- Create an empty GameObject in the main project under **Assemblies** with the Product-ID


### Highlight points with tool interactions

Points which require tool interactions can be highlighted with so called "Toolpoints". The prefab for this purpose can be found in **Prefabs -> General**.

Toolpoints are ignored when creating the miniature. In order to be identified, the name must start with "ToolPoint_" (see dog assembly).

- 