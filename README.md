# VSTS-Bot 

![Build Status](https://almrangers.visualstudio.com/_apis/public/build/definitions/7f3cfb9a-d1cb-4e66-9d36-1af87b906fe9/143/badge)

## Introduction
**ALM | DevOps Rangers community Bot** is a Bot for Microsoft Teams, Skype and Slack, based on [Microsoft Bot Framework](https://dev.botframework.com/). Read [VSTS Bot is now in preview](https://blogs.msdn.microsoft.com/visualstudioalmrangers/2017/10/12/visual-studio-team-services-bot-vsts-bot-is-now-in-preview/) for details.

## Getting Started
1. Read the documention about [Microsoft Bot Framework](https://dev.botframework.com/).
2. Download the [Bot Framework Emulator](https://docs.botframework.com/en-us/tools/bot-framework-emulator/#navtitle) to help you test.
3. Download the [Cosmos DB](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator) which we emulate Cosmos DB on your local machine.
3. Run Git-Ignore-Changes.bat which will disable tracking of Team-Services-Bot.runsettings and AppSettings.config, so your local information will not get uploaded to the repository.
4. For the Acceptance tests:
   1. Download [ngrok](https://ngrok.com/download) to help run the Acceptance tests.
   2. Run the Team-Services-Bot.Api within Visual Studio, which will run on: http://localhost:3979/
   3. Run 
      ```
      ngrok http -host-header=rewrite 3979
      ```
    4. [Register a bot](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#registering). Use the url from ngrok (Example: https://9fddfdcb.ngrok.io/api/messages) as the messaging endpoint.  
    Make sure you turn on Direct Line
    5. Register the application with vsts https://app.vsaex.visualstudio.com/app/register?mkt=en-US.
    6. Create / Update src\Team-Services-Bot.runsettings (description can be found on the [wiki](https://github.com/ALM-Rangers/Team-Services-Bot/wiki/Team-Services-Bot.runsettings)).
       ``` xml
       <?xml version="1.0" encoding="utf-8"?>
       <RunSettings>
         <TestRunParameters>
           <Parameter name="Account" value="" />
           <Parameter name="AppId" value="" />
           <Parameter name="AppSecret" value="" />
           <Parameter name="AuthorizeUrl" value="" />
           <Parameter name="BotId" value="" />
           <Parameter name="BotSecret" value="" />
           <Parameter name="MicrosoftApplicationId" value="" />
           <Parameter name="MicrosoftApplicationPassword" value="" />
           <Parameter name="RefreshToken" value="" />
           <Parameter name="RefreshTokenReinitialize" value="false" />
           <Parameter name="TeamProjectOne" value="" />
           <Parameter name="TeamProjectTwo" value="" />
         </TestRunParameters>
       </RunSettings>
       ```
    7. Create / Update src\Team-Services-Bot.Api\AppSettings.config (description can be found on the [wiki](https://github.com/ALM-Rangers/Team-Services-Bot/wiki/AppSettings)).
       ``` xml
       <?xml version="1.0" encoding="utf-8"?>
       <appSettings>
         <!-- update these with your BotId, Microsoft App Id and your Microsoft App Password-->
         <add key="AppId" value="" />
         <add key="AppSecret" value="" />
         <add key="AppScope" value=""/>
         <add key="AuthorizeUrl" value="" />
         <add key="DocumentDbKey" value="C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==" />
         <add key="DocumentDbUri" value="https://localhost:8081" />
         <add key="EmulatorListeningUrl" value="" />
         <add key="InstrumentationKey" value="" />
         <add key="BotId" value="" />
         <add key="MicrosoftAppId" value="" />
         <add key="MicrosoftAppPassword" value="" />
       </appSettings>
       ```

## Build and Test
Open the **Team-Services-Bot.sln** in the [src](https://github.com/ALM-Rangers/Team-Services-Bot/tree/Master/src) folder. It contains the projects and tests, which you can run with the Test Explorer in Visual Studio.

## Contributors
We thank the following contributors for this extension: Vladimir Gusarov and Jeffrey Opdam.

## Contribute
Contributions to this project are welcome. Here is how you can contribute:  

- Submit bugs and help us verify fixes  
- Submit pull requests for bug fixes and features and discuss existing proposals   

Please refer to [Contribution guidelines](.github/CONTRIBUTING.md) and the [Code of Conduct](.github/COC.md) for more details.

## Notices
Notices for certain third party software included in this solution are provided here: [Third Party Notice](ThirdPartyNotices.txt).
