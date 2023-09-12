# ManualWorkStation_AR_Display

Code for the Unity project MWS_AR_Display.

Complete Unity projects including assets can be found here: https://fh-aachen.sciebo.de/apps/files/?dir=/Wollert/Projekte_Daten/3_Demonstratoren/HAP/Unity&fileid=463557605


## Steps to upload a new build

Build Unity project in a separate folder. Then, copy the content of the "Build" folder into the "Build" folder of the repository.


## Add a new product

### Prepare CAD file for Unity

The CAD files must fulfill the following properties to ensure the programmed functionalities:

- The coordinate system must be in the center of all parts which are mounted separately

- Holders which are used to assemble the product must be named "AssemblyHolder" in order to be recognized.

- All parts which are mounted require a unique name. If several same parts are used in the CAD (e.g. Screw_M5), they should be uniquely identified by using ":1", ":2", etc. This way, same parts can be identified in the storage (M5) and located in the whole assembly (Screw_M5:1, Screw_M5:2).

In order to import the CAD files in Unity, they need to be in the .fbx format.


### Include CAD file in Unity project

- Copy the CAD assembly in the folder **Prefabs -> ProductAssemblies** and adjust the CAD file if required (e.g. by adding colors)

- Create a new folder in **Prefabs -> Parts** with the Product-ID and move single parts for assembly there. Same parts which are used several times (e.g. Screw_M5:1, Screw_M5:2) only need to be there once (Screw_M5). It might be necessary to adjust the single parts (size & angle) to allow a good display during work. For testing purposes, move the single parts to the Gameobject "NextObjects" in the main project.

- Move the complete assembly in the main project under **Assemblies** with the Product-ID


### Highlight points with tool interactions

Points which require tool interactions can be highlighted with so called "Toolpoints". The prefab for this purpose can be found in **Prefabs -> General**.

Toolpoints are ignored when creating the miniature. In order to be identified, the name must start with "ToolPoint_" (see dog assembly).