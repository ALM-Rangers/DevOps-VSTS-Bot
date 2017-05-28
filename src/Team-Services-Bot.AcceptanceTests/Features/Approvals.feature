Feature: Approvals
	In order to get software deployed the user needs to get a list of approvals appointed to them.
	And should be able to approve, reject or navigate to the release summary.

Background: 
	Given A user 'Test User'
	And A clean state
	And Is authorized
	And No approvals are waiting in 'config:TeamProjectOne'
	And No approvals are waiting in 'config:TeamProjectTwo'

@Acceptance @ignore
Scenario: List approvals
	Given I started 'Release 1' on 'config:TeamProjectOne'
	And   I started 'Release 1' on 'config:TeamProjectTwo'
	When I send a message 'approvals'
	Then I get a list of approvals
	| Team Project          | Release   | Environment |
	| config:TeamProjectOne | Release 1 | Development |
	| config:TeamProjectTwo | Release 1 | Development |

@Acceptance @ignore
Scenario: Approve approval
	Given I have an approval for 'config:TeamProjectOne', Release: 'Release 1'
	When I send a message 'approve config:ApprovalId  'A comment''
	Then 'config:ApprovalId' is approved with comment 'A comment'

@Acceptance @ignore
Scenario: Reject approval
	Given I have an approval for 'config:TeamProjectOne', Release: 'Release 1'
	When I send a message 'reject config:ApprovalId 'A comment''
	Then 'config:ApprovalId' is rejected with comment 'A comment'