USE [KeebeeAAT]
GO

-- profiles
SET IDENTITY_INSERT [dbo].[Profiles] ON 
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (1, N'Public Library', 1, GetDate(), GetDate())
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (2, N'Alma''s Profile', 2, GetDate(), GetDate())
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (3, N'Earl''s Profile', 3, GetDate(), GetDate())
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (4, N'Ruth''s Profile', 5, GetDate(), GetDate())
SET IDENTITY_INSERT [dbo].[Profiles] OFF

-- residents
SET IDENTITY_INSERT [dbo].[Residents] ON 
INSERT [dbo].[Residents] ([Id], [Tag], [FirstName], [LastName], [Gender], [DateCreated], [DateUpdated], [ProfileId]) 
VALUES (1, N'111111', N'Alma', N'Robinson', N'F', GetDate(), GetDate(), 2)

INSERT [dbo].[Residents] ([Id], [Tag], [FirstName], [LastName], [Gender], [DateCreated], [DateUpdated], [ProfileId]) 
VALUES (2, N'222222', N'Earl', N'Smith', N'M', GetDate(), GetDate(), 3)

INSERT [dbo].[Residents] ([Id], [Tag], [FirstName], [LastName], [Gender], [DateCreated], [DateUpdated], [ProfileId]) 
VALUES (3, N'333333', N'Ruth', N'Chambers', N'F', GetDate(), GetDate(), 4)
SET IDENTITY_INSERT [dbo].[Residents] OFF


SET IDENTITY_INSERT [dbo].[ActivityTypes] ON 
-- sensors
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (1, 'Sensor0', null)
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (2, 'Sensor1', null)
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (3, 'Sensor2', 'Fur')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (4, 'Sensor3', 'Side Button')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (5, 'Sensor4', 'Radio Knob')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (6, 'Sensor5', 'TV Knob')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (7, 'Sensor6', 'Side Button')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (8, 'Sensor7', 'Side Button')
-- inputs
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (9, 'Input0', 'Pull Switch')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (10, 'Input1', 'Wheel')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (11, 'Input2', null)
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (12, 'Input3', null)
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (13, 'Input4', 'Toggle Switch')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (14, 'Input5', 'Toggle Switch')
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (15, 'Input6', null)
INSERT [dbo].[ActivityTypes] ([Id], [PhidgetType], [Description]) VALUES (16, 'Input7', null)
SET IDENTITY_INSERT [dbo].[ActivityTypes] OFF

SET IDENTITY_INSERT [dbo].[ResponseTypeCategories] ON 
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (1, 'Image')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (2, 'Music')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (3, 'Video')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (4, 'Game')
INSERT [dbo].[ResponseTypeCategories] ([Id], [Description]) VALUES (5, 'System')
SET IDENTITY_INSERT [dbo].[ResponseTypeCategories] OFF

SET IDENTITY_INSERT [dbo].[ResponseTypes] ON 
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive]) VALUES (1, 1, 'Slide Show', 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive]) VALUES (2, 4, 'Memory Matching Game', 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive]) VALUES (3, 3, 'Cats', 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive]) VALUES (4, 5, 'Kill Display', 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive]) VALUES (5, 2, 'Radio', 0)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive]) VALUES (6, 3, 'Television', 0)
SET IDENTITY_INSERT [dbo].[ResponseTypes] OFF

SET IDENTITY_INSERT [dbo].[ProfileDetails] ON 
-- sensors
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (1, 1, 1, 1)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (2, 1, 2, 2)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (3, 1, 3, 3)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (4, 1, 5, 5)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (5, 1, 6, 6)

INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (6, 2, 1, 1)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (7, 2, 2, 2)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (8, 2, 3, 3)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (9, 2, 5, 5)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (10, 2, 6, 6)

INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (11, 3, 1, 1)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (12, 3, 2, 2)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (13, 3, 3, 3)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (14, 3, 5, 5)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (15, 3, 6, 6)

INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (16, 4, 1, 1)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (17, 4, 2, 2)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (18, 4, 3, 3)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (19, 4, 5, 5)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (20, 4, 6, 6)

-- inputs
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (21, 1, 9, 1)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (22, 1, 10, 2)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (23, 1, 13, 5)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (24, 1, 14, 6)

INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (25, 2, 9, 1)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (26, 2, 10, 2)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (27, 2, 13, 5)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (28, 2, 14, 6)

INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (29, 3, 9, 1)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (30, 3, 10, 2)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (31, 3, 13, 5)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (32, 3, 14, 6)

INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (33, 4, 9, 1)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (34, 4, 10, 2)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (35, 4, 13, 5)
INSERT [dbo].[ProfileDetails] ([Id], [ProfileId], [ActivityTypeId], [ResponseTypeId]) VALUES (36, 4, 14, 6)
SET IDENTITY_INSERT [dbo].[ProfileDetails] OFF

SET IDENTITY_INSERT [dbo].[GameTypes] ON 
INSERT [dbo].[GameTypes] ([Id], [Description]) VALUES (1, N'Match The Pictures')
INSERT [dbo].[GameTypes] ([Id], [Description]) VALUES (2, N'Match The Pairs')
SET IDENTITY_INSERT [dbo].[GameTypes] OFF