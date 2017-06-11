Feature: Welcome
	In order to make the user feel welcome. We provide a welcome message.

Background: 
	Given A user 'Test User'
	And A clean state

@Acceptance
Scenario: Starting a conversation for the first time with the bot.
	Given I started a conversation
	When I say 'Hi'
	Then the bot should respond with 'Hi Test User. Good to see you. I will help you with your Team Services tasks.'