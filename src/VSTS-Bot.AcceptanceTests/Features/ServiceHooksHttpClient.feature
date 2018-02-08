Feature: ServiceHooksHttpClient
	Integration tests for the Service Hooks Http Client.

Background: 
	Given A user 'Test User'
	And I started a conversation
	And A clean state
	And Is authorized

@Integration
Scenario: Create and get service hook
	Given I Created a Service Hook for 'config:TeamProjectOne'
	Then I list the Service Hook
	Then I get the Service Hook
	And I delete the Service Hook
