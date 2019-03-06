# UnityEditor AssemblyDefinition Creator for Editor folder
Tool that helps to create AssemblyDefinitions for Editor folders in UnityEditor.
### Migrating to custom AssemblyDefinitions in Unity Project
1. Create new AssemblyDefinition in Assets folder
2. Run script with params
    1. path to project Assets folder
    2. additional space-separated assembly references (probably the one you've just created)
3. Fix remaining compilation error caused by missing references in specific assembly (e.g. TextMeshPro not found)
