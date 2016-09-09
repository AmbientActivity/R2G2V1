USE [KeebeeAAT]
GO

-- activity types
SET IDENTITY_INSERT [dbo].[ActivityTypes] ON 
-- sensors 0 - 7
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (1, 'Sensor0')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (2, 'Sensor1')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (3, 'Sensor2')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (4, 'Sensor3')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (5, 'Sensor4')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (6, 'Sensor5')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (7, 'Sensor6')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (8, 'Sensor7')
-- inputs 0 - 7
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (9, 'Input0')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (10, 'Input1')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (11, 'Input2')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (12, 'Input3')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (13, 'Input4')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (14, 'Input5')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (15, 'Input6')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType]) VALUES (16, 'Input7')
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
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (3, 3, 'Cats Video', 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (4, 5, 'Kill Display', 0, 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (5, 2, 'Radio', 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (6, 3, 'Television', 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (7, 5, 'Caregiver', 0, 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (8, 5, 'Ambient', 0, 1)
SET IDENTITY_INSERT [dbo].[ResponseTypes] OFF

-- game types
SET IDENTITY_INSERT [dbo].[GameTypes] ON 
INSERT [dbo].[GameTypes] ([Id], [Description]) VALUES (1, N'Match The Pictures')
INSERT [dbo].[GameTypes] ([Id], [Description]) VALUES (2, N'Match The Pairs')
SET IDENTITY_INSERT [dbo].[GameTypes] OFF

SET IDENTITY_INSERT [dbo].[Configs] ON 
INSERT [dbo].[Configs] ([Id], [Description], [IsActive]) VALUES (1, N'Panel Configuration 1', 1)
INSERT [dbo].[Configs] ([Id], [Description], [IsActive]) VALUES (2, N'Panel Configuration 2', 0)
SET IDENTITY_INSERT [dbo].[Configs] OFF

SET IDENTITY_INSERT [dbo].[ConfigDetails] ON
--- CONFIGURATION 1
-- sensors
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (1, 1, 3, 3, 'Fur')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (2, 1, 4, 4, 'Side Button')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (3, 1, 5, 5, 'Radio Knob')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (4, 1, 6, 6, 'TV Knob')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (5, 1, 7, 7, 'Side Button')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (6, 1, 8, 8, 'Side Button')

-- inputs
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (7, 1, 9, 1, 'Pull Switch')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (8, 1, 10, 2, 'Wheel')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (9, 1, 13, 5, 'Toggle Switch')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (10, 1, 14, 6, 'Toggle Switch')

--- CONFIGURATION 2
-- sensors
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (11, 2, 1, 1, 'Zipper')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (12, 2, 2, 2, 'Lead Pipe')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (13, 2, 3, 3, 'Bear''s Claw')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (14, 2, 4, 4, 'Side Button')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (15, 2, 5, 5, 'Radio Dial')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (16, 2, 6, 6, 'TV Dial')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (17, 2, 7, 7, 'Side Button')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (18, 2, 8, 8, 'Side Button')

-- inputs
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (19, 2, 9, 1, 'Knife Handle')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (20, 2, 10, 2, 'Shark''s Fin')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (21, 2, 13, 5, 'Toggle Switch')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId], [ActivityTypeDesc]) VALUES (22, 2, 14, 6, 'Toggle Switch')
SET IDENTITY_INSERT [dbo].[ConfigDetails] OFF