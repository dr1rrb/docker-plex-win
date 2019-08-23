FROM mcr.microsoft.com/windows:1809

# Copy/download install files to container
COPY PlexSetup C:\\PlexSetup
WORKDIR C:\\PlexSetup
# Installer is downloaded as artefact of the build process, so the docker build is reproductible
# ADD https://downloads.plex.tv/plex-media-server-new/1.15.3.876-ad6e39743/windows/PlexMediaServer-1.15.3.876-ad6e39743-x86.exe Setup.exe

# Install Plex
RUN {plex.installer} /quiet
RUN reg import Config.reg

# Cleanup
RUN del /F /Q {plex.installer}
RUN del /F /Q Config.reg

# Expose images possible configuration
EXPOSE 32400/tcp
VOLUME C:\\Plex

# Define the entrypoint
CMD Run.cmd