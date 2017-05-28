Feature: Welcome
	In order to make the user feel welcome. We provide a welcome message.

Background: 
	Given A user 'Test User'
	And A clean state

@Acceptance
Scenario: Starting a conversation for the first time with the bot.
	Given I started a conversation
	When I say 'Hi'
	Then the bot should respond with 'Welcome Test User. This is the first time we talk.'

@Acceptance
Scenario: Starting a conversation for the second time with the bot.
	Given I started a conversation
	And The user has previously logged in into the account and team project 'config:TeamProjectOne'
	When I say 'Hi'
	Then the bot should respond with 'Welcome back Test User. I have connected you to Account 'config:Account', Team Project 'config:TeamProjectOne'.'
