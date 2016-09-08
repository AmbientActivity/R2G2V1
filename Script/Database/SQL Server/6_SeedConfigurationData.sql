USE [KeebeeAAT]
GO

-- activity types
SET IDENTITY_INSERT [dbo].[ActivityTypes] ON 
-- sensors 0 - 7
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (1, 'Sensor0', 'N/A')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (2, 'Sensor1', 'N/A')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (3, 'Sensor2', 'Fur')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (4, 'Sensor3', 'Side Button')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (5, 'Sensor4', 'Radio Knob')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (6, 'Sensor5', 'TV Knob')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (7, 'Sensor6', 'Side Button')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (8, 'Sensor7', 'Side Button')
-- inputs 0 - 7
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

-- game types
SET IDENTITY_INSERT [dbo].[GameTypes] ON 
INSERT [dbo].[GameTypes] ([Id], [Description]) VALUES (1, N'Match The Pictures')
INSERT [dbo].[GameTypes] ([Id], [Description]) VALUES (2, N'Match The Pairs')
SET IDENTITY_INSERT [dbo].[GameTypes] OFF

SET IDENTITY_INSERT [dbo].[Configs] ON 
INSERT [dbo].[Configs] ([Id], [Description], [IsActive]) VALUES (1, N'Panel Configration 1', 1)
SET IDENTITY_INSERT [dbo].[Configs] OFF

SET IDENTITY_INSERT [dbo].[ConfigDetails] ON 
-- sensors
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId]) VALUES (1, 1, 3, 3)
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId]) VALUES (2, 1, 4, 4)
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId]) VALUES (3, 1, 5, 5)
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId]) VALUES (4, 1, 6, 6)
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId]) VALUES (5, 1, 7, 7)
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId]) VALUES (6, 1, 8, 8)

-- inputs
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId]) VALUES (7,  1, 9, 1)
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId]) VALUES (8, 1, 10, 2)
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId]) VALUES (9, 1, 13, 5)
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [ActivityTypeId], [ResponseTypeId]) VALUES (10, 1, 14, 6)
SET IDENTITY_INSERT [dbo].[ConfigDetails] OFF