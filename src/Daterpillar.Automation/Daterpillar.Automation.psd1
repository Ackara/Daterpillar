#
# Module manifest for module 'PSGet_Daterpillar.Automation'
#
# Generated by: Ackara
#
# Generated on: 6/8/2017
#

@{

# Script module or binary module file associated with this manifest.
RootModule = 'Acklann.Daterpillar.Automation.dll'

# Version number of this module.
ModuleVersion = '4.6.8'

# Supported PSEditions
# CompatiblePSEditions = @()

# ID used to uniquely identify this module
GUID = '029d5970-92a0-4c44-81ea-53f9df411f7c'

# Author of this module
Author = 'Ackara'

# Company or vendor of this module
CompanyName = 'Ackara'

# Copyright statement for this module
Copyright = 'licensed under MIT License'

# Description of the functionality provided by this module
Description = 'Daterpillar is a build automation tool and miro-orm.
'

# Minimum version of the Windows PowerShell engine required by this module
PowerShellVersion = '5.0'

# Name of the Windows PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
DotNetFrameworkVersion = '4.6'

# Minimum version of the common language runtime (CLR) required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
CLRVersion = '4.0'

# Processor architecture (None, X86, Amd64) required by this module
# ProcessorArchitecture = ''

# Modules that must be imported into the global environment prior to importing this module
# RequiredModules = @()

# Assemblies that must be loaded prior to importing this module
# RequiredAssemblies = @()

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
# ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
# FormatsToProcess = @()

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
# NestedModules = @()

# Functions to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no functions to export.
FunctionsToExport = '*'

# Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
CmdletsToExport = '*'

# Variables to export from this module
# VariablesToExport = @()

# Aliases to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no aliases to export.
AliasesToExport = @()

# DSC resources to export from this module
# DscResourcesToExport = @()

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
# FileList = @()

# Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
PrivateData = @{

    PSData = @{

        # Tags applied to this module. These help with module discovery in online galleries.
        Tags = 'orm','miroc-orm','sql','t4','build','automation'

        # A URL to the license for this module.
        LicenseUri = 'https://github.com/Ackara/Daterpillar/blob/master/LICENSE'

        # A URL to the main website for this project.
        ProjectUri = 'https://github.com/Ackara/Daterpillar'

        # A URL to an icon representing this module.
        IconUri = 'http://static.gigobyte.com/images/daterpillar.png'

        # ReleaseNotes of this module
        ReleaseNotes = 'version 4.6.1
-------------
* Rename namespace to Acklann


version 4.6.0
-------------
* Add LINQ capabilities


version 4.5.0
-------------
* Add Import-SQLData cmdlet
* Add Invoke-SQLCommand cmdlet


version 4.4.0
-------------
* Add Add-Database cmdlet
* Add Remove-Database cmdlet


version 4.3.0
-------------
* Add ConvertTo-Schema cmdlet
* Add ConvertTo-Script cmdlet
* Add New-MigrationScript cmdlet


version 4.2.0
-------------
* add ConvertTo-Script cmdlet


version 4.1.0
-------------
* Add msbuild targets'

        # External dependent modules of this module
        # ExternalModuleDependencies = ''

    } # End of PSData hashtable
    
 } # End of PrivateData hashtable

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}

