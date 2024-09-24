Publish the project in your destination folder
either run the exe file or use sc command to create a service
- sc create WebsiteStatus binpath= c:/temp/workerservice/WebsiteStatus.exe start= auto
- sc delete WebsiteStatus 
voila its done
