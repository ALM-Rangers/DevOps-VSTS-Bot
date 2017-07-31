Feature: Approvals
	In order to get software deployed the user needs to get a list of approvals appointed to them.
	And should be able to approve, reject or navigate to the release summary.

Background: 
	Given A user 'Test User'
	And A clean state
	And Is authorized
	And I am connected to 'config:TeamProjectOne'
	And I started a conversation
	And I say 'Hi'
	And No approvals are waiting in 'config:TeamProjectOne'

@Acceptance
Scenario: List approvals
	Given I started release '1' on 'config:TeamProjectOne'
	When I say 'approvals'
	Then I get a list of approvals
	| Release Definition | Environment |
	| Release 1          | Development |

@Acceptance
Scenario: Approve approval
	Given I started release '1' on 'config:TeamProjectOne'
	And I have an approval for 'config:TeamProjectOne', Release: '1'
	And I say 'approvals'
	When I approve the approval with comment 'A comment'
	Then the approval is approved for 'config:TeamProjectOne'

@Acceptance
Scenario: Reject approval
	Given I started release '1' on 'config:TeamProjectOne'
	And I have an approval for 'config:TeamProjectOne', Release: '1'
	And I say 'approvals'
	When I reject the approval with comment 'A comment'
	Then the approval is rejected for 'config:TeamProjectOne'