USE [KeebeeAAT]
GO

-- activity types
SET IDENTITY_INSERT [dbo].[PhidgetTypes] ON 
-- sensors 0 - 7
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (1, 'Sensor 0')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (2, 'Sensor 1')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (3, 'Sensor 2')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (4, 'Sensor 3')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (5, 'Sensor 4')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (6, 'Sensor 5')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (7, 'Sensor 6')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (8, 'Sensor 7')
-- inputs 0 - 7
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (9, 'Input 0')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (10, 'Input 1')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (11, 'Input 2')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (12, 'Input 3')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (13, 'Input 4')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (14, 'Input 5')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (15, 'Input 6')
INSERT [dbo].[PhidgetTypes] ([Id], [Description]) VALUES (16, 'Input 7')
SET IDENTITY_INSERT [dbo].[PhidgetTypes] OFF

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
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (1, 1, 3, 3, 'Fur')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (2, 1, 4, 4, 'Side Button')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (3, 1, 5, 5, 'Radio Knob')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (4, 1, 6, 6, 'TV Knob')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (5, 1, 7, 7, 'Side Button')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (6, 1, 8, 8, 'Side Button')

-- inputs
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (7, 1, 9, 1, 'Pull Switch')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (8, 1, 10, 2, 'Wheel')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (9, 1, 13, 5, 'Toggle Switch')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (10, 1, 14, 6, 'Toggle Switch')

--- CONFIGURATION 2
-- sensors
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (11, 2, 1, 1, 'Zipper')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (12, 2, 2, 2, 'Lead Pipe')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (13, 2, 3, 3, 'Rabbit''s Foot')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (14, 2, 4, 4, 'Side Lever')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (15, 2, 5, 5, 'Radio Buttons')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (16, 2, 6, 6, 'TV Scanner')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (17, 2, 7, 7, 'Side Button')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (18, 2, 8, 8, 'Side Button')

-- inputs
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (19, 2, 9, 1, 'Door Knob')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (20, 2, 10, 2, 'Key Turn')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (21, 2, 13, 5, 'Push Button')
INSERT [dbo].[ConfigDetails] ([Id], [ConfigId], [PhidgetTypeId], [ResponseTypeId], [Description]) VALUES (22, 2, 14, 6, 'Push Button')
SET IDENTITY_INSERT [dbo].[ConfigDetails] OFF