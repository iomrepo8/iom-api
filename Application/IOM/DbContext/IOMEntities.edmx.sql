
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/14/2023 18:42:44
-- Generated from EDMX file: C:\Users\Asus\ASPNET core projects 2.0\IOM-api\Application\IOM\DbContext\IOMEntities.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [IOMEU-uat];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AccountMember_Account]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AccountMember] DROP CONSTRAINT [FK_AccountMember_Account];
GO
IF OBJECT_ID(N'[dbo].[FK_AccountMember_UserDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AccountMember] DROP CONSTRAINT [FK_AccountMember_UserDetails];
GO
IF OBJECT_ID(N'[dbo].[FK_Attendance_UserDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Attendance] DROP CONSTRAINT [FK_Attendance_UserDetails];
GO
IF OBJECT_ID(N'[dbo].[FK_AttendanceRow_ToAttendance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AttendanceRow] DROP CONSTRAINT [FK_AttendanceRow_ToAttendance];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_IOMTeamTask_IOMTasks]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[IOMTeamTask] DROP CONSTRAINT [FK_IOMTeamTask_IOMTasks];
GO
IF OBJECT_ID(N'[dbo].[FK_IOMTeamTask_Teams]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[IOMTeamTask] DROP CONSTRAINT [FK_IOMTeamTask_Teams];
GO
IF OBJECT_ID(N'[dbo].[FK_Notification_Sender]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Notification] DROP CONSTRAINT [FK_Notification_Sender];
GO
IF OBJECT_ID(N'[dbo].[FK_Notification_UserDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Notification] DROP CONSTRAINT [FK_Notification_UserDetails];
GO
IF OBJECT_ID(N'[dbo].[FK_NotificationRecipientRole_NotificationSetting]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NotificationRecipientRole] DROP CONSTRAINT [FK_NotificationRecipientRole_NotificationSetting];
GO
IF OBJECT_ID(N'[dbo].[FK_NotificationRecipientRole_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NotificationRecipientRole] DROP CONSTRAINT [FK_NotificationRecipientRole_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_RolePermissions_AspNetModules]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RolePermissions] DROP CONSTRAINT [FK_RolePermissions_AspNetModules];
GO
IF OBJECT_ID(N'[dbo].[FK_RolePermissions_ToAspNetRoles]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RolePermissions] DROP CONSTRAINT [FK_RolePermissions_ToAspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskGroupItems_IOMTasks]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskGroupItems] DROP CONSTRAINT [FK_TaskGroupItems_IOMTasks];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskGroupItems_TaskGroups]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskGroupItems] DROP CONSTRAINT [FK_TaskGroupItems_TaskGroups];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskHistory_ToTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskHistory] DROP CONSTRAINT [FK_TaskHistory_ToTable];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskHistory_ToTable_1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskHistory] DROP CONSTRAINT [FK_TaskHistory_ToTable_1];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamClientPOC_Team]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamClientPOC] DROP CONSTRAINT [FK_TeamClientPOC_Team];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamClientPOC_UserDetail]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamClientPOC] DROP CONSTRAINT [FK_TeamClientPOC_UserDetail];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamDayOff_Teams]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamDayOff] DROP CONSTRAINT [FK_TeamDayOff_Teams];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamHolidays_Teams]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamHolidays] DROP CONSTRAINT [FK_TeamHolidays_Teams];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamMember_Teams]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamMember] DROP CONSTRAINT [FK_TeamMember_Teams];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamMember_UserDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamMember] DROP CONSTRAINT [FK_TeamMember_UserDetails];
GO
IF OBJECT_ID(N'[dbo].[FK_Teams_Location]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Teams] DROP CONSTRAINT [FK_Teams_Location];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamTaskGroup_TaskGroups]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamTaskGroup] DROP CONSTRAINT [FK_TeamTaskGroup_TaskGroups];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamTaskGroup_Teams]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamTaskGroup] DROP CONSTRAINT [FK_TeamTaskGroup_Teams];
GO
IF OBJECT_ID(N'[dbo].[FK_UserDayOff_ToTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserDayOff] DROP CONSTRAINT [FK_UserDayOff_ToTable];
GO
IF OBJECT_ID(N'[dbo].[FK_UserDetails_Location]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserDetails] DROP CONSTRAINT [FK_UserDetails_Location];
GO
IF OBJECT_ID(N'[dbo].[FK_UserImageFile_ToTable]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserImageFile] DROP CONSTRAINT [FK_UserImageFile_ToTable];
GO
IF OBJECT_ID(N'[dbo].[FK_UserNotificationSetting_NotificationType]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserNotificationSetting] DROP CONSTRAINT [FK_UserNotificationSetting_NotificationType];
GO
IF OBJECT_ID(N'[dbo].[FK_UserNotificationSetting_UserDetail]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserNotificationSetting] DROP CONSTRAINT [FK_UserNotificationSetting_UserDetail];
GO
IF OBJECT_ID(N'[dbo].[FK_UserShiftDetail_UserDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserShiftDetail] DROP CONSTRAINT [FK_UserShiftDetail_UserDetails];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTag_Tag]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserTag] DROP CONSTRAINT [FK_UserTag_Tag];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTag_UserDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserTag] DROP CONSTRAINT [FK_UserTag_UserDetails];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTask_AspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserTask] DROP CONSTRAINT [FK_UserTask_AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTask_IOMTasks]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserTask] DROP CONSTRAINT [FK_UserTask_IOMTasks];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTask_TaskGroups]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserTask] DROP CONSTRAINT [FK_UserTask_TaskGroups];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTask_Teams]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserTask] DROP CONSTRAINT [FK_UserTask_Teams];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTaskGroup_AspNetUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserTaskGroup] DROP CONSTRAINT [FK_UserTaskGroup_AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[FK_UserTaskGroup_TaskGroups]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserTaskGroup] DROP CONSTRAINT [FK_UserTaskGroup_TaskGroups];
GO
IF OBJECT_ID(N'[dbo].[FK_UserWorkDay_UserDetails]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserWorkDay] DROP CONSTRAINT [FK_UserWorkDay_UserDetails];
GO
IF OBJECT_ID(N'[dbo].[FK_Teams_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Teams] DROP CONSTRAINT [FK_Teams_fk];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamSupervisors_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamSupervisors] DROP CONSTRAINT [FK_TeamSupervisors_fk];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamSupervisors_fk2]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamSupervisors] DROP CONSTRAINT [FK_TeamSupervisors_fk2];
GO
IF OBJECT_ID(N'[dbo].[FK_TeamTask_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TeamTasks] DROP CONSTRAINT [FK_TeamTask_fk];
GO
IF OBJECT_ID(N'[dbo].[FK_UserDetailModels_fk]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserDetails] DROP CONSTRAINT [FK_UserDetailModels_fk];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[__RefactorLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[__RefactorLog];
GO
IF OBJECT_ID(N'[dbo].[AccountMember]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AccountMember];
GO
IF OBJECT_ID(N'[dbo].[Accounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Accounts];
GO
IF OBJECT_ID(N'[dbo].[AspNetModules]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetModules];
GO
IF OBJECT_ID(N'[dbo].[AspNetRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserClaims]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserClaims];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserLogins]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserLogins];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserPermission]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserPermission];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[Attendance]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Attendance];
GO
IF OBJECT_ID(N'[dbo].[AttendanceOTUpdates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AttendanceOTUpdates];
GO
IF OBJECT_ID(N'[dbo].[AttendanceRow]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AttendanceRow];
GO
IF OBJECT_ID(N'[dbo].[AttendanceStatusOptions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AttendanceStatusOptions];
GO
IF OBJECT_ID(N'[dbo].[AttendanceStatusUpdates]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AttendanceStatusUpdates];
GO
IF OBJECT_ID(N'[dbo].[EmployeeShift]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmployeeShift];
GO
IF OBJECT_ID(N'[dbo].[EmployeeStatus]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EmployeeStatus];
GO
IF OBJECT_ID(N'[dbo].[EODReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EODReports];
GO
IF OBJECT_ID(N'[dbo].[EODTaskItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EODTaskItems];
GO
IF OBJECT_ID(N'[dbo].[IOMTasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[IOMTasks];
GO
IF OBJECT_ID(N'[dbo].[IOMTeamTask]', 'U') IS NOT NULL
    DROP TABLE [dbo].[IOMTeamTask];
GO
IF OBJECT_ID(N'[dbo].[IpWhitelist]', 'U') IS NOT NULL
    DROP TABLE [dbo].[IpWhitelist];
GO
IF OBJECT_ID(N'[dbo].[Location]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Location];
GO
IF OBJECT_ID(N'[dbo].[Notification]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Notification];
GO
IF OBJECT_ID(N'[dbo].[NotificationRecipientRole]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NotificationRecipientRole];
GO
IF OBJECT_ID(N'[dbo].[NotificationSetting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NotificationSetting];
GO
IF OBJECT_ID(N'[dbo].[NotificationType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NotificationType];
GO
IF OBJECT_ID(N'[dbo].[RolePermissions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RolePermissions];
GO
IF OBJECT_ID(N'[dbo].[Seat]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Seat];
GO
IF OBJECT_ID(N'[dbo].[SystemLog]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SystemLog];
GO
IF OBJECT_ID(N'[dbo].[Tag]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tag];
GO
IF OBJECT_ID(N'[dbo].[TaskComment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskComment];
GO
IF OBJECT_ID(N'[dbo].[TaskGroupItems]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskGroupItems];
GO
IF OBJECT_ID(N'[dbo].[TaskGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskGroups];
GO
IF OBJECT_ID(N'[dbo].[TaskHistory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskHistory];
GO
IF OBJECT_ID(N'[dbo].[TaskHistoryType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskHistoryType];
GO
IF OBJECT_ID(N'[dbo].[TeamClientPOC]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TeamClientPOC];
GO
IF OBJECT_ID(N'[dbo].[TeamDayOff]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TeamDayOff];
GO
IF OBJECT_ID(N'[dbo].[TeamHolidays]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TeamHolidays];
GO
IF OBJECT_ID(N'[dbo].[TeamManager]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TeamManager];
GO
IF OBJECT_ID(N'[dbo].[TeamMember]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TeamMember];
GO
IF OBJECT_ID(N'[dbo].[Teams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Teams];
GO
IF OBJECT_ID(N'[dbo].[TeamSupervisors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TeamSupervisors];
GO
IF OBJECT_ID(N'[dbo].[TeamTaskGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TeamTaskGroup];
GO
IF OBJECT_ID(N'[dbo].[TeamTasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TeamTasks];
GO
IF OBJECT_ID(N'[dbo].[TimeZone]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TimeZone];
GO
IF OBJECT_ID(N'[dbo].[UserDayOff]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserDayOff];
GO
IF OBJECT_ID(N'[dbo].[UserDetails]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserDetails];
GO
IF OBJECT_ID(N'[dbo].[UserImageFile]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserImageFile];
GO
IF OBJECT_ID(N'[dbo].[UserNotificationSetting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserNotificationSetting];
GO
IF OBJECT_ID(N'[dbo].[UserShiftDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserShiftDetail];
GO
IF OBJECT_ID(N'[dbo].[UserTag]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserTag];
GO
IF OBJECT_ID(N'[dbo].[UserTask]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserTask];
GO
IF OBJECT_ID(N'[dbo].[UserTaskGroup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserTaskGroup];
GO
IF OBJECT_ID(N'[dbo].[UserTaskNotifications]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserTaskNotifications];
GO
IF OBJECT_ID(N'[dbo].[UserWorkDay]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserWorkDay];
GO
IF OBJECT_ID(N'[IOMDbContextStoreContainer].[vw_ActiveUsers]', 'U') IS NOT NULL
    DROP TABLE [IOMDbContextStoreContainer].[vw_ActiveUsers];
GO
IF OBJECT_ID(N'[IOMDbContextStoreContainer].[vw_AgentTeamSupervisors]', 'U') IS NOT NULL
    DROP TABLE [IOMDbContextStoreContainer].[vw_AgentTeamSupervisors];
GO
IF OBJECT_ID(N'[IOMDbContextStoreContainer].[vw_AllUsers]', 'U') IS NOT NULL
    DROP TABLE [IOMDbContextStoreContainer].[vw_AllUsers];
GO
IF OBJECT_ID(N'[IOMDbContextStoreContainer].[vw_AttendanceDefaultView]', 'U') IS NOT NULL
    DROP TABLE [IOMDbContextStoreContainer].[vw_AttendanceDefaultView];
GO
IF OBJECT_ID(N'[IOMDbContextStoreContainer].[vw_TaskClocker]', 'U') IS NOT NULL
    DROP TABLE [IOMDbContextStoreContainer].[vw_TaskClocker];
GO
IF OBJECT_ID(N'[IOMDbContextStoreContainer].[database_firewall_rules]', 'U') IS NOT NULL
    DROP TABLE [IOMDbContextStoreContainer].[database_firewall_rules];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'C__RefactorLog'
CREATE TABLE [dbo].[C__RefactorLog] (
    [OperationKey] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AccountMembers'
CREATE TABLE [dbo].[AccountMembers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [AccountId] int  NOT NULL,
    [Created] datetime  NULL,
    [IsDeleted] bit  NOT NULL,
    [IsNameMasked] bit  NOT NULL
);
GO

-- Creating table 'Accounts'
CREATE TABLE [dbo].[Accounts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(255)  NOT NULL,
    [ContactPerson] nvarchar(255)  NULL,
    [EmailAddress] nvarchar(255)  NULL,
    [OfficeAddress] nvarchar(255)  NULL,
    [Website] nvarchar(255)  NULL,
    [Created] datetime  NOT NULL,
    [IsActive] bit  NOT NULL,
    [IsDeleted] bit  NOT NULL,
    [SeatCode] nvarchar(50)  NULL,
    [SeatSlot] int  NULL
);
GO

-- Creating table 'AspNetModules'
CREATE TABLE [dbo].[AspNetModules] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ModuleCode] varchar(150)  NULL,
    [Name] varchar(150)  NULL,
    [CreatedBy] varchar(150)  NULL,
    [DateCreated] datetime  NULL,
    [UpdateBy] varchar(150)  NULL,
    [DateUpdated] datetime  NULL,
    [IsActive] bit  NULL,
    [ParentModuleCode] varchar(150)  NULL,
    [Order] int  NULL,
    [SubModuleOrder] tinyint  NULL
);
GO

-- Creating table 'AspNetRoles'
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(128)  NOT NULL,
    [Name] nvarchar(256)  NOT NULL,
    [RoleCode] varchar(5)  NULL,
    [PermissionsLocked] bit  NOT NULL,
    [IsAllUsers] bit  NOT NULL
);
GO

-- Creating table 'AspNetUserClaims'
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [ClaimType] nvarchar(max)  NULL,
    [ClaimValue] nvarchar(max)  NULL
);
GO

-- Creating table 'AspNetUserLogins'
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] nvarchar(128)  NOT NULL,
    [ProviderKey] nvarchar(128)  NOT NULL,
    [UserId] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'AspNetUserPermissions'
CREATE TABLE [dbo].[AspNetUserPermissions] (
    [id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NULL,
    [ModuleCode] nvarchar(128)  NULL,
    [CanView] bit  NULL,
    [CanEdit] bit  NULL,
    [CanDelete] bit  NULL,
    [CanAdd] bit  NULL,
    [CreatedBy] nvarchar(150)  NULL,
    [DateCreated] datetime  NULL,
    [UpdateBy] nvarchar(150)  NULL,
    [DateUpdated] datetime  NULL,
    [IsActive] bit  NULL
);
GO

-- Creating table 'AspNetUsers'
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(128)  NOT NULL,
    [Email] nvarchar(256)  NULL,
    [EmailConfirmed] bit  NOT NULL,
    [PasswordHash] nvarchar(max)  NULL,
    [SecurityStamp] nvarchar(max)  NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [PhoneNumberConfirmed] bit  NOT NULL,
    [TwoFactorEnabled] bit  NOT NULL,
    [LockoutEndDateUtc] datetime  NULL,
    [LockoutEnabled] bit  NOT NULL,
    [AccessFailedCount] int  NOT NULL,
    [UserName] nvarchar(256)  NOT NULL,
    [IsLoggedIn] bit  NOT NULL,
    [StatusUpdateDT] datetime  NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'AttendanceOTUpdates'
CREATE TABLE [dbo].[AttendanceOTUpdates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HistoryDate] datetime  NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [RegularHours] decimal(18,2)  NULL,
    [OTHours] decimal(18,2)  NULL,
    [DateCreated] datetime  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [LastUpdated] datetime  NULL,
    [LastUpdatedBy] int  NULL
);
GO

-- Creating table 'AttendanceStatusOptions'
CREATE TABLE [dbo].[AttendanceStatusOptions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Status_Name] varchar(50)  NULL
);
GO

-- Creating table 'AttendanceStatusUpdates'
CREATE TABLE [dbo].[AttendanceStatusUpdates] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [CreatedBy] nvarchar(150)  NOT NULL,
    [OldStatus] varchar(100)  NULL,
    [NewStatus] varchar(100)  NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [StatusDate] varchar(50)  NULL
);
GO

-- Creating table 'EmployeeShifts'
CREATE TABLE [dbo].[EmployeeShifts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(150)  NOT NULL,
    [Description] varchar(255)  NULL
);
GO

-- Creating table 'EmployeeStatus'
CREATE TABLE [dbo].[EmployeeStatus] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(150)  NOT NULL,
    [Description] varchar(255)  NULL
);
GO

-- Creating table 'EODReports'
CREATE TABLE [dbo].[EODReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [Note] nvarchar(max)  NULL,
    [EODDate] datetime  NOT NULL,
    [SentUTCDateTime] datetime  NULL,
    [DateCreated] datetime  NOT NULL,
    [ClientOffset] float  NULL,
    [IsConfirmed] bit  NULL,
    [IsEdited] bit  NULL,
    [ConfirmedUTCDate] datetime  NULL,
    [ConfirmedBy] int  NULL,
    [Recipients] nvarchar(max)  NULL
);
GO

-- Creating table 'EODTaskItems'
CREATE TABLE [dbo].[EODTaskItems] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TaskId] int  NOT NULL,
    [TotalTaskHours] decimal(10,2)  NOT NULL,
    [AdjustedTotalHours] decimal(10,2)  NOT NULL,
    [EODReportId] int  NOT NULL,
    [IsEdited] bit  NULL,
    [IsRemoved] bit  NULL,
    [IsInserted] bit  NULL
);
GO

-- Creating table 'IOMTasks'
CREATE TABLE [dbo].[IOMTasks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TaskNumber] varchar(50)  NULL,
    [Name] varchar(150)  NOT NULL,
    [Description] varchar(250)  NULL,
    [CreatedDate] datetime  NOT NULL,
    [CreatedBy] nvarchar(128)  NOT NULL,
    [IsDeleted] bit  NOT NULL,
    [IsActive] bit  NOT NULL,
    [ClickUpRef] nvarchar(max)  NULL,
    [Trigger] nvarchar(max)  NULL,
    [Manual] nvarchar(max)  NULL
);
GO

-- Creating table 'IOMTeamTasks'
CREATE TABLE [dbo].[IOMTeamTasks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TeamId] int  NOT NULL,
    [TaskId] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [CreatedBy] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'IpWhitelists'
CREATE TABLE [dbo].[IpWhitelists] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IPAddress] varchar(20)  NOT NULL,
    [Alias] varchar(50)  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [UpdatedBy] int  NULL,
    [IsDeleted] bit  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedDate] datetime  NULL
);
GO

-- Creating table 'Locations'
CREATE TABLE [dbo].[Locations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(250)  NOT NULL,
    [Description] varchar(max)  NOT NULL,
    [IsDeleted] bit  NOT NULL,
    [CreateDate] datetime  NULL,
    [CreatedBy] varchar(50)  NULL,
    [LastUpdateDate] datetime  NULL,
    [UpdatedBy] varchar(50)  NULL
);
GO

-- Creating table 'Notifications'
CREATE TABLE [dbo].[Notifications] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ToUserId] int  NOT NULL,
    [NoteDate] datetime  NOT NULL,
    [IsRead] bit  NOT NULL,
    [IsArchived] bit  NOT NULL,
    [Message] nvarchar(max)  NULL,
    [ReadDate] datetime  NULL,
    [ArchiveDate] datetime  NULL,
    [LastUpdateBy] int  NULL,
    [Title] varchar(120)  NULL,
    [Icon] varchar(100)  NULL,
    [NoteType] varchar(50)  NULL,
    [IsDisplayed] bit  NOT NULL,
    [SenderId] int  NULL
);
GO

-- Creating table 'NotificationRecipientRoles'
CREATE TABLE [dbo].[NotificationRecipientRoles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NotificationSettingId] int  NOT NULL,
    [RoleId] nvarchar(128)  NOT NULL,
    [CreatedDate] datetime  NULL,
    [CreatedBy] int  NULL,
    [LastUpdated] datetime  NULL,
    [UpdatedBy] int  NULL
);
GO

-- Creating table 'NotificationSettings'
CREATE TABLE [dbo].[NotificationSettings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(30)  NOT NULL,
    [Action] varchar(250)  NULL,
    [Subject] varchar(250)  NULL,
    [Message] varchar(max)  NULL,
    [Type] varchar(50)  NULL,
    [CreatedDate] datetime  NOT NULL,
    [CreatedBy] int  NULL,
    [LastUpdated] datetime  NOT NULL,
    [UpdatedBy] int  NULL
);
GO

-- Creating table 'NotificationTypes'
CREATE TABLE [dbo].[NotificationTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(30)  NOT NULL,
    [Description] varchar(250)  NULL
);
GO

-- Creating table 'RolePermissions'
CREATE TABLE [dbo].[RolePermissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RoleId] nvarchar(128)  NOT NULL,
    [ModuleId] int  NOT NULL,
    [IsLocked] bit  NOT NULL,
    [CanView] bit  NOT NULL,
    [CanAdd] bit  NOT NULL,
    [CanEdit] bit  NOT NULL,
    [CanDelete] bit  NOT NULL,
    [CreatedBy] varchar(150)  NULL,
    [DateCreated] datetime  NULL,
    [ModuleCode] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'Seats'
CREATE TABLE [dbo].[Seats] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AccountId] int  NOT NULL,
    [UserId] int  NOT NULL,
    [SeatNumber] int  NULL,
    [Status] varchar(50)  NULL
);
GO

-- Creating table 'SystemLogs'
CREATE TABLE [dbo].[SystemLogs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [LogDate] datetime  NOT NULL,
    [ActorUserId] int  NOT NULL,
    [RawUrl] varchar(250)  NULL,
    [ActionType] varchar(50)  NULL,
    [Description] varchar(500)  NULL,
    [Entity] varchar(100)  NULL,
    [BrowserUsed] varchar(100)  NULL,
    [IPAddress] varchar(50)  NULL,
    [ElapseTime] decimal(7,0)  NULL,
    [ResponseStatusCode] int  NULL,
    [UrlParams] nvarchar(500)  NULL,
    [RequestBody] nvarchar(max)  NULL,
    [Note] nvarchar(250)  NULL,
    [EODEmailReference] nvarchar(50)  NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(65)  NULL,
    [DateCreated] datetime  NULL,
    [IsDeleted] bit  NOT NULL
);
GO

-- Creating table 'TaskComments'
CREATE TABLE [dbo].[TaskComments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TaskHistoryId] int  NOT NULL,
    [Comment] varchar(65)  NOT NULL,
    [ChildOf] int  NULL,
    [CreateDate] datetime  NOT NULL,
    [UpdateDate] datetime  NULL,
    [CreatedBy] int  NOT NULL
);
GO

-- Creating table 'TaskGroupItems'
CREATE TABLE [dbo].[TaskGroupItems] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TaskId] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [CreatedBy] nvarchar(150)  NOT NULL,
    [GroupId] int  NOT NULL
);
GO

-- Creating table 'TaskGroups'
CREATE TABLE [dbo].[TaskGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(150)  NOT NULL,
    [Description] varchar(250)  NULL,
    [CreatedDate] datetime  NOT NULL,
    [CreatedBy] nvarchar(128)  NOT NULL,
    [IsDeleted] bit  NOT NULL,
    [IsActive] bit  NOT NULL
);
GO

-- Creating table 'TaskHistories'
CREATE TABLE [dbo].[TaskHistories] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TaskId] int  NULL,
    [UserDetailsId] int  NOT NULL,
    [HistoryDate] datetime  NOT NULL,
    [Start] datetime  NULL,
    [Duration] decimal(10,2)  NULL,
    [IsActive] bit  NOT NULL,
    [TaskHistoryTypeId] int  NOT NULL,
    [ActivateTime] datetime  NULL
);
GO

-- Creating table 'TaskHistoryTypes'
CREATE TABLE [dbo].[TaskHistoryTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [Description] varchar(255)  NULL
);
GO

-- Creating table 'TeamClientPOCs'
CREATE TABLE [dbo].[TeamClientPOCs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TeamId] int  NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'TeamDayOffs'
CREATE TABLE [dbo].[TeamDayOffs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TeamId] int  NOT NULL,
    [Day] varchar(50)  NOT NULL,
    [CreateDate] datetime  NULL,
    [CreatedBy] varchar(50)  NULL,
    [LastUpdateDate] datetime  NULL,
    [UpdatedBy] varchar(50)  NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'TeamHolidays'
CREATE TABLE [dbo].[TeamHolidays] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TeamId] int  NOT NULL,
    [HolidayDate] datetime  NOT NULL,
    [CreateDate] datetime  NULL,
    [CreatedBy] varbinary(50)  NULL,
    [LastUpdateDate] datetime  NULL,
    [UpdatedBy] varchar(50)  NULL,
    [Description] varchar(250)  NULL,
    [Title] varchar(100)  NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'TeamManagers'
CREATE TABLE [dbo].[TeamManagers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TeamId] int  NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [IsDeleted] bit  NOT NULL
);
GO

-- Creating table 'TeamMembers'
CREATE TABLE [dbo].[TeamMembers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [TeamId] int  NOT NULL,
    [CreatedDateUtc] datetime  NULL,
    [IsDeleted] bit  NOT NULL,
    [UpdatedDateUtc] datetime  NULL
);
GO

-- Creating table 'Teams'
CREATE TABLE [dbo].[Teams] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AccountId] int  NOT NULL,
    [Name] nvarchar(255)  NOT NULL,
    [Description] nvarchar(255)  NULL,
    [ShiftSchedule] nvarchar(255)  NULL,
    [LocationId] int  NULL,
    [ShiftId] int  NULL,
    [IsActive] bit  NOT NULL,
    [IsDeleted] bit  NOT NULL,
    [Created] datetime  NOT NULL
);
GO

-- Creating table 'TeamSupervisors'
CREATE TABLE [dbo].[TeamSupervisors] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TeamId] int  NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [Created] datetime  NOT NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'TeamTaskGroups'
CREATE TABLE [dbo].[TeamTaskGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TeamId] int  NOT NULL,
    [GroupId] int  NOT NULL,
    [CreatedBy] nvarchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL
);
GO

-- Creating table 'TeamTasks'
CREATE TABLE [dbo].[TeamTasks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [TeamId] int  NOT NULL,
    [Name] nvarchar(255)  NOT NULL,
    [Description] nvarchar(255)  NOT NULL,
    [Created] datetime  NOT NULL,
    [IsActive] bit  NOT NULL,
    [EnableNotification] bit  NOT NULL,
    [IsDeleted] bit  NULL
);
GO

-- Creating table 'TimeZones'
CREATE TABLE [dbo].[TimeZones] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Zone] nvarchar(100)  NULL,
    [Value] nvarchar(100)  NULL
);
GO

-- Creating table 'UserDayOffs'
CREATE TABLE [dbo].[UserDayOffs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [Day] varchar(50)  NOT NULL,
    [NumericDay] int  NOT NULL
);
GO

-- Creating table 'UserDetails'
CREATE TABLE [dbo].[UserDetails] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [FirstName] nvarchar(125)  NOT NULL,
    [LastName] nvarchar(125)  NOT NULL,
    [Name] nvarchar(255)  NOT NULL,
    [EmployeeStatus] nvarchar(255)  NULL,
    [HourlyRate] decimal(8,2)  NULL,
    [Role] nvarchar(255)  NULL,
    [Shift] nvarchar(255)  NULL,
    [WeekSchedule] nvarchar(255)  NULL,
    [Image] nvarchar(max)  NULL,
    [TemporaryPassword] bit  NOT NULL,
    [LocationId] int  NULL,
    [Created] datetime  NOT NULL,
    [CreatedBy] nvarchar(128)  NULL,
    [IsLocked] bit  NULL,
    [EmployeeStatusId] int  NULL,
    [EmployeeShiftId] int  NULL,
    [IsDeleted] bit  NOT NULL,
    [AccountId] int  NULL,
    [StaffId] nvarchar(20)  NULL,
    [IsUnrestrictedIp] bit  NOT NULL,
    [TimeZoneId] int  NULL,
    [IPAddress] varchar(50)  NULL
);
GO

-- Creating table 'UserImageFiles'
CREATE TABLE [dbo].[UserImageFiles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [ImageUri] varchar(500)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreateDate] datetime  NULL,
    [CreatedBy] varchar(50)  NULL
);
GO

-- Creating table 'UserNotificationSettings'
CREATE TABLE [dbo].[UserNotificationSettings] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [NotificationTypeId] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [IsAllowed] bit  NOT NULL
);
GO

-- Creating table 'UserShiftDetails'
CREATE TABLE [dbo].[UserShiftDetails] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [ShiftStart] time  NULL,
    [ShiftEnd] time  NULL,
    [LunchBreak] float  NOT NULL,
    [PaidBreaks] tinyint  NOT NULL,
    [CreatedDate] datetime  NULL,
    [UpdatedDate] datetime  NULL,
    [UpdatedBy] int  NULL
);
GO

-- Creating table 'UserTags'
CREATE TABLE [dbo].[UserTags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [TagId] int  NOT NULL,
    [DateCreated] datetime  NOT NULL
);
GO

-- Creating table 'UserTasks'
CREATE TABLE [dbo].[UserTasks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [TaskId] int  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [CreatedBy] nvarchar(128)  NOT NULL,
    [TeamId] int  NULL,
    [GroupId] int  NULL
);
GO

-- Creating table 'UserTaskGroups'
CREATE TABLE [dbo].[UserTaskGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [GroupId] int  NOT NULL,
    [CreatedBy] nvarchar(128)  NOT NULL,
    [CreatedDate] datetime  NOT NULL
);
GO

-- Creating table 'UserTaskNotifications'
CREATE TABLE [dbo].[UserTaskNotifications] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [TaskId] int  NOT NULL,
    [DateCreated] datetime  NOT NULL
);
GO

-- Creating table 'UserWorkDays'
CREATE TABLE [dbo].[UserWorkDays] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [DayIndex] tinyint  NOT NULL,
    [Day] varchar(10)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [UpdatedDate] datetime  NOT NULL,
    [UpdatedBy] int  NULL
);
GO

-- Creating table 'vw_ActiveUsers'
CREATE TABLE [dbo].[vw_ActiveUsers] (
    [UserDetailsId] int  NOT NULL,
    [NetUserId] nvarchar(128)  NOT NULL,
    [Email] nvarchar(256)  NULL,
    [UserName] nvarchar(256)  NOT NULL,
    [FullName] nvarchar(251)  NOT NULL,
    [FirstName] nvarchar(125)  NOT NULL,
    [LastName] nvarchar(125)  NOT NULL,
    [Role] nvarchar(255)  NULL,
    [RoleName] nvarchar(256)  NULL,
    [Image] nvarchar(max)  NULL,
    [StaffId] nvarchar(20)  NULL,
    [AccountIds] nvarchar(max)  NULL,
    [TeamIds] nvarchar(max)  NULL
);
GO

-- Creating table 'vw_AgentTeamSupervisors'
CREATE TABLE [dbo].[vw_AgentTeamSupervisors] (
    [UserUniqueId] nvarchar(128)  NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [Fullname] nvarchar(255)  NULL,
    [FirstName] nvarchar(125)  NULL,
    [LastName] nvarchar(125)  NULL,
    [Role] nvarchar(255)  NULL,
    [RoleName] nvarchar(256)  NULL,
    [Image] nvarchar(max)  NULL,
    [AccountIds] nvarchar(max)  NULL,
    [TeamIds] nvarchar(max)  NULL
);
GO

-- Creating table 'vw_AllUsers'
CREATE TABLE [dbo].[vw_AllUsers] (
    [UserDetailsId] int  NOT NULL,
    [NetUserId] nvarchar(128)  NOT NULL,
    [Email] nvarchar(256)  NULL,
    [UserName] nvarchar(256)  NOT NULL,
    [FullName] nvarchar(251)  NOT NULL,
    [FirstName] nvarchar(125)  NOT NULL,
    [LastName] nvarchar(125)  NOT NULL,
    [Role] nvarchar(255)  NULL,
    [RoleName] nvarchar(256)  NULL,
    [Image] nvarchar(max)  NULL,
    [IsDeleted] bit  NOT NULL,
    [IsLocked] bit  NULL,
    [StaffId] nvarchar(20)  NULL,
    [TimeZoneId] int  NULL,
    [AccountIds] nvarchar(max)  NULL,
    [TeamIds] nvarchar(max)  NULL
);
GO

-- Creating table 'vw_AttendanceDefaultView'
CREATE TABLE [dbo].[vw_AttendanceDefaultView] (
    [UserDetailsId] int  NOT NULL,
    [TotalActiveTime] decimal(10,2)  NULL,
    [HistoryDate] datetime  NOT NULL,
    [RegularHours] decimal(10,2)  NULL,
    [OTHours] decimal(11,2)  NULL
);
GO

-- Creating table 'vw_TaskClocker'
CREATE TABLE [dbo].[vw_TaskClocker] (
    [UserUniqueId] nvarchar(128)  NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [StaffId] nvarchar(20)  NULL,
    [Fullname] nvarchar(255)  NULL,
    [FirstName] nvarchar(125)  NULL,
    [LastName] nvarchar(125)  NULL,
    [Role] nvarchar(255)  NULL,
    [RoleName] nvarchar(256)  NOT NULL,
    [Image] nvarchar(max)  NULL,
    [Email] nvarchar(256)  NULL,
    [AccountIds] nvarchar(max)  NULL,
    [TeamIds] nvarchar(max)  NULL
);
GO

-- Creating table 'database_firewall_rules'
CREATE TABLE [dbo].[database_firewall_rules] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] nvarchar(128)  NOT NULL,
    [start_ip_address] varchar(45)  NOT NULL,
    [end_ip_address] varchar(45)  NOT NULL,
    [create_date] datetime  NOT NULL,
    [modify_date] datetime  NOT NULL
);
GO

-- Creating table 'AttendanceRows'
CREATE TABLE [dbo].[AttendanceRows] (
    [Id] int  NOT NULL,
    [Hours] float  NOT NULL,
    [AttendanceTag] nvarchar(50)  NULL,
    [CreatedDate] datetime  NULL,
    [CreatedBy] nvarchar(128)  NULL,
    [UpdatedDate] datetime  NULL,
    [UpdatedBy] nvarchar(128)  NULL,
    [AttendanceId] int  NOT NULL
);
GO

-- Creating table 'Attendances'
CREATE TABLE [dbo].[Attendances] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserDetailsId] int  NOT NULL,
    [AttendanceDate] datetime  NOT NULL,
    [WorkedHours] float  NOT NULL,
    [WorkedDay] float  NOT NULL,
    [CreatedDate] datetime  NULL,
    [CreatedBy] nvarchar(128)  NULL,
    [UpdatedDate] datetime  NULL,
    [UpdatedBy] nvarchar(128)  NULL,
    [TotalHours] float  NULL,
    [StartTime] datetime  NULL,
    [EndTime] datetime  NULL
);
GO

-- Creating table 'AspNetUserRoles'
CREATE TABLE [dbo].[AspNetUserRoles] (
    [AspNetRoles_Id] nvarchar(128)  NOT NULL,
    [AspNetUsers_Id] nvarchar(128)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [OperationKey] in table 'C__RefactorLog'
ALTER TABLE [dbo].[C__RefactorLog]
ADD CONSTRAINT [PK_C__RefactorLog]
    PRIMARY KEY CLUSTERED ([OperationKey] ASC);
GO

-- Creating primary key on [Id] in table 'AccountMembers'
ALTER TABLE [dbo].[AccountMembers]
ADD CONSTRAINT [PK_AccountMembers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Accounts'
ALTER TABLE [dbo].[Accounts]
ADD CONSTRAINT [PK_Accounts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetModules'
ALTER TABLE [dbo].[AspNetModules]
ADD CONSTRAINT [PK_AspNetModules]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetRoles'
ALTER TABLE [dbo].[AspNetRoles]
ADD CONSTRAINT [PK_AspNetRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [PK_AspNetUserClaims]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [LoginProvider], [ProviderKey], [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [PK_AspNetUserLogins]
    PRIMARY KEY CLUSTERED ([LoginProvider], [ProviderKey], [UserId] ASC);
GO

-- Creating primary key on [id] in table 'AspNetUserPermissions'
ALTER TABLE [dbo].[AspNetUserPermissions]
ADD CONSTRAINT [PK_AspNetUserPermissions]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUsers'
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [PK_AspNetUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AttendanceOTUpdates'
ALTER TABLE [dbo].[AttendanceOTUpdates]
ADD CONSTRAINT [PK_AttendanceOTUpdates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AttendanceStatusOptions'
ALTER TABLE [dbo].[AttendanceStatusOptions]
ADD CONSTRAINT [PK_AttendanceStatusOptions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AttendanceStatusUpdates'
ALTER TABLE [dbo].[AttendanceStatusUpdates]
ADD CONSTRAINT [PK_AttendanceStatusUpdates]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EmployeeShifts'
ALTER TABLE [dbo].[EmployeeShifts]
ADD CONSTRAINT [PK_EmployeeShifts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EmployeeStatus'
ALTER TABLE [dbo].[EmployeeStatus]
ADD CONSTRAINT [PK_EmployeeStatus]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EODReports'
ALTER TABLE [dbo].[EODReports]
ADD CONSTRAINT [PK_EODReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EODTaskItems'
ALTER TABLE [dbo].[EODTaskItems]
ADD CONSTRAINT [PK_EODTaskItems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'IOMTasks'
ALTER TABLE [dbo].[IOMTasks]
ADD CONSTRAINT [PK_IOMTasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'IOMTeamTasks'
ALTER TABLE [dbo].[IOMTeamTasks]
ADD CONSTRAINT [PK_IOMTeamTasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'IpWhitelists'
ALTER TABLE [dbo].[IpWhitelists]
ADD CONSTRAINT [PK_IpWhitelists]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Locations'
ALTER TABLE [dbo].[Locations]
ADD CONSTRAINT [PK_Locations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Notifications'
ALTER TABLE [dbo].[Notifications]
ADD CONSTRAINT [PK_Notifications]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NotificationRecipientRoles'
ALTER TABLE [dbo].[NotificationRecipientRoles]
ADD CONSTRAINT [PK_NotificationRecipientRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NotificationSettings'
ALTER TABLE [dbo].[NotificationSettings]
ADD CONSTRAINT [PK_NotificationSettings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NotificationTypes'
ALTER TABLE [dbo].[NotificationTypes]
ADD CONSTRAINT [PK_NotificationTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RolePermissions'
ALTER TABLE [dbo].[RolePermissions]
ADD CONSTRAINT [PK_RolePermissions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Seats'
ALTER TABLE [dbo].[Seats]
ADD CONSTRAINT [PK_Seats]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SystemLogs'
ALTER TABLE [dbo].[SystemLogs]
ADD CONSTRAINT [PK_SystemLogs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TaskComments'
ALTER TABLE [dbo].[TaskComments]
ADD CONSTRAINT [PK_TaskComments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TaskGroupItems'
ALTER TABLE [dbo].[TaskGroupItems]
ADD CONSTRAINT [PK_TaskGroupItems]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TaskGroups'
ALTER TABLE [dbo].[TaskGroups]
ADD CONSTRAINT [PK_TaskGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TaskHistories'
ALTER TABLE [dbo].[TaskHistories]
ADD CONSTRAINT [PK_TaskHistories]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TaskHistoryTypes'
ALTER TABLE [dbo].[TaskHistoryTypes]
ADD CONSTRAINT [PK_TaskHistoryTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TeamClientPOCs'
ALTER TABLE [dbo].[TeamClientPOCs]
ADD CONSTRAINT [PK_TeamClientPOCs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TeamDayOffs'
ALTER TABLE [dbo].[TeamDayOffs]
ADD CONSTRAINT [PK_TeamDayOffs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TeamHolidays'
ALTER TABLE [dbo].[TeamHolidays]
ADD CONSTRAINT [PK_TeamHolidays]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TeamManagers'
ALTER TABLE [dbo].[TeamManagers]
ADD CONSTRAINT [PK_TeamManagers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TeamMembers'
ALTER TABLE [dbo].[TeamMembers]
ADD CONSTRAINT [PK_TeamMembers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Teams'
ALTER TABLE [dbo].[Teams]
ADD CONSTRAINT [PK_Teams]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [TeamId], [UserId] in table 'TeamSupervisors'
ALTER TABLE [dbo].[TeamSupervisors]
ADD CONSTRAINT [PK_TeamSupervisors]
    PRIMARY KEY CLUSTERED ([TeamId], [UserId] ASC);
GO

-- Creating primary key on [Id] in table 'TeamTaskGroups'
ALTER TABLE [dbo].[TeamTaskGroups]
ADD CONSTRAINT [PK_TeamTaskGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TeamTasks'
ALTER TABLE [dbo].[TeamTasks]
ADD CONSTRAINT [PK_TeamTasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TimeZones'
ALTER TABLE [dbo].[TimeZones]
ADD CONSTRAINT [PK_TimeZones]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserDayOffs'
ALTER TABLE [dbo].[UserDayOffs]
ADD CONSTRAINT [PK_UserDayOffs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserDetails'
ALTER TABLE [dbo].[UserDetails]
ADD CONSTRAINT [PK_UserDetails]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserImageFiles'
ALTER TABLE [dbo].[UserImageFiles]
ADD CONSTRAINT [PK_UserImageFiles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserNotificationSettings'
ALTER TABLE [dbo].[UserNotificationSettings]
ADD CONSTRAINT [PK_UserNotificationSettings]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserShiftDetails'
ALTER TABLE [dbo].[UserShiftDetails]
ADD CONSTRAINT [PK_UserShiftDetails]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserTags'
ALTER TABLE [dbo].[UserTags]
ADD CONSTRAINT [PK_UserTags]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserTasks'
ALTER TABLE [dbo].[UserTasks]
ADD CONSTRAINT [PK_UserTasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserTaskGroups'
ALTER TABLE [dbo].[UserTaskGroups]
ADD CONSTRAINT [PK_UserTaskGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserTaskNotifications'
ALTER TABLE [dbo].[UserTaskNotifications]
ADD CONSTRAINT [PK_UserTaskNotifications]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserWorkDays'
ALTER TABLE [dbo].[UserWorkDays]
ADD CONSTRAINT [PK_UserWorkDays]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [UserDetailsId], [NetUserId], [UserName], [FullName], [FirstName], [LastName] in table 'vw_ActiveUsers'
ALTER TABLE [dbo].[vw_ActiveUsers]
ADD CONSTRAINT [PK_vw_ActiveUsers]
    PRIMARY KEY CLUSTERED ([UserDetailsId], [NetUserId], [UserName], [FullName], [FirstName], [LastName] ASC);
GO

-- Creating primary key on [UserUniqueId], [UserDetailsId] in table 'vw_AgentTeamSupervisors'
ALTER TABLE [dbo].[vw_AgentTeamSupervisors]
ADD CONSTRAINT [PK_vw_AgentTeamSupervisors]
    PRIMARY KEY CLUSTERED ([UserUniqueId], [UserDetailsId] ASC);
GO

-- Creating primary key on [UserDetailsId], [NetUserId], [UserName], [FullName], [FirstName], [LastName], [IsDeleted] in table 'vw_AllUsers'
ALTER TABLE [dbo].[vw_AllUsers]
ADD CONSTRAINT [PK_vw_AllUsers]
    PRIMARY KEY CLUSTERED ([UserDetailsId], [NetUserId], [UserName], [FullName], [FirstName], [LastName], [IsDeleted] ASC);
GO

-- Creating primary key on [UserDetailsId], [HistoryDate] in table 'vw_AttendanceDefaultView'
ALTER TABLE [dbo].[vw_AttendanceDefaultView]
ADD CONSTRAINT [PK_vw_AttendanceDefaultView]
    PRIMARY KEY CLUSTERED ([UserDetailsId], [HistoryDate] ASC);
GO

-- Creating primary key on [UserUniqueId], [UserDetailsId], [RoleName] in table 'vw_TaskClocker'
ALTER TABLE [dbo].[vw_TaskClocker]
ADD CONSTRAINT [PK_vw_TaskClocker]
    PRIMARY KEY CLUSTERED ([UserUniqueId], [UserDetailsId], [RoleName] ASC);
GO

-- Creating primary key on [id], [name], [start_ip_address], [end_ip_address], [create_date], [modify_date] in table 'database_firewall_rules'
ALTER TABLE [dbo].[database_firewall_rules]
ADD CONSTRAINT [PK_database_firewall_rules]
    PRIMARY KEY CLUSTERED ([id], [name], [start_ip_address], [end_ip_address], [create_date], [modify_date] ASC);
GO

-- Creating primary key on [Id] in table 'AttendanceRows'
ALTER TABLE [dbo].[AttendanceRows]
ADD CONSTRAINT [PK_AttendanceRows]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Attendances'
ALTER TABLE [dbo].[Attendances]
ADD CONSTRAINT [PK_Attendances]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [AspNetRoles_Id], [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [PK_AspNetUserRoles]
    PRIMARY KEY CLUSTERED ([AspNetRoles_Id], [AspNetUsers_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AccountId] in table 'AccountMembers'
ALTER TABLE [dbo].[AccountMembers]
ADD CONSTRAINT [FK_AccountMember_Account]
    FOREIGN KEY ([AccountId])
    REFERENCES [dbo].[Accounts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountMember_Account'
CREATE INDEX [IX_FK_AccountMember_Account]
ON [dbo].[AccountMembers]
    ([AccountId]);
GO

-- Creating foreign key on [UserDetailsId] in table 'AccountMembers'
ALTER TABLE [dbo].[AccountMembers]
ADD CONSTRAINT [FK_AccountMember_UserDetails]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountMember_UserDetails'
CREATE INDEX [IX_FK_AccountMember_UserDetails]
ON [dbo].[AccountMembers]
    ([UserDetailsId]);
GO

-- Creating foreign key on [AccountId] in table 'Teams'
ALTER TABLE [dbo].[Teams]
ADD CONSTRAINT [FK_Teams_fk]
    FOREIGN KEY ([AccountId])
    REFERENCES [dbo].[Accounts]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Teams_fk'
CREATE INDEX [IX_FK_Teams_fk]
ON [dbo].[Teams]
    ([AccountId]);
GO

-- Creating foreign key on [ModuleId] in table 'RolePermissions'
ALTER TABLE [dbo].[RolePermissions]
ADD CONSTRAINT [FK_RolePermissions_AspNetModules]
    FOREIGN KEY ([ModuleId])
    REFERENCES [dbo].[AspNetModules]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RolePermissions_AspNetModules'
CREATE INDEX [IX_FK_RolePermissions_AspNetModules]
ON [dbo].[RolePermissions]
    ([ModuleId]);
GO

-- Creating foreign key on [RoleId] in table 'NotificationRecipientRoles'
ALTER TABLE [dbo].[NotificationRecipientRoles]
ADD CONSTRAINT [FK_NotificationRecipientRole_Role]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NotificationRecipientRole_Role'
CREATE INDEX [IX_FK_NotificationRecipientRole_Role]
ON [dbo].[NotificationRecipientRoles]
    ([RoleId]);
GO

-- Creating foreign key on [RoleId] in table 'RolePermissions'
ALTER TABLE [dbo].[RolePermissions]
ADD CONSTRAINT [FK_RolePermissions_ToAspNetRoles]
    FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RolePermissions_ToAspNetRoles'
CREATE INDEX [IX_FK_RolePermissions_ToAspNetRoles]
ON [dbo].[RolePermissions]
    ([RoleId]);
GO

-- Creating foreign key on [UserId] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserClaims]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserLogins]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'UserTasks'
ALTER TABLE [dbo].[UserTasks]
ADD CONSTRAINT [FK_UserTask_AspNetUsers]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTask_AspNetUsers'
CREATE INDEX [IX_FK_UserTask_AspNetUsers]
ON [dbo].[UserTasks]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'UserTaskGroups'
ALTER TABLE [dbo].[UserTaskGroups]
ADD CONSTRAINT [FK_UserTaskGroup_AspNetUsers]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTaskGroup_AspNetUsers'
CREATE INDEX [IX_FK_UserTaskGroup_AspNetUsers]
ON [dbo].[UserTaskGroups]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'TeamSupervisors'
ALTER TABLE [dbo].[TeamSupervisors]
ADD CONSTRAINT [FK_TeamSupervisors_fk2]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamSupervisors_fk2'
CREATE INDEX [IX_FK_TeamSupervisors_fk2]
ON [dbo].[TeamSupervisors]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'UserDetails'
ALTER TABLE [dbo].[UserDetails]
ADD CONSTRAINT [FK_UserDetailModels_fk]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserDetailModels_fk'
CREATE INDEX [IX_FK_UserDetailModels_fk]
ON [dbo].[UserDetails]
    ([UserId]);
GO

-- Creating foreign key on [TaskId] in table 'IOMTeamTasks'
ALTER TABLE [dbo].[IOMTeamTasks]
ADD CONSTRAINT [FK_IOMTeamTask_IOMTasks]
    FOREIGN KEY ([TaskId])
    REFERENCES [dbo].[IOMTasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_IOMTeamTask_IOMTasks'
CREATE INDEX [IX_FK_IOMTeamTask_IOMTasks]
ON [dbo].[IOMTeamTasks]
    ([TaskId]);
GO

-- Creating foreign key on [TaskId] in table 'TaskGroupItems'
ALTER TABLE [dbo].[TaskGroupItems]
ADD CONSTRAINT [FK_TaskGroupItems_IOMTasks]
    FOREIGN KEY ([TaskId])
    REFERENCES [dbo].[IOMTasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskGroupItems_IOMTasks'
CREATE INDEX [IX_FK_TaskGroupItems_IOMTasks]
ON [dbo].[TaskGroupItems]
    ([TaskId]);
GO

-- Creating foreign key on [TaskId] in table 'UserTasks'
ALTER TABLE [dbo].[UserTasks]
ADD CONSTRAINT [FK_UserTask_IOMTasks]
    FOREIGN KEY ([TaskId])
    REFERENCES [dbo].[IOMTasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTask_IOMTasks'
CREATE INDEX [IX_FK_UserTask_IOMTasks]
ON [dbo].[UserTasks]
    ([TaskId]);
GO

-- Creating foreign key on [TeamId] in table 'IOMTeamTasks'
ALTER TABLE [dbo].[IOMTeamTasks]
ADD CONSTRAINT [FK_IOMTeamTask_Teams]
    FOREIGN KEY ([TeamId])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_IOMTeamTask_Teams'
CREATE INDEX [IX_FK_IOMTeamTask_Teams]
ON [dbo].[IOMTeamTasks]
    ([TeamId]);
GO

-- Creating foreign key on [LocationId] in table 'Teams'
ALTER TABLE [dbo].[Teams]
ADD CONSTRAINT [FK_Teams_Location]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Teams_Location'
CREATE INDEX [IX_FK_Teams_Location]
ON [dbo].[Teams]
    ([LocationId]);
GO

-- Creating foreign key on [LocationId] in table 'UserDetails'
ALTER TABLE [dbo].[UserDetails]
ADD CONSTRAINT [FK_UserDetails_Location]
    FOREIGN KEY ([LocationId])
    REFERENCES [dbo].[Locations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserDetails_Location'
CREATE INDEX [IX_FK_UserDetails_Location]
ON [dbo].[UserDetails]
    ([LocationId]);
GO

-- Creating foreign key on [SenderId] in table 'Notifications'
ALTER TABLE [dbo].[Notifications]
ADD CONSTRAINT [FK_Notification_Sender]
    FOREIGN KEY ([SenderId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Notification_Sender'
CREATE INDEX [IX_FK_Notification_Sender]
ON [dbo].[Notifications]
    ([SenderId]);
GO

-- Creating foreign key on [ToUserId] in table 'Notifications'
ALTER TABLE [dbo].[Notifications]
ADD CONSTRAINT [FK_Notification_UserDetails]
    FOREIGN KEY ([ToUserId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Notification_UserDetails'
CREATE INDEX [IX_FK_Notification_UserDetails]
ON [dbo].[Notifications]
    ([ToUserId]);
GO

-- Creating foreign key on [NotificationSettingId] in table 'NotificationRecipientRoles'
ALTER TABLE [dbo].[NotificationRecipientRoles]
ADD CONSTRAINT [FK_NotificationRecipientRole_NotificationSetting]
    FOREIGN KEY ([NotificationSettingId])
    REFERENCES [dbo].[NotificationSettings]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_NotificationRecipientRole_NotificationSetting'
CREATE INDEX [IX_FK_NotificationRecipientRole_NotificationSetting]
ON [dbo].[NotificationRecipientRoles]
    ([NotificationSettingId]);
GO

-- Creating foreign key on [NotificationTypeId] in table 'UserNotificationSettings'
ALTER TABLE [dbo].[UserNotificationSettings]
ADD CONSTRAINT [FK_UserNotificationSetting_NotificationType]
    FOREIGN KEY ([NotificationTypeId])
    REFERENCES [dbo].[NotificationTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserNotificationSetting_NotificationType'
CREATE INDEX [IX_FK_UserNotificationSetting_NotificationType]
ON [dbo].[UserNotificationSettings]
    ([NotificationTypeId]);
GO

-- Creating foreign key on [TagId] in table 'UserTags'
ALTER TABLE [dbo].[UserTags]
ADD CONSTRAINT [FK_UserTag_Tag]
    FOREIGN KEY ([TagId])
    REFERENCES [dbo].[Tags]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTag_Tag'
CREATE INDEX [IX_FK_UserTag_Tag]
ON [dbo].[UserTags]
    ([TagId]);
GO

-- Creating foreign key on [GroupId] in table 'TaskGroupItems'
ALTER TABLE [dbo].[TaskGroupItems]
ADD CONSTRAINT [FK_TaskGroupItems_TaskGroups]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[TaskGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskGroupItems_TaskGroups'
CREATE INDEX [IX_FK_TaskGroupItems_TaskGroups]
ON [dbo].[TaskGroupItems]
    ([GroupId]);
GO

-- Creating foreign key on [GroupId] in table 'TeamTaskGroups'
ALTER TABLE [dbo].[TeamTaskGroups]
ADD CONSTRAINT [FK_TeamTaskGroup_TaskGroups]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[TaskGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamTaskGroup_TaskGroups'
CREATE INDEX [IX_FK_TeamTaskGroup_TaskGroups]
ON [dbo].[TeamTaskGroups]
    ([GroupId]);
GO

-- Creating foreign key on [GroupId] in table 'UserTasks'
ALTER TABLE [dbo].[UserTasks]
ADD CONSTRAINT [FK_UserTask_TaskGroups]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[TaskGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTask_TaskGroups'
CREATE INDEX [IX_FK_UserTask_TaskGroups]
ON [dbo].[UserTasks]
    ([GroupId]);
GO

-- Creating foreign key on [GroupId] in table 'UserTaskGroups'
ALTER TABLE [dbo].[UserTaskGroups]
ADD CONSTRAINT [FK_UserTaskGroup_TaskGroups]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[TaskGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTaskGroup_TaskGroups'
CREATE INDEX [IX_FK_UserTaskGroup_TaskGroups]
ON [dbo].[UserTaskGroups]
    ([GroupId]);
GO

-- Creating foreign key on [UserDetailsId] in table 'TaskHistories'
ALTER TABLE [dbo].[TaskHistories]
ADD CONSTRAINT [FK_TaskHistory_ToTable]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskHistory_ToTable'
CREATE INDEX [IX_FK_TaskHistory_ToTable]
ON [dbo].[TaskHistories]
    ([UserDetailsId]);
GO

-- Creating foreign key on [TaskHistoryTypeId] in table 'TaskHistories'
ALTER TABLE [dbo].[TaskHistories]
ADD CONSTRAINT [FK_TaskHistory_ToTable_1]
    FOREIGN KEY ([TaskHistoryTypeId])
    REFERENCES [dbo].[TaskHistoryTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskHistory_ToTable_1'
CREATE INDEX [IX_FK_TaskHistory_ToTable_1]
ON [dbo].[TaskHistories]
    ([TaskHistoryTypeId]);
GO

-- Creating foreign key on [TeamId] in table 'TeamClientPOCs'
ALTER TABLE [dbo].[TeamClientPOCs]
ADD CONSTRAINT [FK_TeamClientPOC_Team]
    FOREIGN KEY ([TeamId])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamClientPOC_Team'
CREATE INDEX [IX_FK_TeamClientPOC_Team]
ON [dbo].[TeamClientPOCs]
    ([TeamId]);
GO

-- Creating foreign key on [UserDetailsId] in table 'TeamClientPOCs'
ALTER TABLE [dbo].[TeamClientPOCs]
ADD CONSTRAINT [FK_TeamClientPOC_UserDetail]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamClientPOC_UserDetail'
CREATE INDEX [IX_FK_TeamClientPOC_UserDetail]
ON [dbo].[TeamClientPOCs]
    ([UserDetailsId]);
GO

-- Creating foreign key on [TeamId] in table 'TeamDayOffs'
ALTER TABLE [dbo].[TeamDayOffs]
ADD CONSTRAINT [FK_TeamDayOff_Teams]
    FOREIGN KEY ([TeamId])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamDayOff_Teams'
CREATE INDEX [IX_FK_TeamDayOff_Teams]
ON [dbo].[TeamDayOffs]
    ([TeamId]);
GO

-- Creating foreign key on [TeamId] in table 'TeamHolidays'
ALTER TABLE [dbo].[TeamHolidays]
ADD CONSTRAINT [FK_TeamHolidays_Teams]
    FOREIGN KEY ([TeamId])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamHolidays_Teams'
CREATE INDEX [IX_FK_TeamHolidays_Teams]
ON [dbo].[TeamHolidays]
    ([TeamId]);
GO

-- Creating foreign key on [TeamId] in table 'TeamMembers'
ALTER TABLE [dbo].[TeamMembers]
ADD CONSTRAINT [FK_TeamMember_Teams]
    FOREIGN KEY ([TeamId])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamMember_Teams'
CREATE INDEX [IX_FK_TeamMember_Teams]
ON [dbo].[TeamMembers]
    ([TeamId]);
GO

-- Creating foreign key on [UserDetailsId] in table 'TeamMembers'
ALTER TABLE [dbo].[TeamMembers]
ADD CONSTRAINT [FK_TeamMember_UserDetails]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamMember_UserDetails'
CREATE INDEX [IX_FK_TeamMember_UserDetails]
ON [dbo].[TeamMembers]
    ([UserDetailsId]);
GO

-- Creating foreign key on [TeamId] in table 'TeamTaskGroups'
ALTER TABLE [dbo].[TeamTaskGroups]
ADD CONSTRAINT [FK_TeamTaskGroup_Teams]
    FOREIGN KEY ([TeamId])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamTaskGroup_Teams'
CREATE INDEX [IX_FK_TeamTaskGroup_Teams]
ON [dbo].[TeamTaskGroups]
    ([TeamId]);
GO

-- Creating foreign key on [TeamId] in table 'UserTasks'
ALTER TABLE [dbo].[UserTasks]
ADD CONSTRAINT [FK_UserTask_Teams]
    FOREIGN KEY ([TeamId])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTask_Teams'
CREATE INDEX [IX_FK_UserTask_Teams]
ON [dbo].[UserTasks]
    ([TeamId]);
GO

-- Creating foreign key on [TeamId] in table 'TeamSupervisors'
ALTER TABLE [dbo].[TeamSupervisors]
ADD CONSTRAINT [FK_TeamSupervisors_fk]
    FOREIGN KEY ([TeamId])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [TeamId] in table 'TeamTasks'
ALTER TABLE [dbo].[TeamTasks]
ADD CONSTRAINT [FK_TeamTask_fk]
    FOREIGN KEY ([TeamId])
    REFERENCES [dbo].[Teams]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TeamTask_fk'
CREATE INDEX [IX_FK_TeamTask_fk]
ON [dbo].[TeamTasks]
    ([TeamId]);
GO

-- Creating foreign key on [UserDetailsId] in table 'UserDayOffs'
ALTER TABLE [dbo].[UserDayOffs]
ADD CONSTRAINT [FK_UserDayOff_ToTable]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserDayOff_ToTable'
CREATE INDEX [IX_FK_UserDayOff_ToTable]
ON [dbo].[UserDayOffs]
    ([UserDetailsId]);
GO

-- Creating foreign key on [UserDetailsId] in table 'UserImageFiles'
ALTER TABLE [dbo].[UserImageFiles]
ADD CONSTRAINT [FK_UserImageFile_ToTable]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserImageFile_ToTable'
CREATE INDEX [IX_FK_UserImageFile_ToTable]
ON [dbo].[UserImageFiles]
    ([UserDetailsId]);
GO

-- Creating foreign key on [UserDetailsId] in table 'UserNotificationSettings'
ALTER TABLE [dbo].[UserNotificationSettings]
ADD CONSTRAINT [FK_UserNotificationSetting_UserDetail]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserNotificationSetting_UserDetail'
CREATE INDEX [IX_FK_UserNotificationSetting_UserDetail]
ON [dbo].[UserNotificationSettings]
    ([UserDetailsId]);
GO

-- Creating foreign key on [UserDetailsId] in table 'UserShiftDetails'
ALTER TABLE [dbo].[UserShiftDetails]
ADD CONSTRAINT [FK_UserShiftDetail_UserDetails]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserShiftDetail_UserDetails'
CREATE INDEX [IX_FK_UserShiftDetail_UserDetails]
ON [dbo].[UserShiftDetails]
    ([UserDetailsId]);
GO

-- Creating foreign key on [UserDetailsId] in table 'UserTags'
ALTER TABLE [dbo].[UserTags]
ADD CONSTRAINT [FK_UserTag_UserDetails]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserTag_UserDetails'
CREATE INDEX [IX_FK_UserTag_UserDetails]
ON [dbo].[UserTags]
    ([UserDetailsId]);
GO

-- Creating foreign key on [UserDetailsId] in table 'UserWorkDays'
ALTER TABLE [dbo].[UserWorkDays]
ADD CONSTRAINT [FK_UserWorkDay_UserDetails]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserWorkDay_UserDetails'
CREATE INDEX [IX_FK_UserWorkDay_UserDetails]
ON [dbo].[UserWorkDays]
    ([UserDetailsId]);
GO

-- Creating foreign key on [AspNetRoles_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRole]
    FOREIGN KEY ([AspNetRoles_Id])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUser]
    FOREIGN KEY ([AspNetUsers_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetUserRoles_AspNetUser'
CREATE INDEX [IX_FK_AspNetUserRoles_AspNetUser]
ON [dbo].[AspNetUserRoles]
    ([AspNetUsers_Id]);
GO

-- Creating foreign key on [UserDetailsId] in table 'Attendances'
ALTER TABLE [dbo].[Attendances]
ADD CONSTRAINT [FK_Attendance_UserDetails]
    FOREIGN KEY ([UserDetailsId])
    REFERENCES [dbo].[UserDetails]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Attendance_UserDetails'
CREATE INDEX [IX_FK_Attendance_UserDetails]
ON [dbo].[Attendances]
    ([UserDetailsId]);
GO

-- Creating foreign key on [AttendanceId] in table 'AttendanceRows'
ALTER TABLE [dbo].[AttendanceRows]
ADD CONSTRAINT [FK_AttendanceRow_ToAttendance]
    FOREIGN KEY ([AttendanceId])
    REFERENCES [dbo].[Attendances]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AttendanceRow_ToAttendance'
CREATE INDEX [IX_FK_AttendanceRow_ToAttendance]
ON [dbo].[AttendanceRows]
    ([AttendanceId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------