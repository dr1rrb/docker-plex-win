FROM mcr.microsoft.com/windows:1809

# Copy/download install files to container
COPY PlexSetup C:\\PlexSetup
WORKDIR C:\\PlexSetup
# Installer is pushed on this repo, so the docker build is reproductible, if needed you can uncomment this line to get it from the officiel server
# ADD https://downloads.plex.tv/plex-media-server-new/1.18.4.2171-ac2afe5f8/windows/PlexMediaServer-1.18.4.2171-ac2afe5f8-x86.exe PlexMediaServer-1.18.4.2171-ac2afe5f8-x86.exe

# Install Plex
RUN PlexMediaServer-1.18.4.2171-ac2afe5f8-x86.exe /quiet
RUN reg import Config.reg

# Cleanup
RUN del /F /Q PlexMediaServer-1.18.4.2171-ac2afe5f8-x86.exe
RUN del /F /Q Config.reg

# Expose images possible configuration
EXPOSE 32400/tcp
VOLUME C:\\Plex

# Define the entrypoint
CMD Run.cmd