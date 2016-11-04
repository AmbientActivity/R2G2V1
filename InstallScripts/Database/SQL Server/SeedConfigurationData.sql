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
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (4, 5, 'Kill Display', 0, 1)
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
INSERT [dbo].[MediaPathTypes] ([Id], [Description]) VALUES (1, 'images')
INSERT [dbo].[MediaPathTypes] ([Id], [Description]) VALUES (2, 'videos')
INSERT [dbo].[MediaPathTypes] ([Id], [Description]) VALUES (3, 'music')
INSERT [dbo].[MediaPathTypes] ([Id], [Description]) VALUES (4, 'pictures')
INSERT [dbo].[MediaPathTypes] ([Id], [Description]) VALUES (5, 'shapes')
INSERT [dbo].[MediaPathTypes] ([Id], [Description]) VALUES (6, 'sounds')
SET IDENTITY_INSERT [dbo].[MediaPathTypes] OFF

--- CONFIGURATION 1
SET IDENTITY_INSERT [dbo].[Configs] ON 
INSERT [dbo].[Configs] ([Id], [Description], [IsActive], [IsActiveEventLog]) VALUES (1, N'Default Configuration', 1, 1)
SET IDENTITY_INSERT [dbo].[Configs] OFF

-- sensors
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 3, 1, 3, 'Fur')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 4, 1, 4, 'Side Button')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 5, 2, 5, 'Radio Knob')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 6, 2, 6, 'TV Knob')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 7, 1, 7, 'Side Button')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 8, 1, 8, 'Side Button')

-- inputs
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 9, 5, 1, 'Pull Switch')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 10, 5, 2, 'Wheel')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 13, 5, 5, 'Toggle Switch')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 14, 5, 6, 'Toggle Switch')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 1, 5, 10, 'Side Button')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 16, 5, 9, 'Toggle Switch')