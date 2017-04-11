Feature: Echo
	Feature file to test if the basic echo works.
	Will be thrown away as soon as the project matures, 
	But captures the basic test flow.

@mytag
Scenario: Echo
	Given I have a client
	When I send a message 'Test'
	Then I should receive a response 'You sent Test which was 4 characters'
