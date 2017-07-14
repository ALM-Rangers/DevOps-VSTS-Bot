Feature: Builds
	In order to get artifacts that we can deploy, we first need to build and validate it.

Background: 
	Given A user 'Test User'
	And A clean state
	And Is authorized
	And I am connected to 'config:TeamProjectOne'
	And I started a conversation
	And I say 'Hi'

@Acceptance
Scenario: List Builds
	When I say 'builds'
	Then I get a list of build definitions
	| Name    |
	| Build 1 |

Scenario: List & Queue Build
	Given I say 'builds'
	And I get a list of build definitions
	| Name    |
	| Build 1 |
	When I say 'queue 1'
	Then A build with id '1' should exist on 'config:TeamProjectOne'