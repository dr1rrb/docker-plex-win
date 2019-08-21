# Plex in a Windows container
Run [Plex](https://plex.tv/) on a Windows container host.
Compared to the [Linux image](https://github.com/plexinc/pms-docker/blob/master/README.md), 
this gives you the ability to easily integrate with the [Windows Storage Spaces](https://docs.microsoft.com/en-us/windows-server/storage/storage-spaces/overview)
and easily includes SMB shares in your library (and even [DFS share](https://docs.microsoft.com/en-us/windows/win32/dfs/distributed-file-system-dfs-functions))

[![Build Status](https://dev.azure.com/dr1rrb/docker-plex-win/_apis/build/status/dr1rrb.docker-plex-win?branchName=master)](https://dev.azure.com/dr1rrb/docker-plex-win/_build/latest?definitionId=1&branchName=master)
![crawler](https://healthchecks.io/badge/5d6ba759-a8f6-471e-994e-0498930dd48c/GALn9f76/crawler.svg "crawler")

## Configuration
### Network
In order to select the streaming quality, Plex is analyzing the network to determine if your player devices are close to the server or not.
The easiest solution is to put the Plex container directly on your LAN. To do this on Windows, you have to use the `transparent`
network interface.

Usually an `transparent` network is already configured in docker for Windows. To determine its name, on you host, run the command:

```
C:\Docker\Podcasts>docker network ls
NETWORK ID          NAME                                                   DRIVER              SCOPE
48495b5114ea        Intel(R) Gigabit CT Desktop Adapter - Virtual Switch   transparent         local
4161e0d9839f        nat                                                    nat                 local
91b6269a26bd        none                                                   null                local
```

Choose a network which uses the `transparent` driver, and run the container with `--network "NETWORK_NAME"`.

For instance, here the dump of an `ipconfig` in a blank Windows container (the IP should be in the same range of your LAN):
```
C:\Docker>docker run --rm -i --network "Intel(R) Gigabit CT Desktop Adapter - Virtual Switch"  mcr.microsoft.com/windows:1809 cmd
Microsoft Windows [Version 10.0.17763.134]
(c) 2018 Microsoft Corporation. All rights reserved.

C:\>ipconfig
ipconfig

Windows IP Configuration


Ethernet adapter vEthernet (Ethernet) 4:

   Connection-specific DNS Suffix  . : 
   Link-local IPv6 Address . . . . . : fe80::b84b:baef:4e1f:cf22%63
   IPv4 Address. . . . . . . . . . . : 192.168.1.46
   Subnet Mask . . . . . . . . . . . : 255.255.255.0
   Default Gateway . . . . . . . . . : 192.168.1.1

C:\>exit
```

For the remote access, you have to [forward the 32400/TCP](https://support.plex.tv/articles/200931138-troubleshooting-remote-access/)
to your Plex instance. For this we recommand to fix the MAC address of your container using `--mac-address d0:c8:32:12:23:56`,
and then, on your router, setup a DHCP reservation and the port mapping.

### Volumes
* `C:\Plex`: Directory for the configuration of your plex instance

You can also add all your local media storages, like in the suggested docker file below.

## Run the container
### Command line
```
docker run -d -v C:\Docker\Plex:C:\Plex --network "Intel(R) Gigabit CT Desktop Adapter - Virtual Switch" --mac-address d0:c8:32:12:23:56 dr1rrb/plex-win
```

### Docker compose
Suggested docker compose file
```
version: '3.4'
services:
  plex:
    container_name: plex
    image: dr1rrb/plex-win
    restart: unless-stopped
    volumes:
      - C:\Docker\Plex:C:\Plex
      - C:\Storages\Photos:P::ro
	  # ./.
    security_opt: 
      - "credentialspec=file://plex.json" # Network identity, cf. TIPS below
    mac_address: d0:c8:32:12:34:56
    hostname: plex

networks:
  default:
    external:
      name: Intel(R) Gigabit CT Desktop Adapter - Virtual Switch
```

## TIPS: Running a Windows container on a AD joined host
Usually when running a Windows container, we need it to integrates with other servers and network infrastructure.
If you have an active directory (AD), it's pretty easy to give a valid network identity to your container.
You can find more info [here](https://docs.microsoft.com/en-us/virtualization/windowscontainers/manage-containers/manage-serviceaccounts)
and [here](https://artisticcheese.wordpress.com/2017/09/09/enabling-integrated-windows-authentication-in-windows-docker-container/).

## Crawler
A crawler is running every day to check if a new release of Plex is avaible for Windows and automatically build a new docker image for it. 

Here are the status of this crawler: ![crawler](https://healthchecks.io/badge/5d6ba759-a8f6-471e-994e-0498930dd48c/GALn9f76/crawler.svg "crawler")

