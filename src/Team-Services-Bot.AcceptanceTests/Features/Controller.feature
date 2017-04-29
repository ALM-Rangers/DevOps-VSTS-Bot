Feature: Controller
	As a chat user
	I want to be able to invoke the Team Services Bot
	So that I can get it to help me with Team Services tasks

@Acceptance
Scenario: Invoke the root dialog
	Given I have a controller
	When I post a message activity to the controller
	Then the root dialog is invoked
	 And the activity is passed to the dialog
	 And I get a HTTP 200 response

@Acceptance
Scenario: Exception invoking the root dialog
	Given I have a controller
	  And There is a problem invoking the root dialog
	When I post a message activity to the controller
	Then I get a HTTP 500 response
