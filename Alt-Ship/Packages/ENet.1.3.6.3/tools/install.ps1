param($installPath, $toolsPath, $package, $project)

$x86Dll = $project.ProjectItems.Item("ENetX86.dll")
$x86Dll.Properties.Item("BuildAction").Value = 0
$x86Dll.Properties.Item("CopyToOutputDirectory").Value = 1

$x64Dll = $project.ProjectItems.Item("ENetX64.dll")
$x64Dll.Properties.Item("BuildAction").Value = 0
$x64Dll.Properties.Item("CopyToOutputDirectory").Value = 1

$macDll = $project.ProjectItems.Item("libenet.dylib")
$macDll.Properties.Item("BuildAction").Value = 0
$macDll.Properties.Item("CopyToOutputDirectory").Value = 1
