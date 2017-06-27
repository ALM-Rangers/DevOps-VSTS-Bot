Feature: Connect
	In order to talk to Team Services, we need to be able to connect to Team Services

Background: 
	Given A user 'Test User'
	And A clean state
	And Is authorized
	And I started a conversation

@Acceptance
Scenario: Connect to a account and team project.
	When I connect to the account and 'config:TeamProjectOne'
	Then I am connected to the account and 'config:TeamProjectOne'