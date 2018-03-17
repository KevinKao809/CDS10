/****** Object: Table [SunnyPoint].[DevicesProvision] Script Date: 2016/8/27 上午 11:02:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'[SmartFactory].[RefCultureInfo]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[RefCultureInfo];
END
CREATE TABLE [SmartFactory].[RefCultureInfo] (
    [CultureCode] NVARCHAR (10) NOT NULL,
    [Name]        NVARCHAR (30) NULL,
    PRIMARY KEY CLUSTERED ([CultureCode] ASC)
);


IF OBJECT_ID(N'[SmartFactory].[Company]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[Company];
END
CREATE TABLE [SmartFactory].[Company] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (100) NOT NULL,
    [ShortName]      NVARCHAR (10)  NULL,
    [Address]        NVARCHAR (255) NULL,
    [CompanyWebSite] NVARCHAR (255) NULL,
    [ContactName]    NVARCHAR (50)  NULL,
    [ContactPhone]   NVARCHAR (50)  NULL,
    [ContactEmail]   NVARCHAR (50)  NULL,
    [Latitude]       FLOAT (53)     NULL,
    [Longitude]      FLOAT (53)     NULL,
    [LogoURL]        NVARCHAR (100) NULL,
    [CultureInfo]    NVARCHAR (10)  NULL,
	[DocDBConnectionString] NVARCHAR (256) NULL,
	[AllowDomain]			NVARCHAR (1000) NULL,
	[ExtAppAuthenticationKey] NVARCHAR (255) NULL,
    [CreatedAt]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]      DATETIME       NULL,
    [DeletedFlag]    BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Company_ToTable] FOREIGN KEY ([CultureInfo]) REFERENCES [SmartFactory].[RefCultureInfo] ([CultureCode])
);


GO
CREATE NONCLUSTERED INDEX [IX_Company_Column]
    ON [SmartFactory].[Company]([DeletedFlag] ASC);


IF OBJECT_ID(N'[SmartFactory].[DeviceCertificate]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[DeviceCertificate];
END
CREATE TABLE [SmartFactory].[DeviceCertificate] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CompanyID]   INT            NOT NULL,
    [Name]        NVARCHAR (100) NOT NULL,
    [FileName]    NVARCHAR (100) NULL,
    [Thumbprint]  NVARCHAR (200) NOT NULL,
    [PFXPassword] NVARCHAR (50)  NOT NULL,
    [ExpiredAt]   DATETIME       NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME       NULL,
    [DeletedFlag] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DeviceCertificate_ToTable] FOREIGN KEY ([CompanyID]) REFERENCES [SmartFactory].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_DeviceCertificate_Column]
    ON [SmartFactory].[DeviceCertificate]([CompanyID] ASC, [DeletedFlag] ASC);


IF OBJECT_ID(N'[SmartFactory].[PermissionCatalog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[PermissionCatalog];
END
CREATE TABLE [SmartFactory].[PermissionCatalog] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (100) NULL,
    [Description]  NVARCHAR (255) NULL,
    [PermissionId] INT            NOT NULL,
    [CreatedAt]    DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]    DATETIME       NULL,
    [DeletedFlag]  BIT            DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_PermissionCatalog_Column]
    ON [SmartFactory].[PermissionCatalog]([DeletedFlag] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_PermissionCatalog_Column_1]
    ON [SmartFactory].[PermissionCatalog]([Name] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_PermissionCatalog_Column_1]
    ON [SmartFactory].[PermissionCatalog]([PermissionId] ASC);

IF OBJECT_ID(N'[SmartFactory].[DeviceType]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[DeviceType];
END
CREATE TABLE [SmartFactory].[DeviceType] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (255) NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME       NULL,
    [DeletedFlag] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_DeviceType_Column]
    ON [SmartFactory].[DeviceType]([DeletedFlag] ASC);

IF OBJECT_ID(N'[SmartFactory].[EquipmentClass]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[EquipmentClass];
END
CREATE TABLE [SmartFactory].[EquipmentClass] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]   INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (255) NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME       NULL,
    [DeletedFlag] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EquipmentClass_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [SmartFactory].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_EquipmentClass_Column]
    ON [SmartFactory].[EquipmentClass]([DeletedFlag] ASC);
GO
CREATE Unique INDEX [IX_EquipmentClass_Column_1] ON [SmartFactory].[EquipmentClass] (CompanyId, Name)
Go

IF OBJECT_ID(N'[SmartFactory].[Employee]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[Employee];
END
CREATE TABLE [SmartFactory].[Employee] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]      INT            NOT NULL,
    [EmployeeNumber] NVARCHAR (50)  NULL,
    [FirstName]      NVARCHAR (50)  NULL,
    [LastName]       NVARCHAR (50)  NULL,
    [Email]          NVARCHAR (100) NOT NULL,
    [PhotoURL]       NVARCHAR (100) NULL,
    [Password]       NVARCHAR (255) NOT NULL,
    [AdminFlag]      BIT            DEFAULT ((0)) NOT NULL,
	[Lang]			 NVARCHAR (50)  NULL,
    [CreatedAt]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]      DATETIME       NULL,
    [DeletedFlag]    BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Employee_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [SmartFactory].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Employee_Column]
    ON [SmartFactory].[Employee]([CompanyId] ASC, [DeletedFlag] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_Employee_Email]
    ON [SmartFactory].[Employee]([Email] ASC);

IF OBJECT_ID(N'[SmartFactory].[UserRole]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[UserRole];
END
CREATE TABLE [SmartFactory].[UserRole] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [CompanyId]   INT           NOT NULL,
    [Name]        NVARCHAR (50) NULL,
    [CreatedAt]   DATETIME      DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME      NULL,
    [DeletedFlag] BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRole_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [SmartFactory].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_UserRole_Column]
    ON [SmartFactory].[UserRole]([CompanyId] ASC, [DeletedFlag] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_UserRole_Column_1]
    ON [SmartFactory].[UserRole]([CompanyId] ASC, [Name] ASC);

IF OBJECT_ID(N'[SmartFactory].[EmployeeInRole]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[EmployeeInRole];
END
CREATE TABLE [SmartFactory].[EmployeeInRole] (
    [Id]          INT      IDENTITY (1, 1) NOT NULL,
    [EmployeeID]  INT      NOT NULL,
    [UserRoleID]  INT      NOT NULL,
    [CreatedAt]   DATETIME DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME NULL,
    [DeletedFlag] BIT      DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EmployeeInRole_ToTable] FOREIGN KEY ([EmployeeID]) REFERENCES [SmartFactory].[Employee] ([Id]),
    CONSTRAINT [FK_EmployeeInRole_ToTable_1] FOREIGN KEY ([UserRoleID]) REFERENCES [SmartFactory].[UserRole] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_EmployeeInRole_Column]
    ON [SmartFactory].[EmployeeInRole]([EmployeeID] ASC, [UserRoleID] ASC, [DeletedFlag] ASC);

IF OBJECT_ID(N'[SmartFactory].[SuperAdmin]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[SuperAdmin];
END
CREATE TABLE [SmartFactory].[SuperAdmin] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (50)  NULL,
    [LastName]    NVARCHAR (50)  NULL,
    [Email]       NVARCHAR (100) NULL,
    [Password]    NVARCHAR (255) NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME       NULL,
    [DeletedFlag] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_SuperAdmin_Column]
    ON [SmartFactory].[SuperAdmin]([Email] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SuperAdmin_Column_1]
    ON [SmartFactory].[SuperAdmin]([DeletedFlag] ASC);


IF OBJECT_ID(N'[SmartFactory].[MessageCatalog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[MessageCatalog];
END
CREATE TABLE [SmartFactory].[MessageCatalog] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CompanyID]   INT            NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (255) NULL,
    [ChildMessageFlag] BIT            DEFAULT ((0)) NOT NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME       NULL,
    [DeletedFlag] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MessageCatalog_ToTable_1] FOREIGN KEY ([CompanyID]) REFERENCES [SmartFactory].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MessageCatalog_Column]
    ON [SmartFactory].[MessageCatalog]([DeletedFlag] ASC);


GO


IF OBJECT_ID(N'[SmartFactory].[MessageElement]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[MessageElement];
END
CREATE TABLE [SmartFactory].[MessageElement] (
    [Id]                    INT           IDENTITY (1, 1) NOT NULL,
    [MessageCatalogID]      INT           NOT NULL,
    [ElementName]           NVARCHAR (50) NULL,
    [ElementDataType]       NVARCHAR (20) NULL,
    [ChildMessageCatalogID] INT           NULL,
    [MandatoryFlag]         BIT           DEFAULT ((0)) NOT NULL,
	[SFMandatoryFlag]		BIT			  DEFAUlT ((0)) NOT NULL,
	[ShowOnEquipmentList] BIT NULL DEFAULT ((0)), 
    [ShowOnFactoryBoard] BIT NULL DEFAULT ((0)), 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MessageElement_ToTable] FOREIGN KEY ([MessageCatalogID]) REFERENCES [SmartFactory].[MessageCatalog] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_MessageElement_Column]
    ON [SmartFactory].[MessageElement]([MessageCatalogID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_MessageElement_Column_1]
    ON [SmartFactory].[MessageElement]([MessageCatalogID] ASC, [ElementName] ASC);

IF OBJECT_ID(N'[SmartFactory].[IoTHub]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[IoTHub];
END
CREATE TABLE [SmartFactory].[IoTHub] (
    [IoTHubAlias]                       NVARCHAR (50)  NOT NULL,
    [Description]                       NVARCHAR (255) NULL,
    [CompanyID]                         INT            NOT NULL,    
    [P_IoTHubEndPoint]                  NVARCHAR (50)  NULL,
    [P_IoTHubConnectionString]          NVARCHAR (256) NULL,
    [P_EventConsumerGroup]              NVARCHAR (50)  NULL,
    [P_EventHubStorageConnectionString] NVARCHAR (256) NULL,
	[P_UploadContainer]			        NVARCHAR (50)  NULL, 
    [S_IoTHubEndPoint]                  NVARCHAR (50)  NULL,
    [S_IoTHubConnectionString]          NVARCHAR (256) NULL,
    [S_EventConsumerGroup]              NVARCHAR (50)  NULL,
    [S_EventHubStorageConnectionString] NVARCHAR (256) NULL,
	[S_UploadContainer]			        NVARCHAR (50)  NULL, 
    [CreatedAt]                         DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]                         DATETIME       NULL,
    [DeletedFlag]                       BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_IoTHub] PRIMARY KEY CLUSTERED ([IoTHubAlias] ASC),
    CONSTRAINT [FK_IoTHub_ToTable] FOREIGN KEY ([CompanyID]) REFERENCES [SmartFactory].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_IoTHub_Column]
    ON [SmartFactory].[IoTHub]([DeletedFlag] ASC, [CompanyID] ASC);

Go
CREATE Unique INDEX [IX_IoTHub_Unique] ON [SmartFactory].[IoTHub] (P_IoTHubConnectionString, P_EventConsumerGroup)
Go

IF OBJECT_ID(N'[SmartFactory].[Factory]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[Factory];
END
CREATE TABLE [SmartFactory].[Factory] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Description] NVARCHAR (255) NULL,
    [CompanyId]   INT            DEFAULT ((0)) NOT NULL,
    [Latitude]    FLOAT (53)     DEFAULT ((0)) NULL,
    [Longitude]   FLOAT (53)     DEFAULT ((0)) NULL,
    [PhotoURL]    NVARCHAR (255) NULL,
    [TimeZone]    INT            DEFAULT ((0)) NOT NULL,
    [CultureInfo] NVARCHAR (10)  NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME       NULL,
    [DeletedFlag] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Factory_ToTable_1] FOREIGN KEY ([CultureInfo]) REFERENCES [SmartFactory].[RefCultureInfo] ([CultureCode]),
    CONSTRAINT [FK_Factory_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [SmartFactory].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Factory_Column]
    ON [SmartFactory].[Factory]([CompanyId] ASC, [DeletedFlag] ASC);


IF OBJECT_ID(N'[SmartFactory].[IoTDevice]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[IoTDevice];
END
CREATE TABLE [SmartFactory].[IoTDevice] (
    [IoTHubDeviceID]            NVARCHAR (128)  NOT NULL,
    [IoTHubDevicePW]            NVARCHAR (128)  NULL,
    [IoTHubDeviceKey]           NVARCHAR (128)  NULL,
    [IoTHubAlias]               NVARCHAR (50)   NOT NULL,
    [IoTHubProtocol]            NVARCHAR (128)  NULL,
    [FactoryID]                 INT             NULL,
    [AuthenticationType]        NVARCHAR (20)   NULL,
    [DeviceCertificateID]       INT             NULL,
    [DeviceTypeId]              INT             NOT NULL,
    [DeviceVendor]              NVARCHAR (128)  NULL,
    [DeviceModel]               NVARCHAR (128)  NULL,
	[MessageConvertScript]	    NVARCHAR (Max)  NULL,
	[EnableMessageConvert]      BIT      DEFAULT ((0)) NOT NULL,      
    [DeviceTwinsDesired]        NVARCHAR (1000) NULL,
    [DeviceTwinsReported]       NVARCHAR (1000) NULL,
    [DeviceConfigurationStatus] INT             CONSTRAINT [DF_IoTDevice_DeviceConfigurationStatus] DEFAULT ((0)) NOT NULL,
    [CreatedAt]                 DATETIME        CONSTRAINT [DF__IoTDevice__Creat__1F198FD4] DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]                 DATETIME        NULL,
    [DeletedFlag]               BIT             CONSTRAINT [DF__IoTDevice__Delet__200DB40D] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK__IoTDevic__8A1E2A4B02FB5B3C] PRIMARY KEY CLUSTERED ([IoTHubDeviceID] ASC),
    CONSTRAINT [FK_IoTDevice_ToTable] FOREIGN KEY ([IoTHubAlias]) REFERENCES [SmartFactory].[IoTHub] ([IoTHubAlias]),
    CONSTRAINT [FK_IoTDevice_ToTable_1] FOREIGN KEY ([DeviceTypeId]) REFERENCES [SmartFactory].[DeviceType] ([Id]),
    CONSTRAINT [FK_IoTDevice_ToTable_2] FOREIGN KEY ([FactoryID]) REFERENCES [SmartFactory].[Factory] ([Id]),
    CONSTRAINT [FK_IoTDevice_ToTable_3] FOREIGN KEY ([DeviceCertificateID]) REFERENCES [SmartFactory].[DeviceCertificate] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Devices_Column]
    ON [SmartFactory].[IoTDevice]([FactoryID] ASC, [DeletedFlag] ASC);

IF OBJECT_ID(N'[SmartFactory].[Equipment]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[Equipment];
END
CREATE TABLE [SmartFactory].[Equipment] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [EquipmentId]      NVARCHAR (50)  NOT NULL,
    [Name]             NVARCHAR (50)  NOT NULL,
    [EquipmentClassId] INT            NOT NULL,
    [FactoryId]        INT            NOT NULL,
    [IoTHubDeviceID]   NVARCHAR (128) NOT NULL,
    [Latitude]         FLOAT (53)     NULL,
    [Longitude]        FLOAT (53)     NULL,
	[MaxIdleInSec]	   INT			  DEFAUlT ((30)) NOT NULL,
    [PhotoURL]         NVARCHAR (255) NULL,
    [CreatedAt]        DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]        DATETIME       NULL,
    [DeletedFlag]      BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Equipment_ToTable_2] FOREIGN KEY ([EquipmentClassId]) REFERENCES [SmartFactory].[EquipmentClass] ([Id]),
    CONSTRAINT [FK_Equipment_ToTable_1] FOREIGN KEY ([IoTHubDeviceID]) REFERENCES [SmartFactory].[IoTDevice] ([IoTHubDeviceID]),
    CONSTRAINT [FK_Equipment_ToTable] FOREIGN KEY ([FactoryId]) REFERENCES [SmartFactory].[Factory] ([Id])
);


GO
CREATE Unique NONCLUSTERED INDEX [IX_Equipment_Column]
    ON [SmartFactory].[Equipment]([EquipmentId] ASC);


GO
CREATE INDEX [IX_Equipment_Column_1] ON [SmartFactory].[Equipment] (FactoryId, equipmentId, deletedflag)



IF OBJECT_ID(N'[SmartFactory].[IoTDeviceConfiguration]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[IoTDeviceConfiguration];
END
CREATE TABLE [SmartFactory].[IoTDeviceConfiguration] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (50)  NOT NULL,
    [DataType]     NVARCHAR (10)  NULL,
    [Description]  NVARCHAR (255) NULL,
    [DefaultValue] NVARCHAR (50)  NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);



GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_IoTDeviceConfiguration_Name]
    ON [SmartFactory].[IoTDeviceConfiguration]([Name] ASC);


IF OBJECT_ID(N'[SmartFactory].[IoTDeviceCustomizedConfiguration]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[IoTDeviceCustomizedConfiguration];
END
CREATE TABLE [SmartFactory].[IoTDeviceCustomizedConfiguration] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]    INT            NOT NULL,
    [Name]         NVARCHAR (50)  NOT NULL,
    [DataType]     NVARCHAR (10)  NOT NULL,
    [Description]  NVARCHAR (255) NULL,
    [DefaultValue] NVARCHAR (50)  NULL,
    [DeleteFlag]   BIT            CONSTRAINT [DF_IoTDeviceCustomizedConfiguration_DeleteFlag] DEFAULT ((0)) NOT NULL,
    [CreatedAt]    DATETIME       NOT NULL,
    [UpdatedAt]    DATETIME       NULL,
    CONSTRAINT [PK_IoTDeviceCustomizedConfiguration] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Company_IoTDeviceCustomizedConfiguration] FOREIGN KEY ([CompanyId]) REFERENCES [SmartFactory].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_IoTDeviceCustomizedConfiguration]
    ON [SmartFactory].[IoTDeviceCustomizedConfiguration]([CompanyId] ASC);


IF OBJECT_ID(N'[SmartFactory].[IoTDeviceMessageCatalog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[IoTDeviceMessageCatalog];
END
CREATE TABLE [SmartFactory].[IoTDeviceMessageCatalog] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [IoTHubDeviceID]   NVARCHAR (128) NOT NULL,
    [MessageCatalogID] INT            NOT NULL,
    [CreatedAt]        DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]        DATETIME       NULL,
    [DeletedFlag]      BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Table_ToTable_1] FOREIGN KEY ([MessageCatalogID]) REFERENCES [SmartFactory].[MessageCatalog] ([Id]),
    CONSTRAINT [FK_Table_ToTable] FOREIGN KEY ([IoTHubDeviceID]) REFERENCES [SmartFactory].[IoTDevice] ([IoTHubDeviceID])
);


GO
CREATE NONCLUSTERED INDEX [IX_IoTDeviceMessageCatalog_Column]
    ON [SmartFactory].[IoTDeviceMessageCatalog]([DeletedFlag] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_IoTDeviceMessageCatalog_Column_1]
    ON [SmartFactory].[IoTDeviceMessageCatalog]([IoTHubDeviceID] ASC, [MessageCatalogID] ASC);

IF OBJECT_ID(N'[SmartFactory].[UserRolePermission]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[UserRolePermission];
END
CREATE TABLE [SmartFactory].[UserRolePermission] (
    [Id]                  INT      IDENTITY (1, 1) NOT NULL,
    [UserRoleID]          INT      NOT NULL,
    [PermissionCatalogID] INT      NOT NULL,
    [CreatedAt]           DATETIME DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]           DATETIME NULL,
    [DeletedFlag]         BIT      DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRolePermission_ToTable] FOREIGN KEY ([UserRoleID]) REFERENCES [SmartFactory].[UserRole] ([Id]),
    CONSTRAINT [FK_UserRolePermission_ToTable_1] FOREIGN KEY ([PermissionCatalogID]) REFERENCES [SmartFactory].[PermissionCatalog] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_UserRolePermission_Column]
    ON [SmartFactory].[UserRolePermission]([DeletedFlag] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_U_UserRolePermission_Column_1]
    ON [SmartFactory].[UserRolePermission]([UserRoleID] ASC, [PermissionCatalogID] ASC);

IF OBJECT_ID(N'[SmartFactory].[MessageMandatoryElementDef]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[MessageMandatoryElementDef];
END
CREATE TABLE [SmartFactory].[MessageMandatoryElementDef] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [ElementName]     NVARCHAR (50) NULL,
    [ElementDataType] NVARCHAR (20) NULL,
    [MandatoryFlag]   BIT           DEFAULT ((1)) NOT NULL,
	[Description]     NVARCHAR (255) NULL,
    [CreatedAt]       DATETIME      DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]       DATETIME      NULL,
    [DeletedFlag]     BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_MessageMandatoryElementDef_Column]
    ON [SmartFactory].[MessageMandatoryElementDef]([DeletedFlag] ASC);

GO

IF OBJECT_ID(N'[SmartFactory].OperationTask', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].OperationTask;
END
CREATE TABLE SmartFactory.OperationTask
(
	[Id] INT Identity(1,1) NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [TaskStatus] NVARCHAR(20) NULL, 
    [CompletedAt] DATETIME NULL, 
    [RetryCounter] INT NULL, 
    [CompanyId] INT NOT NULL, 
    [Entity] NVARCHAR(50) NULL, 
    [EntityId] NVARCHAR(50) NULL,
	[TaskContent] NVARCHAR(1500) NULL,
	[TaskLog]     NVARCHAR (3000) NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT (getdate()), 
    [UpdatedAt] DATETIME NULL, 
    [DeletedFlag] BIT NOT NULL DEFAULT (0)
)

GO

CREATE INDEX [IX_Task_Column] ON [SmartFactory].OperationTask (TaskStatus, CompanyId, DeletedFlag)
GO

Create TRIGGER SmartFactory.TR_MessageSFMandatoryElement on SmartFactory.MessageCatalog AFTER Insert
AS
Begin
	DECLARE @ID int, @ChildMessage bit
	SET @ID = (SELECT ID FROM inserted)
	SET @ChildMessage = (SELECT ChildMessageFlag FROM inserted)

	IF (@ChildMessage = 0)
	BEGIN
		insert into SmartFactory.MessageElement (MessageCatalogID, ElementName, ElementDataType, MandatoryFlag, SFMandatoryFlag) 
			select @ID, ElementName, ElementDataType, MandatoryFlag, MandatoryFlag from SmartFactory.MessageMandatoryElementDef;
	End
End

Go

IF OBJECT_ID(N'[SmartFactory].[AlarmRuleCatalog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[AlarmRuleCatalog];
END
CREATE TABLE [SmartFactory].[AlarmRuleCatalog] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]            INT            NOT NULL,
    [MessageCatalogId]     INT            NOT NULL,
    [Name]                 NVARCHAR (50)  NOT NULL,
    [Description]          NVARCHAR (255) NULL,    
	[KeepHappenInSec]      INT			  DEFAULT ((60)) NOT NULL,
	[ActiveFlag]           BIT            DEFAULT ((0)) NOT NULL,
    [CreatedAt]            DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]            DATETIME       NULL,
    [DeletedFlag]          BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AlarmRuleCatalog_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [SmartFactory].[Company] ([Id]),
    CONSTRAINT [FK_AlarmRuleCatalog_ToTable_1] FOREIGN KEY ([MessageCatalogId]) REFERENCES [SmartFactory].[MessageCatalog] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_AlarmRuleCatalog_Column]
    ON [SmartFactory].[AlarmRuleCatalog]([CompanyId] ASC, [DeletedFlag] ASC);

Go
IF OBJECT_ID(N'[SmartFactory].[AlarmRuleItem]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[AlarmRuleItem];
END
CREATE TABLE [SmartFactory].[AlarmRuleItem] (
    [Id]                 INT           IDENTITY (1, 1) NOT NULL,
    [AlarmRuleCatalogId] INT           NOT NULL,
    [Ordering]           INT           NOT NULL,
	[MessageElementParentId] INT           NULL,
    [MessageElementId]   INT           NOT NULL,
    [EqualOperation]     NVARCHAR (20) NOT NULL,
    [Value]              NVARCHAR (50) NOT NULL,
    [BitWiseOperation]   NVARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AlarmRuleItem_ToTable] FOREIGN KEY ([AlarmRuleCatalogId]) REFERENCES [SmartFactory].[AlarmRuleCatalog] ([Id]),
	CONSTRAINT [FK_AlarmRuleItem_ToTable_1] FOREIGN KEY (MessageElementId) REFERENCES SmartFactory.MessageElement(Id),
	CONSTRAINT [FK_AlarmRuleItem_ParentMessageElement] FOREIGN KEY ([MessageElementParentId]) REFERENCES [SmartFactory].[MessageElement] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_AlarmRuleItem_Column]
    ON [SmartFactory].[AlarmRuleItem]([AlarmRuleCatalogId] ASC, [Ordering] ASC);

Go
IF OBJECT_ID(N'[SmartFactory].[ExternalApplication]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[ExternalApplication];
END
CREATE TABLE [SmartFactory].[ExternalApplication] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [CompanyId]       INT             NOT NULL,
    [Name]            NVARCHAR (50)   NOT NULL,
    [Description]     NVARCHAR (255)  NULL,
    [MessageTemplate] NVARCHAR (2000) NULL,
	[TargetType]	  NVARCHAR (50)   NOT NULL,
    [Method]          NVARCHAR (10)   NOT NULL,
    [ServiceURL]      NVARCHAR (255)  NOT NULL,
    [AuthType]        NVARCHAR (20)   NOT NULL,
    [AuthID]          NVARCHAR (50)   NULL,
    [AuthPW]          NVARCHAR (50)   NULL,
    [TokenURL]        NVARCHAR (255)  NULL,
    [HeaderValues]    NVARCHAR (255)  NULL,
    [CreatedAt]       DATETIME        DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]       DATETIME        NULL,
    [DeletedFlag]     BIT             DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ExternalApplication_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [SmartFactory].[Company] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_ExternalApplication_Column]
    ON [SmartFactory].[ExternalApplication]([CompanyId] ASC, [DeletedFlag] ASC);

Go
IF OBJECT_ID(N'[SmartFactory].[AlarmNotification]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[AlarmNotification];
END
CREATE TABLE [SmartFactory].[AlarmNotification] (
    [Id]                    INT      IDENTITY (1, 1) NOT NULL,
    [AlarmRuleCatalogId]    INT      NULL,
    [ExternalApplicationId] INT      NULL,
    [CreatedAt]             DATETIME DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]             DATETIME NULL,
    [DeletedFlag]           BIT      DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AlarmNotification_ToTable] FOREIGN KEY ([AlarmRuleCatalogId]) REFERENCES [SmartFactory].[AlarmRuleCatalog] ([Id]),
    CONSTRAINT [FK_AlarmNotification_ToTable_1] FOREIGN KEY ([ExternalApplicationId]) REFERENCES [SmartFactory].[ExternalApplication] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_AlarmNotification_Column]
    ON [SmartFactory].[AlarmNotification]([AlarmRuleCatalogId] ASC, [DeletedFlag] ASC);

Go

IF OBJECT_ID(N'[SmartFactory].[WidgetClass]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[WidgetClass];
END
CREATE TABLE [SmartFactory].[WidgetClass] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (50)  NOT NULL,
    [Key]         INT            DEFAULT ((0)) NOT NULL,
    [Level]       NVARCHAR (50)  DEFAULT ('equipment') NOT NULL,
    [PhotoURL]    NVARCHAR (255) NULL,
    [MinWidth]    INT            NULL,
    [MinHeight]   INT            NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME       NULL,
    [DeletedFlag] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Key] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_WidgetClass_Column]
    ON [SmartFactory].[WidgetClass]([Level] ASC, [DeletedFlag] ASC);

GO


IF OBJECT_ID(N'[SmartFactory].[WidgetCatalog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[WidgetCatalog];
END
CREATE TABLE [SmartFactory].[WidgetCatalog] (
    [Id]               INT             IDENTITY (1, 1) NOT NULL,
    [CompanyID]        INT             NOT NULL,
    [MessageCatalogID] INT             NULL,
    [Name]             NVARCHAR (50)   NOT NULL,
    [Level]            NVARCHAR (50)   DEFAULT ('equipment') NOT NULL,
    [WidgetClassKey]   INT             NOT NULL,
    [Context]          NVARCHAR (2000) NOT NULL,
    [CreatedAt]        DATETIME        DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]        DATETIME        NULL,
    [DeletedFlag]      BIT             DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WidgetCatalog_ToTable_2] FOREIGN KEY ([MessageCatalogID]) REFERENCES [SmartFactory].[MessageCatalog] ([Id]),
    CONSTRAINT [FK_WidgetCatalog_ToTable_1] FOREIGN KEY ([CompanyID]) REFERENCES [SmartFactory].[Company] ([Id]),
    CONSTRAINT [FK_WidgetCatalog_ToTable] FOREIGN KEY ([WidgetClassKey]) REFERENCES [SmartFactory].[WidgetClass] ([Key])
);


GO
CREATE NONCLUSTERED INDEX [IX_WidgetCatalog_Column]
    ON [SmartFactory].[WidgetCatalog]([CompanyID] ASC, [MessageCatalogID] ASC, [Level] ASC, [WidgetClassKey] ASC, [DeletedFlag] ASC);

GO

IF OBJECT_ID(N'[SmartFactory].[Dashboard]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[Dashboard];
END
CREATE TABLE [SmartFactory].[Dashboard] (
    [Id]               INT          IDENTITY (1, 1) NOT NULL,
    [CompanyID]        INT          NOT NULL,
    [DashboardType]    NVARCHAR (50) NULL,
    [FactoryID]        INT          NULL,
    [EquipmentClassID] INT          NULL,
    [EquipmentID]      INT          NULL,    
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Dashboard_ToTable] FOREIGN KEY ([CompanyID]) REFERENCES [SmartFactory].[Company] ([Id]),
    CONSTRAINT [FK_Dashboard_ToTable_1] FOREIGN KEY ([EquipmentClassID]) REFERENCES [SmartFactory].[EquipmentClass] ([Id]),
    CONSTRAINT [FK_Dashboard_ToTable_2] FOREIGN KEY ([FactoryID]) REFERENCES [SmartFactory].[Factory] ([Id]),
    CONSTRAINT [FK_Dashboard_ToTable_3] FOREIGN KEY ([EquipmentID]) REFERENCES [SmartFactory].[Equipment] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Dashboard_Column]
    ON [SmartFactory].[Dashboard]([CompanyID] ASC, [FactoryID] ASC, [EquipmentClassID] ASC, [EquipmentID] ASC);
GO

IF OBJECT_ID(N'[SmartFactory].[DashboardWidgets]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[DashboardWidgets];
END
CREATE TABLE [SmartFactory].[DashboardWidgets] (
    [Id]              INT IDENTITY (1, 1) NOT NULL,
    [DashboardID]     INT NOT NULL,
    [WidgetCatalogID] INT NOT NULL,
    [RowNo]           INT NOT NULL,
    [ColumnSeq]       INT NULL,
    [WidthSpace]      INT NOT NULL,
    [HeightPixel]     INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DashboardWidgets_ToTable_1] FOREIGN KEY ([WidgetCatalogID]) REFERENCES [SmartFactory].[WidgetCatalog] ([Id]),
    CONSTRAINT [FK_DashboardWidgets_ToTable] FOREIGN KEY ([DashboardID]) REFERENCES [SmartFactory].[Dashboard] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_DashboardWidgets_Column]
    ON [SmartFactory].[DashboardWidgets]([DashboardID] ASC, [WidgetCatalogID] ASC, [RowNo] ASC, [ColumnSeq] ASC);

GO

IF OBJECT_ID(N'[SmartFactory].[UsageLog]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[UsageLog];
END
CREATE TABLE [SmartFactory].[UsageLog] (
    [Id]            INT      IDENTITY (1, 1) NOT NULL,
    [CompanyId]     INT      NOT NULL,
    [FactoryQty]    INT      NULL,
    [EquipmentQty]  INT      NULL,
    [DeviceMessage] BIGINT   NOT NULL,
    [AlarmMessage]  BIGINT   NOT NULL,
    [DocSizeInGB]   INT      NOT NULL,
    [DocDBPercentage] INT NOT NULL, 
    [UpdatedAt] DATETIME NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UsageLog_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [SmartFactory].[Company] ([Id])
);

GO

CREATE TABLE SmartFactory.MetaDataDefination
(
	[Id] INT Identity(1,1) NOT NULL PRIMARY KEY, 
    [CompanyId] INT NOT NULL, 
    [EntityType] NVARCHAR(50) NOT NULL, 
    [ObjectName] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_MetaDataDefination_ToTable] FOREIGN KEY (CompanyId) REFERENCES SmartFactory.Company(Id) 
)

GO

CREATE unique INDEX [IX_MetaDataDefination_Column] ON [SmartFactory].[MetaDataDefination] (CompanyId, [EntityType], [ObjectName])
GO

CREATE TABLE SmartFactory.MetaDataValue
(
	[Id] INT Identity(1,1) NOT NULL PRIMARY KEY, 
    [MetaDataDefinationId] INT NOT NULL, 
    [ObjectValue] NVARCHAR(MAX) NULL, 
    [ReferenceId] INT NOT NULL, 
    CONSTRAINT [FK_MetaDataValue_ToTable] FOREIGN KEY (MetaDataDefinationId) REFERENCES SmartFactory.MetaDataDefination(ID)
)

GO

CREATE INDEX [IX_MetaDataValue_Column] ON [SmartFactory].[MetaDataValue] (MetaDataDefinationId, ReferenceId)
GO

CREATE TRIGGER SmartFactory.TR_MetaDataDef_Delete ON [SmartFactory].[MetaDataDefination] INSTEAD OF DELETE
	AS
	BEGIN
		Delete SmartFactory.MetaDataValue WHERE MetaDataDefinationId IN (SELECT deleted.Id from deleted);
		Delete SmartFactory.MetaDataDefination Where Id IN (SELECT deleted.Id from deleted);
	END
GO


CREATE INDEX [IX_UsageLog_Column] ON [SmartFactory].[UsageLog] ([UpdatedAt])
Go

CREATE VIEW SmartFactory.UsageLogSumByDay
	AS select ISNULL(ROW_NUMBER() OVER(ORDER BY MIN(Id) DESC), -1) AS Id, COUNT(*) as CompanyQty, sum(FactoryQty) as FactoryQty, sum(EquipmentQty) as EquipmentQty, sum(DeviceMessage) as DeviceMessage, dateadd(DAY,0, datediff(day,0, UpdatedAt)) as UpdatedDateTime from SmartFactory.UsageLog group by dateadd(DAY,0, datediff(day,0, UpdatedAt))
Go

CREATE TRIGGER SmartFactory.TR_Company on SmartFactory.Company AFTER Insert
AS
Begin
	insert into SmartFactory.UserRole(CompanyID, Name) select ID, 'Default' from inserted
	insert into SmartFactory.EquipmentClass(CompanyId, Name) select ID, 'Default' from inserted
	insert into SmartFactory.WidgetCatalog(CompanyID, Name, Level, WidgetClassKey, Context) select A.ID, B.Name, B.Level, B.[Key], '{"Title":"Title"}' from inserted A, SmartFactory.WidgetClass B where B.Level='company'
	insert into SmartFactory.WidgetCatalog(CompanyID, Name, Level, WidgetClassKey, Context) select A.ID, B.Name, B.Level, B.[Key], '{"Title":"Title"}' from inserted A, SmartFactory.WidgetClass B where B.Level='factory'
	insert into SmartFactory.Dashboard(CompanyID, DashboardType) select ID, 'company' from inserted
End
GO

CREATE TRIGGER SmartFactory.TR_Factory on SmartFactory.Factory AFTER Insert
AS
Begin	
	insert into SmartFactory.Dashboard(CompanyID , DashboardType, FactoryID) select A.CompanyId, 'factory', A.Id from inserted A
End
Go

CREATE TRIGGER SmartFactory.TR_Factory_Delete on SmartFactory.Factory AFTER Update
AS
Begin	
	Declare @DeletedFlag bit
	Declare @FactoryId Int
	Declare @BoardId Int
	Select @DeletedFlag = Inserted.DeletedFlag from Inserted

	if (@DeletedFlag=1)	
		Begin			
			Select @FactoryId = Inserted.Id from Inserted
			Select @BoardId = A.Id from SmartFactory.Dashboard A where A.FactoryID=@FactoryId and A.DashboardType='factory'
			delete SmartFactory.DashboardWidgets where DashboardID = @BoardId
			delete SmartFactory.Dashboard where Id = @BoardId
		End	
End
Go

Create TRIGGER SmartFactory.TR_CompanyDashboardWidgets on SmartFactory.Dashboard AFTER Insert
AS
Begin
	insert into SmartFactory.DashboardWidgets(DashboardID, WidgetCatalogID, RowNo, ColumnSeq, WidthSpace, HeightPixel) select A.ID, B.ID, 0, 0, 4, 5 from inserted A, SmartFactory.WidgetCatalog B Where B.CompanyID = A.CompanyID AND B.Level = A.DashboardType
End
GO

IF OBJECT_ID(N'[SmartFactory].[ExternalDashboard]', N'U') IS NOT NULL
BEGIN
  DROP TABLE [SmartFactory].[ExternalDashboard];
END
CREATE TABLE [SmartFactory].[ExternalDashboard] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [CompanyId]   INT            NOT NULL,
    [Order]       INT            NULL,
    [Name]        NVARCHAR (50)  NULL,
    [URL]         NVARCHAR (255) NULL,
    [CreatedAt]   DATETIME       DEFAULT (getdate()) NOT NULL,
    [UpdatedAt]   DATETIME       NULL,
    [DeletedFlag] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ExternalDashboard_ToTable] FOREIGN KEY ([CompanyId]) REFERENCES [SmartFactory].[Company] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_ExternalDashboard_Column]
    ON [SmartFactory].[ExternalDashboard]([DeletedFlag] ASC);
GO

CREATE TRIGGER SmartFactory.TR_EquipmentClass_Delete ON SmartFactory.[EquipmentClass] INSTEAD OF DELETE
	AS
	BEGIN
		Delete SmartFactory.Dashboard WHERE EquipmentClassID in (SELECT deleted.Id from deleted);
		Delete SmartFactory.EquipmentClass WHERE Id in (SELECT deleted.Id from deleted);
	END
GO

CREATE TRIGGER SmartFactory.TR_EquipmentClass_Insert on SmartFactory.EquipmentClass AFTER Insert
AS
Begin	
	insert into SmartFactory.Dashboard(CompanyID , DashboardType, EquipmentClassID) select A.CompanyId, 'equipmentclass', A.Id from inserted A
End
GO