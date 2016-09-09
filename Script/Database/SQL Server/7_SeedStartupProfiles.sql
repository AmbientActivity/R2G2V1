USE [KeebeeAAT]
GO

-- profiles
SET IDENTITY_INSERT [dbo].[Profiles] ON 
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (1, N'Public Library', 1, GetDate(), GetDate())
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (2, N'Alma Robinson''s Profile', 1, GetDate(), GetDate())
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (3, N'Earl Smith''s Profile', 2, GetDate(), GetDate())
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (4, N'Ruth Chambers'' Profile', 3, GetDate(), GetDate())
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