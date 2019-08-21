# $Env:PLEX_VERSION = "1.16.5.1488-deeb86e7f";
# $Env:PLEX_LATEST_VERSION = "1.16.5.1488-deeb86e7f";
# $Env:PLEX_LATEST_URL = "https://downloads.plex.tv/plex-media-server-new/1.16.5.1488-deeb86e7f/windows/PlexMediaServer-1.16.5.1488-deeb86e7f-x86.exe";

$version = $Env:PLEX_VERSION;
$url = $Env:PLEX_URL;
$latestVersion = $Env:PLEX_LATEST_VERSION;
$latestUrl = $Env:PLEX_LATEST_URL;
$tags = $Env:PLEX_TAGS;

# Capture all variables prefixed by PLEX
$variables = @{}
"Env:PLEX_*" | Get-Item | ForEach { $variables[$_.Name.ToString().Split('_', 2)[1].ToLower()] = $_.Value }

# Coalesce inputs
if ( $variables['version'] -like '') { $variables['version'] = $latestVersion }
if ( $variables['url'] -like '') { $variables['url'] = $latestUrl }
if ( $variables['tags'] -like '') { 
    if ($variables['version'] -like $latestVersion) { $variables['tags'] = 'latest' }
    else { $variables['tags'] = '' }
}
# Easy patch
$variables['installer'] = [System.IO.Path]::GetFileName($variables['url'])

# Set build variables (And dump to file for release pipeline)
$variables.keys | ForEach { "##vso[task.setvariable variable=plex."+$_+"]"+ $variables[$_] | Write-Host }
$variables.keys | ForEach { "##vso[task.setvariable variable=plex."+$_+"]"+ $variables[$_] } > "config.txt"

# Dump to ouput
$variables