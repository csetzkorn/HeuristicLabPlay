USE [HeuristicLab.OKB]
GO
/****** Object:  Table [dbo].[ValueName]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ValueName](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Category] [tinyint] NOT NULL,
	[Type] [tinyint] NOT NULL,
 CONSTRAINT [PK_ValueName_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProblemClass]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProblemClass](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_ProblemClass_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ProblemClass_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ProblemClass] ON
INSERT [dbo].[ProblemClass] ([Id], [Name], [Description]) VALUES (1, N'Unknown', N'Unknown or undefined problem class.')
SET IDENTITY_INSERT [dbo].[ProblemClass] OFF
/****** Object:  Table [dbo].[Characteristic]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Characteristic](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Type] [tinyint] NOT NULL,
 CONSTRAINT [PK_Characteristic_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BinaryData]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BinaryData](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Data] [varbinary](max) NOT NULL,
	[Hash] [varbinary](20) NOT NULL,
 CONSTRAINT [PK_BinaryData_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_BinaryData_Hash] UNIQUE NONCLUSTERED 
(
	[Hash] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AlgorithmClass]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AlgorithmClass](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_AlgorithmClass_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_AlgorithmClass_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AlgorithmClass] ON
INSERT [dbo].[AlgorithmClass] ([Id], [Name], [Description]) VALUES (1, N'Unknown', N'Unknown or undefined algorithm class.')
SET IDENTITY_INSERT [dbo].[AlgorithmClass] OFF
/****** Object:  Table [dbo].[Platform]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Platform](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_Platform_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Platform_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Platform] ON
INSERT [dbo].[Platform] ([Id], [Name], [Description]) VALUES (1, N'Unknown', N'Unknown or undefined platform.')
INSERT [dbo].[Platform] ([Id], [Name], [Description]) VALUES (2, N'HeuristicLab 3.3', NULL)
SET IDENTITY_INSERT [dbo].[Platform] OFF
/****** Object:  Table [dbo].[DataType]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataType](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[TypeName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_DataType_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[DataType] ON
INSERT [dbo].[DataType] ([Id], [Name], [TypeName]) VALUES (1, N'Unknown', N'Unknown')
SET IDENTITY_INSERT [dbo].[DataType] OFF
/****** Object:  Table [dbo].[Problem]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Problem](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ProblemClassId] [bigint] NOT NULL,
	[PlatformId] [bigint] NOT NULL,
	[DataTypeId] [bigint] NOT NULL,
	[BinaryDataId] [bigint] NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_Problem_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Problem_Name_PlatformId] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[PlatformId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Algorithm]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Algorithm](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AlgorithmClassId] [bigint] NOT NULL,
	[PlatformId] [bigint] NOT NULL,
	[DataTypeId] [bigint] NOT NULL,
	[BinaryDataId] [bigint] NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_Algorithm_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Algorithm_Name_PlatformId] UNIQUE NONCLUSTERED 
(
	[Name] ASC,
	[PlatformId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CharacteristicValue]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CharacteristicValue](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ProblemId] [bigint] NOT NULL,
	[CharacteristicId] [bigint] NOT NULL,
	[DataTypeId] [bigint] NOT NULL,
	[BoolValue] [bit] SPARSE  NULL,
	[IntValue] [int] SPARSE  NULL,
	[LongValue] [bigint] SPARSE  NULL,
	[FloatValue] [real] SPARSE  NULL,
	[DoubleValue] [float] SPARSE  NULL,
	[StringValue] [nvarchar](max) SPARSE  NULL,
 CONSTRAINT [PK_CharacteristicValue_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AlgorithmUser]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AlgorithmUser](
	[AlgorithmId] [bigint] NOT NULL,
	[UserGroupId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_AlgorithmUser_AlgorithmId_UserId] PRIMARY KEY CLUSTERED 
(
	[AlgorithmId] ASC,
	[UserGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Run]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Run](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AlgorithmId] [bigint] NOT NULL,
	[ProblemId] [bigint] NOT NULL,	
	[CreatedDate] [datetime2](7) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ClientId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Run_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProblemUser]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProblemUser](
	[ProblemId] [bigint] NOT NULL,
	[UserGroupId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_ProblemUser_ProblemId_UserId] PRIMARY KEY CLUSTERED 
(
	[ProblemId] ASC,
	[UserGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Value]    Script Date: 01/31/2011 02:17:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Value](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RunId] [bigint] NOT NULL,
	[ValueNameId] [bigint] NOT NULL,
	[DataTypeId] [bigint] NOT NULL,
	[BoolValue] [bit] SPARSE  NULL,
	[IntValue] [int] SPARSE  NULL,
	[LongValue] [bigint] SPARSE  NULL,
	[FloatValue] [real] SPARSE  NULL,
	[DoubleValue] [float] SPARSE  NULL,
	[StringValue] [nvarchar](max) SPARSE  NULL,
	[BinaryDataId] [bigint] SPARSE  NULL,
 CONSTRAINT [PK_Value_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[SingleObjectiveSolution](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ProblemId] [bigint] NULL,
	[DataTypeId] [bigint] NULL,
	[BinaryDataId] [bigint] NULL,
	[RunId] [bigint] NULL,
	[Quality] [float] NOT NULL,
 CONSTRAINT [PK_SingleObjectiveSolution] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_AlgorithmClass_Algorithm]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Algorithm]  WITH CHECK ADD  CONSTRAINT [FK_AlgorithmClass_Algorithm] FOREIGN KEY([AlgorithmClassId])
REFERENCES [dbo].[AlgorithmClass] ([Id])
GO
ALTER TABLE [dbo].[Algorithm] CHECK CONSTRAINT [FK_AlgorithmClass_Algorithm]
GO
/****** Object:  ForeignKey [FK_BinaryData_Algorithm]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Algorithm]  WITH CHECK ADD  CONSTRAINT [FK_BinaryData_Algorithm] FOREIGN KEY([BinaryDataId])
REFERENCES [dbo].[BinaryData] ([Id])
GO
ALTER TABLE [dbo].[Algorithm] CHECK CONSTRAINT [FK_BinaryData_Algorithm]
GO
/****** Object:  ForeignKey [FK_DataType_Algorithm]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Algorithm]  WITH CHECK ADD  CONSTRAINT [FK_DataType_Algorithm] FOREIGN KEY([DataTypeId])
REFERENCES [dbo].[DataType] ([Id])
GO
ALTER TABLE [dbo].[Algorithm] CHECK CONSTRAINT [FK_DataType_Algorithm]
GO
/****** Object:  ForeignKey [FK_Platform_Algorithm]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Algorithm]  WITH CHECK ADD  CONSTRAINT [FK_Platform_Algorithm] FOREIGN KEY([PlatformId])
REFERENCES [dbo].[Platform] ([Id])
GO
ALTER TABLE [dbo].[Algorithm] CHECK CONSTRAINT [FK_Platform_Algorithm]
GO
/****** Object:  ForeignKey [FK_Algorithm_AlgorithmUser]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[AlgorithmUser]  WITH CHECK ADD  CONSTRAINT [FK_Algorithm_AlgorithmUser] FOREIGN KEY([AlgorithmId])
REFERENCES [dbo].[Algorithm] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AlgorithmUser] CHECK CONSTRAINT [FK_Algorithm_AlgorithmUser]
GO
/****** Object:  ForeignKey [FK_Characteristic_CharacteristicValue]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[CharacteristicValue]  WITH CHECK ADD  CONSTRAINT [FK_Characteristic_CharacteristicValue] FOREIGN KEY([CharacteristicId])
REFERENCES [dbo].[Characteristic] ([Id])
GO
ALTER TABLE [dbo].[CharacteristicValue] CHECK CONSTRAINT [FK_Characteristic_CharacteristicValue]
GO
/****** Object:  ForeignKey [FK_DataType_CharacteristicValue]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[CharacteristicValue]  WITH CHECK ADD  CONSTRAINT [FK_DataType_CharacteristicValue] FOREIGN KEY([DataTypeId])
REFERENCES [dbo].[DataType] ([Id])
GO
ALTER TABLE [dbo].[CharacteristicValue] CHECK CONSTRAINT [FK_DataType_CharacteristicValue]
GO
/****** Object:  ForeignKey [FK_Problem_CharacteristicValue]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[CharacteristicValue]  WITH CHECK ADD  CONSTRAINT [FK_Problem_CharacteristicValue] FOREIGN KEY([ProblemId])
REFERENCES [dbo].[Problem] ([Id])
GO
ALTER TABLE [dbo].[CharacteristicValue] CHECK CONSTRAINT [FK_Problem_CharacteristicValue]
GO
/****** Object:  ForeignKey [FK_BinaryData_Problem]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Problem]  WITH CHECK ADD  CONSTRAINT [FK_BinaryData_Problem] FOREIGN KEY([BinaryDataId])
REFERENCES [dbo].[BinaryData] ([Id])
GO
ALTER TABLE [dbo].[Problem] CHECK CONSTRAINT [FK_BinaryData_Problem]
GO
/****** Object:  ForeignKey [FK_DataType_Problem]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Problem]  WITH CHECK ADD  CONSTRAINT [FK_DataType_Problem] FOREIGN KEY([DataTypeId])
REFERENCES [dbo].[DataType] ([Id])
GO
ALTER TABLE [dbo].[Problem] CHECK CONSTRAINT [FK_DataType_Problem]
GO
/****** Object:  ForeignKey [FK_Platform_Problem]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Problem]  WITH CHECK ADD  CONSTRAINT [FK_Platform_Problem] FOREIGN KEY([PlatformId])
REFERENCES [dbo].[Platform] ([Id])
GO
ALTER TABLE [dbo].[Problem] CHECK CONSTRAINT [FK_Platform_Problem]
GO
/****** Object:  ForeignKey [FK_ProblemClass_Problem]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Problem]  WITH CHECK ADD  CONSTRAINT [FK_ProblemClass_Problem] FOREIGN KEY([ProblemClassId])
REFERENCES [dbo].[ProblemClass] ([Id])
GO
ALTER TABLE [dbo].[Problem] CHECK CONSTRAINT [FK_ProblemClass_Problem]
GO
/****** Object:  ForeignKey [FK_Problem_ProblemUser]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[ProblemUser]  WITH CHECK ADD  CONSTRAINT [FK_Problem_ProblemUser] FOREIGN KEY([ProblemId])
REFERENCES [dbo].[Problem] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProblemUser] CHECK CONSTRAINT [FK_Problem_ProblemUser]
GO
/****** Object:  ForeignKey [FK_Algorithm_Run]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Run]  WITH CHECK ADD  CONSTRAINT [FK_Algorithm_Run] FOREIGN KEY([AlgorithmId])
REFERENCES [dbo].[Algorithm] ([Id])
GO
ALTER TABLE [dbo].[Run] CHECK CONSTRAINT [FK_Algorithm_Run]
GO
/****** Object:  ForeignKey [FK_Problem_Run]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Run]  WITH CHECK ADD  CONSTRAINT [FK_Problem_Run] FOREIGN KEY([ProblemId])
REFERENCES [dbo].[Problem] ([Id])
GO
ALTER TABLE [dbo].[Run] CHECK CONSTRAINT [FK_Problem_Run]
GO
/****** Object:  ForeignKey [FK_BinaryData_Value]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Value]  WITH CHECK ADD  CONSTRAINT [FK_BinaryData_Value] FOREIGN KEY([BinaryDataId])
REFERENCES [dbo].[BinaryData] ([Id])
GO
ALTER TABLE [dbo].[Value] CHECK CONSTRAINT [FK_BinaryData_Value]
GO
/****** Object:  ForeignKey [FK_DataType_Value]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Value]  WITH CHECK ADD  CONSTRAINT [FK_DataType_Value] FOREIGN KEY([DataTypeId])
REFERENCES [dbo].[DataType] ([Id])
GO
ALTER TABLE [dbo].[Value] CHECK CONSTRAINT [FK_DataType_Value]
GO
/****** Object:  ForeignKey [FK_Run_Value]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Value]  WITH CHECK ADD  CONSTRAINT [FK_Run_Value] FOREIGN KEY([RunId])
REFERENCES [dbo].[Run] ([Id])
GO
ALTER TABLE [dbo].[Value] CHECK CONSTRAINT [FK_Run_Value]
GO
/****** Object:  ForeignKey [FK_ValueName_Value]    Script Date: 01/31/2011 02:17:22 ******/
ALTER TABLE [dbo].[Value]  WITH CHECK ADD  CONSTRAINT [FK_ValueName_Value] FOREIGN KEY([ValueNameId])
REFERENCES [dbo].[ValueName] ([Id])
GO
ALTER TABLE [dbo].[Value] CHECK CONSTRAINT [FK_ValueName_Value]
GO
ALTER TABLE [dbo].[SingleObjectiveSolution]  WITH CHECK ADD  CONSTRAINT [FK_SingleObjectiveSolution_BinaryData] FOREIGN KEY([BinaryDataId])
REFERENCES [dbo].[BinaryData] ([Id])
GO
ALTER TABLE [dbo].[SingleObjectiveSolution] CHECK CONSTRAINT [FK_SingleObjectiveSolution_BinaryData]
GO
ALTER TABLE [dbo].[SingleObjectiveSolution]  WITH CHECK ADD  CONSTRAINT [FK_SingleObjectiveSolution_DataType] FOREIGN KEY([DataTypeId])
REFERENCES [dbo].[DataType] ([Id])
GO
ALTER TABLE [dbo].[SingleObjectiveSolution] CHECK CONSTRAINT [FK_SingleObjectiveSolution_DataType]
GO
ALTER TABLE [dbo].[SingleObjectiveSolution]  WITH CHECK ADD  CONSTRAINT [FK_SingleObjectiveSolution_Problem] FOREIGN KEY([ProblemId])
REFERENCES [dbo].[Problem] ([Id])
GO
ALTER TABLE [dbo].[SingleObjectiveSolution] CHECK CONSTRAINT [FK_SingleObjectiveSolution_Problem]
GO
ALTER TABLE [dbo].[SingleObjectiveSolution]  WITH CHECK ADD  CONSTRAINT [FK_SingleObjectiveSolution_Run] FOREIGN KEY([RunId])
REFERENCES [dbo].[Run] ([Id])
GO
ALTER TABLE [dbo].[SingleObjectiveSolution] CHECK CONSTRAINT [FK_SingleObjectiveSolution_Run]
GO
/****** Object:  Index [RunValue]    Script Date: 03/09/2016 17:04:01 ******/
CREATE NONCLUSTERED INDEX [RunValue] ON [dbo].[Value] 
(
	[RunId] ASC
)
INCLUDE ( [Id],
[ValueNameId],
[DataTypeId],
[BoolValue],
[IntValue],
[LongValue],
[FloatValue],
[DoubleValue],
[StringValue],
[BinaryDataId]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Index [ValueNameValue]    Script Date: 03/09/2016 17:05:09 ******/
CREATE NONCLUSTERED INDEX [ValueNameValue] ON [dbo].[Value] 
(
	[ValueNameId] ASC
)
INCLUDE ( [DataTypeId]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Index [ProblemCharacteristicValue]    Script Date: 03/09/2016 17:07:54 ******/
CREATE NONCLUSTERED INDEX [ProblemCharacteristicValue] ON [dbo].[CharacteristicValue] 
(
	[ProblemId] ASC
)
INCLUDE ( [Id],
[CharacteristicId],
[DataTypeId],
[BoolValue],
[IntValue],
[LongValue],
[FloatValue],
[DoubleValue],
[StringValue]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Index [CharacteristicCharacteristicValue]    Script Date: 03/09/2016 17:08:15 ******/
CREATE NONCLUSTERED INDEX [CharacteristicCharacteristicValue] ON [dbo].[CharacteristicValue] 
(
	[CharacteristicId] ASC
)
INCLUDE ( [DataTypeId]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Index [SolutionRunId]    Script Date: 03/10/2016 15:01:59 ******/
CREATE NONCLUSTERED INDEX [SolutionRunId] ON [dbo].[SingleObjectiveSolution] 
(
	[RunId] ASC
)
INCLUDE ( [Id],
[ProblemId],
[BinaryDataId],
[Quality],
[DataTypeId]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Index [SolutionProblemId]    Script Date: 03/10/2016 15:01:57 ******/
CREATE NONCLUSTERED INDEX [SolutionProblemId] ON [dbo].[SingleObjectiveSolution] 
(
	[ProblemId] ASC
)
INCLUDE ( [Id],
[BinaryDataId],
[RunId],
[Quality],
[DataTypeId]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
