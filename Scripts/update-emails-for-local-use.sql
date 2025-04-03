WITH updateData AS (
	SELECT Id, Email
	, SUBSTRING(Email, 1, CHARINDEX('@', Email)) + 'getnada.com' AS modifiedEmail 
	FROM dbo.AspNetUsers WHERE Email <> 'jonathan@ilucent.com'
)

UPDATE au SET au.Email = u.modifiedEmail
FROM dbo.AspNetUsers au
INNER JOIN updateData u ON au.Id = u.Id

