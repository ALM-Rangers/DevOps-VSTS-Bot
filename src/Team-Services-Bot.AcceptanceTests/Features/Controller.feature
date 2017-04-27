Feature: Controller
	As a chat user
	I want to be able to invoke the Team Services Bot
	So that I can get it to help me with Team Services tasks

@Acceptance
Scenario: Invoke the root dialog
	Given I have a controller
	When I post a message activity to the controller
	Then the root dialog is invoked
