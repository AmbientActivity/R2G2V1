USE [KeebeeAAT]
GO

SET IDENTITY_INSERT [dbo].[Configurations] ON 
INSERT [dbo].[Configurations] ([Id], [Description], [IsActive]) VALUES (1, N'Panel Configration 1', 1)
SET IDENTITY_INSERT [dbo].[Configurations] OFF

-- activity types
SET IDENTITY_INSERT [dbo].[ActivityTypes] ON 
-- sensors
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (1, 'Sensor0', 'N/A')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (2, 'Sensor1', 'N/A')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (3, 'Sensor2', 'Fur')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (4, 'Sensor3', 'Side Button')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (5, 'Sensor4', 'Radio Knob')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (6, 'Sensor5', 'TV Knob')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (7, 'Sensor6', 'Side Button')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (8, 'Sensor7', 'Side Button')
-- inputs
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (9, 'Input0', 'Pull Switch')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (10, 'Input1', 'Wheel')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (11, 'Input2', 'N/A')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (12, 'Input3', 'N/A')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (13, 'Input4', 'Toggle Switch')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (14, 'Input5', 'Toggle Switch')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (15, 'Input6', 'N/A')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (16, 'Input7', 'N/A')
SET IDENTITY_INSERT [dbo].[ActivityTypes] OFF

-- response type categories
SET IDENTITY_INSERT [dbo].[ResponseTypeCategories] ON 
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (1, 'Image')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (2, 'Music')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (3, 'Video')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (4, 'Game')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (5, 'System')
SET IDENTITY_INSERT [dbo].[ResponseTypeCategories] OFF

-- response types
SET IDENTITY_INSERT [dbo].[ResponseTypes] ON 
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (1, 1, 'Slide Show', 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (2, 4, 'Memory Matching Game', 1, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (3, 3, 'Cats', 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (4, 5, 'Kill Display', 0, 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (5, 2, 'Radio', 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (6, 3, 'Television', 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (7, 5, 'Caregiver', 0, 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (8, 5, 'Ambient', 0, 1)
SET IDENTITY_INSERT [dbo].[ResponseTypes] OFF

TRUNCATE TABLE [dbo].[ConfigurationDetails]

SET IDENTITY_INSERT [dbo].[ConfigurationDetails] ON 
-- sensors
INSERT [dbo].[ConfigurationDetails] ([Id], [ConfigurationId], [ActivityTypeId], [ResponseTypeId]) VALUES (1, 1, 3, 3)
INSERT [dbo].[ConfigurationDetails] ([Id], [ConfigurationId], [ActivityTypeId], [ResponseTypeId]) VALUES (2, 1, 4, 4)
INSERT [dbo].[ConfigurationDetails] ([Id], [ConfigurationId], [ActivityTypeId], [ResponseTypeId]) VALUES (3, 1, 5, 5)
INSERT [dbo].[ConfigurationDetails] ([Id], [ConfigurationId], [ActivityTypeId], [ResponseTypeId]) VALUES (4, 1, 6, 6)
INSERT [dbo].[ConfigurationDetails] ([Id], [ConfigurationId], [ActivityTypeId], [ResponseTypeId]) VALUES (5, 1, 7, 7)
INSERT [dbo].[ConfigurationDetails] ([Id], [ConfigurationId], [ActivityTypeId], [ResponseTypeId]) VALUES (6, 1, 8, 8)

-- inputs
INSERT [dbo].[ConfigurationDetails] ([Id], [ConfigurationId], [ActivityTypeId], [ResponseTypeId]) VALUES (7,  1, 9, 1)
INSERT [dbo].[ConfigurationDetails] ([Id], [ConfigurationId], [ActivityTypeId], [ResponseTypeId]) VALUES (8, 1, 10, 2)
INSERT [dbo].[ConfigurationDetails] ([Id], [ConfigurationId], [ActivityTypeId], [ResponseTypeId]) VALUES (9, 1, 13, 5)
INSERT [dbo].[ConfigurationDetails] ([Id], [ConfigurationId], [ActivityTypeId], [ResponseTypeId]) VALUES (10, 1, 14, 6)
SET IDENTITY_INSERT [dbo].[ConfigurationDetails] OFF

-- profiles
SET IDENTITY_INSERT [dbo].[Profiles] ON 
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (1, N'Public Library', 1, GetDate(), GetDate())
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (2, N'Alma''s Profile', 1, GetDate(), GetDate())
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (3, N'Earl''s Profile', 2, GetDate(), GetDate())
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (4, N'Ruth''s Profile', 3, GetDate(), GetDate())
SET IDENTITY_INSERT [dbo].[Profiles] OFF

-- residents
SET IDENTITY_INSERT [dbo].[Residents] ON 
INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [DateCreated], [DateUpdated], [ProfileId]) 
VALUES (1, N'Alma', N'Robinson', N'F', GetDate(), GetDate(), 2)

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [DateCreated], [DateUpdated], [ProfileId]) 
VALUES (2, N'Earl', N'Smith', N'M', GetDate(), GetDate(), 3)

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [DateCreated], [DateUpdated], [ProfileId]) 
VALUES (3, N'Ruth', N'Chambers', N'F', GetDate(), GetDate(), 4)
SET IDENTITY_INSERT [dbo].[Residents] OFF

-- update resident profile ids
UPDATE [dbo].[Residents] SET ProfileId = 2 WHERE Id = 1
UPDATE [dbo].[Residents] SET ProfileId = 3 WHERE Id = 2
UPDATE [dbo].[Residents] SET ProfileId = 4 WHERE Id = 3

-- game types
SET IDENTITY_INSERT [dbo].[GameTypes] ON 
INSERT [dbo].[GameTypes] ([Id], [Description]) VALUES (1, N'Match The Pictures')
INSERT [dbo].[GameTypes] ([Id], [Description]) VALUES (2, N'Match The Pairs')
SET IDENTITY_INSERT [dbo].[GameTypes] OFF