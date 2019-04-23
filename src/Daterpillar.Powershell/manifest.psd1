#
# Module manifest for module 'PSGet_manifest'
#
# Generated by: Ackara
#
# Generated on: 4/23/2019
#

@{

# Script module or binary module file associated with this manifest.
RootModule = 'Daterpillar.Powershell.dll'

# Version number of this module.
ModuleVersion = '8.0.0'

# Supported PSEditions
# CompatiblePSEditions = @()

# ID used to uniquely identify this module
GUID = '9e642b2b-66b3-4784-b04d-49352c283f8f'

# Author of this module
Author = 'Ackara'

# Company or vendor of this module
CompanyName = 'Ackara'

# Copyright statement for this module
Copyright = 'Copyright � 2015 Ackara'

# Description of the functionality provided by this module
Description = 'A tool that generates sql-migration scripts from your classes.'

# Minimum version of the Windows PowerShell engine required by this module
# PowerShellVersion = ''

# Name of the Windows PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
# DotNetFrameworkVersion = ''

# Minimum version of the common language runtime (CLR) required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
# CLRVersion = ''

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
FunctionsToExport = @()

# Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
CmdletsToExport = '*'

# Variables to export from this module
VariablesToExport = '*'

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
        Tags = 'sql','migration','orm','flyway','build'

        # A URL to the license for this module.
        LicenseUri = 'https://github.com/Ackara/Daterpillar/blob/master/license.txt'

        # A URL to the main website for this project.
        ProjectUri = 'https://github.com/Ackara/Daterpillar'

        # A URL to an icon representing this module.
        IconUri = 'https://raw.githubusercontent.com/Ackara/Daterpillar/master/art/icon.png'

        # ReleaseNotes of this module
        ReleaseNotes = 'https://github.com/Ackara/Daterpillar/blob/master/releaseNotes.txt'

        # Prerelease string of this module
        # Prerelease = ''

        # Flag to indicate whether the module requires explicit user acceptance for install/update
        # RequireLicenseAcceptance = $false

        # External dependent modules of this module
        # ExternalModuleDependencies = @()

    } # End of PSData hashtable

 } # End of PrivateData hashtable

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}

