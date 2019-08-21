@echo off
echo Restore Plex configuration
"C:\PlexSetup\DockerRegistrySync\DockerRegistrySync.exe" "C:\Plex\PlexConfig.json" "HKEY_CURRENT_USER\Software\Plex, Inc.\Plex Media Server" --restore

If EXIST C:\Plex\Startup.cmd (
	echo Running custom startup script
	call C:\Plex\Startup.cmd
)

If EXIST C:\PlexSecrets\Startup.cmd (
	echo Running custom secured startup script
	call C:\PlexSecrets\Startup.cmd
)

echo Plex will be running on:
ipconfig | findstr /c:IPv
echo You can take ownership of a newly created server by navigating to one of this IP address on port 32400 using a web browser from a computer on the same local network

echo Starting Plex
start "Registry sync" "C:\PlexSetup\DockerRegistrySync\DockerRegistrySync.exe" "C:\Plex\PlexConfig.json" "HKEY_CURRENT_USER\Software\Plex, Inc.\Plex Media Server"
"C:\Program Files (x86)\Plex\Plex Media Server\Plex Media Server.exe"
