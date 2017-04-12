# Team Services Bot

![Build Status](https://almrangers.visualstudio.com/_apis/public/build/definitions/7f3cfb9a-d1cb-4e66-9d36-1af87b906fe9/143/badge)

## Introduction
**Team Services Bot** is Bot for Microsoft Teams, Skype and Slack, based on [Microsoft Bot Framework](https://dev.botframework.com/).

## Getting Started
1. Read the documention about [Microsoft Bot Framework](https://dev.botframework.com/).
2. Download the [Bot Framework Emulator](https://docs.botframework.com/en-us/tools/bot-framework-emulator/#navtitle) to help you test.
3. For the Acceptance tests:
   1. Download [ngrok](https://ngrok.com/download) to help run the Acceptance tests.
   2. Run the Team-Services-Bot.Api within Visual Studio, which will run on: http://localhost:3979/
   3. Run 
      ```
      ngrok http -host-header=rewrite 3979
      ```
    4. [Register a bot](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html#registering). Use the url from ngrok (Example: https://9fddfdcb.ngrok.io/api/messages) as the messaging endpoint.  
    Make sure you turn on Direct Line
    5. Create / Update src\Team-Services-Bot.runsettings
       ``` xml
       <?xml version="1.0" encoding="utf-8"?>
       <RunSettings>
         <TestRunParameters>
           <Parameter name="BotId" value="__Your-BOT-ID__" />
           <Parameter name="BotSecret" value="__Your-BOT-Secret__" />
         </TestRunParameters>
       </RunSettings>
       ```
    6. Create / Update src\Team-Services-Bot.Api\AppSettings.config
       ``` xml
       <?xml version="1.0" encoding="utf-8"?>
       <appSettings>
         <!-- update these with your BotId, Microsoft App Id and your Microsoft App Password-->
         <add key="BotId" value="__Your-BOT-ID__" />
         <add key="MicrosoftAppId" value="__Microsoft-APP-ID__" />
         <add key="MicrosoftAppPassword" value="__Microsoft-APP-PASSWORD__" />
       </appSettings>
       ```

## Build and Test
Open the **Team-Services-Bot.sln** in the [src](https://github.com/ALM-Rangers/Team-Services-Bot/tree/Master/src) folder. It contains the projects and tests, which you can run with the Test Explorer in Visual Studio.

## Contributors
We thank the following contributors for this extension: Robert Jarrett and Jeffrey Opdam.

## Contribute
Contributions to this project are welcome. Here is how you can contribute:  

- Submit bugs and help us verify fixes  
- Submit pull requests for bug fixes and features and discuss existing proposals   

Please refer to [Contribution guidelines](.github/CONTRIBUTING.md) and the [Code of Conduct](.github/COC.md) for more details.
