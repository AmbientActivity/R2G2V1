USE [KeebeeAAT]
GO

-- active resident
SET IDENTITY_INSERT [dbo].[ActiveResidents] ON 
INSERT [dbo].[ActiveResidents] ([Id], [ResidentId]) VALUES (1, null)
SET IDENTITY_INSERT [dbo].[ActiveResidents] OFF

-- roles
SET IDENTITY_INSERT [dbo].[Roles] ON 
INSERT [dbo].[Roles] ([Id], [Description]) VALUES (1, 'Administrator')
INSERT [dbo].[Roles] ([Id], [Description]) VALUES (2, 'Caregiver')
SET IDENTITY_INSERT [dbo].[Roles] OFF

-- users
SET IDENTITY_INSERT [dbo].[Users] ON 
INSERT [dbo].[Users] ([Id], [Username], [Password]) VALUES (1, 'admin', '24B0712E91489671013C3BC67D4EC894') -- c@regiver
INSERT [dbo].[Users] ([Id], [Username], [Password]) VALUES (2, 'caregiver', 'F11CE51888FFA4F1D96CBE1C1AA0C4DF') -- @dmin
SET IDENTITY_INSERT [dbo].[Users] OFF

-- user roles
SET IDENTITY_INSERT [dbo].[UserRoles] ON 
INSERT [dbo].[UserRoles] ([Id], [UserId], [RoleId]) VALUES (1, 1, 1)
INSERT [dbo].[UserRoles] ([Id], [UserId], [RoleId]) VALUES (2, 1, 2)
INSERT [dbo].[UserRoles] ([Id], [UserId], [RoleId]) VALUES (3, 2, 2)
SET IDENTITY_INSERT [dbo].[UserRoles] OFF

-- phidget style types
SET IDENTITY_INSERT [dbo].[PhidgetStyleTypes] ON 
INSERT [dbo].[PhidgetStyleTypes] ([Id], [Description]) VALUES (1, 'Touch')
INSERT [dbo].[PhidgetStyleTypes] ([Id], [Description]) VALUES (2, 'Multi-turn')
INSERT [dbo].[PhidgetStyleTypes] ([Id], [Description]) VALUES (3, 'Stop-turn')
INSERT [dbo].[PhidgetStyleTypes] ([Id], [Description]) VALUES (4, 'Slider')
INSERT [dbo].[PhidgetStyleTypes] ([Id], [Description]) VALUES (5, 'On/Off')
SET IDENTITY_INSERT [dbo].[PhidgetStyleTypes] OFF

-- phidget types
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
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (4, 5, 'Kill Display',  0, 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (5, 2, 'Radio', 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (6, 3, 'Television', 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (7, 5, 'Caregiver', 0, 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (8, 5, 'Ambient', 0, 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (9, 5, 'Off Screen', 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (10, 5, 'Volume Control', 0, 1)
SET IDENTITY_INSERT [dbo].[ResponseTypes] OFF
UPDATE [dbo].[ResponseTypes] SET IsSystem = 0 WHERE Id = 9
-- game types
SET IDENTITY_INSERT [dbo].[GameTypes] ON 
INSERT [dbo].[GameTypes] ([Id], [Description]) VALUES (1, N'Match The Pictures')
INSERT [dbo].[GameTypes] ([Id], [Description]) VALUES (2, N'Match The Pairs')
SET IDENTITY_INSERT [dbo].[GameTypes] OFF

-- media path types
SET IDENTITY_INSERT [dbo].[MediaPathTypes] ON 
INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription]) VALUES (1, 'audio\music', 'Audio (Music)', 'Music')
INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription]) VALUES (2, 'audio\radio-shows', 'Audio (Radio Shows)', 'Radio Shows')
INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription]) VALUES (3, 'images\general', 'Images (General)', 'Images')
INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription]) VALUES (4, 'images\personal', 'Images (Personal)', 'Images')
INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription]) VALUES (5, 'videos\tv-shows', 'Videos (TV Shows)', 'TV Shows')
INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription]) VALUES (6, 'videos\home-movies', 'Videos (Home Movies)', 'Home Movies')
INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription]) VALUES (7, 'videos\system', 'Videos (System)', 'Videos')
INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription]) VALUES (8, 'activities\matching-game\shapes', 'Matching Game (Shapes)', 'Shapes')
INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription]) VALUES (9, 'activities\matching-game\sounds', 'Matching Game (Sounds)', 'Sounds')
SET IDENTITY_INSERT [dbo].[MediaPathTypes] OFF

--- CONFIGURATION 1
SET IDENTITY_INSERT [dbo].[Configs] ON 
INSERT [dbo].[Configs] ([Id], [Description], [IsActive], [IsActiveEventLog]) VALUES (1, N'Default Configuration', 1, 1)
SET IDENTITY_INSERT [dbo].[Configs] OFF

-- sensors
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 1, 1, 10, 'Panel', 'Top Right')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 2, 1, 7, 'Panel', 'Top Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 3, 1, 3, 'Fur', 'Bottom Right')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 4, 5, 5, 'Radio Knob', 'Middle Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 5, 5, 6, 'TV Knob', 'Middle Right')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 8, 1, 4, 'Panel', 'Side Right')

-- inputs
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 9, 5, 1, 'Wheel', 'Bottom Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 10, 5, 2, 'Toggle', 'Bottom Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 12, 5, 5, 'Toggle', 'Bottom Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 13, 5, 6, 'Toggle', 'Bottom Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 16, 5, 9, 'Left-Right Switch', null)