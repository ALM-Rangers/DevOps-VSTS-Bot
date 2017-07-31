Feature: Releases
	In order get new versions of our application to our environments we need to create a release.

Background: 
	Given A user 'Test User'
	And A clean state
	And Is authorized
	And I am connected to 'config:TeamProjectOne'
	And I started a conversation
	And I say 'Hi'

@Acceptance
Scenario: List Releases
	When I say 'releases'
	Then I get a list of release definitions
	| Name    |
	| Release 1 |

@Acceptance
Scenario: Create Release
	When I say 'releases'
	Then I get a list of release definitions
	| Name    |
	| Release 1 |
	When I say 'create 1'
	Then I get a created release response
	And A created release should exist on 'config:TeamProjectOne'