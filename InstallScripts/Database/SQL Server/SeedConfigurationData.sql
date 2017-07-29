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
INSERT [dbo].[Users] ([Id], [Username], [Password]) VALUES (1, 'admin', '24B0712E91489671013C3BC67D4EC894') -- @dmin
INSERT [dbo].[Users] ([Id], [Username], [Password]) VALUES (2, 'caregiver', '81DC9BDB52D04DC20036DBD8313ED055') -- 1234
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
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (2, 'Audio')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (3, 'Video')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (4, 'Activity')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (5, 'System')
SET IDENTITY_INSERT [dbo].[ResponseTypeCategories] OFF

-- game types
SET IDENTITY_INSERT [dbo].[InteractiveActivityTypes] ON 
INSERT [dbo].[InteractiveActivityTypes] ([Id], [Description], [SwfFile]) VALUES (1, N'Matching Game', 'MatchingGame.swf')
INSERT [dbo].[InteractiveActivityTypes] ([Id], [Description], [SwfFile]) VALUES (2, N'Painting Activity', 'PaintingActivity.swf')
INSERT [dbo].[InteractiveActivityTypes] ([Id], [Description], [SwfFile]) VALUES (3, N'Balloon Popping Game', 'BalloonPoppingGame.swf')
SET IDENTITY_INSERT [dbo].[InteractiveActivityTypes] OFF

-- response types
SET IDENTITY_INSERT [dbo].[ResponseTypes] ON 
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (1, 1, 'Slide Show', null, 0, 1, 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (2, 4, 'Memory Matching Game', 1, 0, 1, 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (3, 3, 'Cats', null, 0, 0, 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (4, 5, 'Kill Display',  null, 1, 0, 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (5, 2, 'Radio', null, 0, 0, 1, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (6, 3, 'Television', null, 0, 0, 1, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (7, 5, 'Caregiver', null, 1, 0, 0, 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (8, 5, 'Ambient', null, 1, 0, 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (9, 5, 'Off Screen', null, 0, 0, 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (10, 5, 'Volume Control', null, 1, 0, 0, 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (11, 4, 'Painting Activity', 2, 0, 1, 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (12, 4, 'Balloon Popping Game', 3, 0, 1, 0, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (13, 3, 'Nature', null, 0, 0, 1, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (14, 3, 'Sports', null, 0, 0, 1, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (15, 3, 'Machinery', null, 0, 0, 1, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (16, 3, 'Animals', null, 0, 0, 1, 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [InteractiveActivityTypeId], [IsSystem], [IsRandom], [IsRotational], [IsUninterrupted]) VALUES (17, 3, 'Cute', null, 0, 0, 1, 0)
SET IDENTITY_INSERT [dbo].[ResponseTypes] OFF

-- media path type categories
SET IDENTITY_INSERT [dbo].[MediaPathTypeCategories] ON 
INSERT [dbo].[MediaPathTypeCategories] ([Id], [Description]) VALUES (1, 'Audio')
INSERT [dbo].[MediaPathTypeCategories] ([Id], [Description]) VALUES (2, 'Image')
INSERT [dbo].[MediaPathTypeCategories] ([Id], [Description]) VALUES (3, 'Video')
SET IDENTITY_INSERT [dbo].[MediaPathTypeCategories] OFF

-- media path types
SET IDENTITY_INSERT [dbo].[MediaPathTypes] ON

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription], [AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (1, 'audio\music', 'Audio (Music)', 'Music', 
'mp3', 'audio/mp3', 15000000, 15, 1, 5, 0, 1) -- 15 mb per song / 15 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription], [AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable])
VALUES (2, 'audio\radio-shows', 'Audio (Radio Shows)', 'Radio Shows',
'mp3', 'audio/mp3', 50000000, 5, 1, 5, 0, 1) -- 50 mb per radio show / 5 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription], [AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (3, 'images\general', 'Images (General)', 'Images', 
'jpg, jpeg, jpe, jif, jfif, jfi, png, gif', 'image/pjpeg, image/jpeg, image/png, image/x-png, image/gif, image/x-gif', 
5000000, 50, 2, 1, 0, 1) -- 5 mb per image / 20 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (4, 'images\personal', 'Images (Personal)', 'Images', 
'jpg, jpeg, jpe, jif, jfif, jfi, png, gif', 'image/pjpeg, image/jpeg, image/png, image/x-png, image/gif, image/x-gif', 
5000000, 50, 2, 1, 0, 0) -- 5 mb per image / 20 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (5, 'videos\tv-shows', 'Videos (TV Shows)', 'TV Shows', 
'mp4', 'video/mp4', 1000000000, 3, 3, 6, 0, 1) -- 1 gb per tv show / 3 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (6, 'videos\home-movies', 'Videos (Home Movies)', 'Home Movies', 
'mp4', 'video/mp4', 1000000000, 3, 3, 6, 0, 0) -- 1 gb per home movie / 3 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (7, 'activities\matching-game\shapes', 'Matching Game (Shapes)', 'Shapes', 
'png', 'image/png', 5000000, 20, 2, 2, 0, 1)  -- 5 mb per shape / 20 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (8, 'activities\matching-game\sounds', 'Matching Game (Sounds)', 'Sounds', 
'mp3', 'audio/mp3', 1000000, 10, 1, 2, 0, 1)  -- 1 mb per sound / 10 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (9, 'videos\ambient', 'Videos (Ambient)', 'Ambient Videos', 
'mp4', 'video/mp4', 3000000000, 1, 3, 8, 1, 1)  -- 3 gb per video / 3 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (10, 'videos\cats', 'Videos (Cats)', 'Cats Videos', 
'mp4', 'video/mp4', 1000000000, 3, 3, 3, 1, 1)  -- 1 gb per video / 3 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (11, 'videos\nature', 'Videos (Nature)', 'Nature Videos', 
'mp4', 'video/mp4', 1000000000, 3, 3, 13, 1, 1)  -- 1 gb per video / 3 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (12, 'videos\sports', 'Videos (Sports)', 'Sports Videos', 
'mp4', 'video/mp4', 1000000000, 3, 3, 14, 1, 1)  -- 1 gb per video / 3 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (13, 'videos\machinery', 'Videos (Machinery)', 'Machinery Videos', 
'mp4', 'video/mp4', 1000000000, 3, 3, 15, 1, 1)  -- 1 gb per video / 3 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (14, 'videos\animals', 'Videos (Animals)', 'Animal Videos', 
'mp4', 'video/mp4', 1000000000, 3, 3, 16, 1, 1)  -- 1 gb per video / 3 uploads at a time

INSERT [dbo].[MediaPathTypes] ([Id], [Path], [Description], [ShortDescription],[AllowedExts], [AllowedTypes], [MaxFileBytes], [MaxFileUploads], [MediaPathTypeCategoryId], [ResponseTypeId], [IsSystem], [IsSharable]) 
VALUES (15, 'videos\cute', 'Videos (Cute)', 'Cute Videos', 
'mp4', 'video/mp4', 1000000000, 3, 3, 17, 1, 1)  -- 1 gb per video / 3 uploads at a time

SET IDENTITY_INSERT [dbo].[MediaPathTypes] OFF

-- ambient invitations
SET IDENTITY_INSERT [dbo].[AmbientInvitations] ON 
INSERT [dbo].[AmbientInvitations] ([Id], [Message], [ResponseTypeId]) VALUES (1, 'Touch The Screen', 2) -- matching game
INSERT [dbo].[AmbientInvitations] ([Id], [Message], [ResponseTypeId]) VALUES (2, 'Pet The Cat', null)
INSERT [dbo].[AmbientInvitations] ([Id], [Message], [ResponseTypeId]) VALUES (3, 'Turn The Wheel', null)
INSERT [dbo].[AmbientInvitations] ([Id], [Message], [ResponseTypeId]) VALUES (4, 'Touch The Screen', 1) -- slide show
INSERT [dbo].[AmbientInvitations] ([Id], [Message], [ResponseTypeId]) VALUES (5, 'Pet The Cat', null)
INSERT [dbo].[AmbientInvitations] ([Id], [Message], [ResponseTypeId]) VALUES (6, 'Turn The Wheel', null)
SET IDENTITY_INSERT [dbo].[AmbientInvitations] OFF

--- CONFIGURATION 1
SET IDENTITY_INSERT [dbo].[Configs] ON 
INSERT [dbo].[Configs] ([Id], [Description], [IsActive], [IsActiveEventLog]) VALUES (1, N'Default Configuration', 1, 1)
SET IDENTITY_INSERT [dbo].[Configs] OFF

-- sensors
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 1, 1, 10, 'Panel', 'Top Right')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 2, 1, 7, 'Panel', 'Top Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 3, 1, 3, 'Fur', 'Bottom Right')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 4, 2, 5, 'Radio Knob', 'Middle Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 5, 2, 6, 'TV Knob', 'Middle Right')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 8, 1, 4, 'Panel', 'Side Right')

-- inputs
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 9, 5, 1, 'Wheel', 'Bottom Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 10, 5, 2, 'Toggle', 'Bottom Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 12, 5, 5, 'Toggle', 'Bottom Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 13, 5, 11, 'Toggle', 'Bottom Left')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description], [Location]) VALUES (1, 16, 5, 9, 'Left-Right Switch', null)