﻿--UPDATE AspNetModules 
--  SET [Order] = 
--			CASE 
--				WHEN ModuleCode = 'm-dashboard' THEN 1
--				WHEN ModuleCode = 'm-accounts' THEN 2
--				WHEN ModuleCode = 'm-teams' THEN 3
--				WHEN ModuleCode = 'm-users' THEN 4 
--				WHEN ModuleCode = 'm-taskdashboard' THEN 5
--				WHEN ModuleCode = 'm-timekeeps' THEN 6
--				WHEN ModuleCode = 'm-syslogs' THEN 7
--				WHEN ModuleCode = 'm-permissions' THEN 8
--				WHEN ModuleCode = 'm-support' THEN 9

--				WHEN ParentModuleCode = 'm-timekeeps' THEN 6
--					 END,

--	SubModuleOrder = CASE 
--				WHEN ModuleCode = 's-management' THEN 1
--				WHEN ModuleCode = 's-report' THEN 2
--				WHEN ModuleCode = 's-attendance' THEN 3
--				WHEN ModuleCode = 's-eodedits' THEN 4
--				END
--GO