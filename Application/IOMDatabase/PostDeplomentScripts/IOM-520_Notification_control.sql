/*
Post-Deployment Script Template
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.        
 Use SQLCMD syntax to include a file in the post-deployment script.            
 Example:      :r .\myfile.sql                                
 Use SQLCMD syntax to reference a variable in the post-deployment script.        
 Example:      :setvar TableName MyTable
               SELECT * FROM [$(TableName)]                    
--------------------------------------------------------------------------------------
*/

WITH insertData AS (
    SELECT 'Change Password' AS [Name], 'ChangePassword' As [Action], 'Password Reset Successful.' As [Subject], 'Email' AS [Type],
'Hello <b>{user.Name}</b>,
<br />
<br />You successfully reset your password.
<br />
<br />Thank You' AS [Message]
	UNION
	SELECT 'Reset Password' AS [Name], 'ResetPassword' As [Action], 'Password Reset Successful' As [Subject], 'Email' AS [Type],
'Hello <b>{user.Name}</b>,
<br />
<br />You successfully reset your password.
<br />
<br />Thank You' AS [Message]
	UNION
	SELECT 'Reset Password Request' AS [Name], 'ResetPasswordRequest' As [Action], 'iLucent Password Reset' As [Subject], 'Email' AS [Type],
'To reset your password, please click the link below.
<br />
<br /><a href="{callbackUrl}">Reset</a>
<br />
<br />This link is valid only for {hours} hours, if you don''t reset password it will automatically cancel the request.
<br />
<br />Thanks!' AS [Message]
	UNION
	SELECT 'Delete User' AS [Name], 'DeleteInactiveUser' As [Action], 'Thank you from iLucent' As [Subject], 'Email' AS [Type],
'Hello <b>{user.Name}</b>,
<br />
<br />Your account has been deleted.
<br />
<br />Thank you for being a part of iLucent.' AS [Message]
	UNION
	SELECT 'Delete User (Admin)' AS [Name], 'DeleteInactiveUserAdmin' As [Action], 'iLucent [Deleted {0}]' As [Subject], 'Email' AS [Type],
'Hello <b>{recipient}</b>,
<br />
<br />A user account has been deleted with the following information:
<br />
<br />Name: {deletedName}
<br />Email: {deletedEmail}
<br />Role: {deleteRole}' AS [Message]
	UNION
	SELECT 'Add User' AS [Name], 'AddUser' As [Action], 'Welcome New User to iLucent Operations Manager' As [Subject], 'Email' AS [Type],
'Hi <b>{userDetail.FirstName}</b>,
<br/>
<br/>Welcome to the iLucent Operations Manager System.
<br/>
<br/>You may access the system with the following details:
<br/>URL: <a href=''{urlLink}''>{urlHost}</a>
<br/>Username: <b>{user.Email}</b>
<br/>{tempStart}Temporary Password: <b>{password}</b>
<br/>{tempEnd}
<br/>For security purposes, please change your password once you''ve logged in.
<br/>
<br/>Thanks,
<br/>
<br/>System Administrator' AS [Message]
	UNION
	SELECT 'Add User (Admins)' AS [Name], 'AddUserAdmin' As [Action], 'iLucent Operations Manager - New {0} Created' As [Subject], 'Email' AS [Type],
'A user account has been registered with the following information:
<br />
<br />Name: <b>{userDetail.Name}</b>
<br />Email: {email}
<br />Role: {roleName}' AS [Message]
)

INSERT INTO dbo.NotificationSetting([Name], [Action], [Subject], [Type], [Message])
SELECT [Name], [Action], [Subject], [Type], [Message] FROM insertData i
WHERE 
	NOT EXISTS(SELECT 1 FROM dbo.NotificationSetting 
				WHERE [Action] = i.[Action] AND [Name] = i.[Name]);


WITH insertData AS (
	SELECT 1 AS NotificationSettingsId, '6e5882b4-64a0-4331-85fb-d531cd1ca23c' AS RoleId
	UNION
	SELECT 2 AS NotificationSettingsId, '6e5882b4-64a0-4331-85fb-d531cd1ca23c' AS RoleId
	UNION
	SELECT 3 AS NotificationSettingsId, '6e5882b4-64a0-4331-85fb-d531cd1ca23c' AS RoleId
	UNION
	SELECT 4 AS NotificationSettingsId, '6e5882b4-64a0-4331-85fb-d531cd1ca23c' AS RoleId
	UNION
	SELECT 5 AS NotificationSettingsId, 'ba7476a9-437d-4a2d-8907-b404f56bc046' AS RoleId
	UNION
	SELECT 6 AS NotificationSettingsId, '6e5882b4-64a0-4331-85fb-d531cd1ca23c' AS RoleId
	UNION
	SELECT 7 AS NotificationSettingsId, 'ba7476a9-437d-4a2d-8907-b404f56bc046' AS RoleId
)

INSERT INTO dbo.NotificationRecipientRole(NotificationSettingId, RoleId)
SELECT NotificationSettingsId, RoleId FROM insertData i
WHERE 
	NOT EXISTS(SELECT 1 FROM dbo.NotificationRecipientRole 
				WHERE NotificationSettingId = i.NotificationSettingsId AND RoleId = i.RoleId)
