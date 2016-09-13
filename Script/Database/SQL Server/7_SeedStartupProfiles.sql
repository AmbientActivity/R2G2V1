USE [KeebeeAAT]
GO
-- generic profile
SET IDENTITY_INSERT [dbo].[Profiles] ON 
INSERT [dbo].[Profiles] ([Id], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (1, N'Public Library', 1, GetDate(), GetDate())
SET IDENTITY_INSERT [dbo].[Profiles] OFF

-- residents
SET IDENTITY_INSERT [dbo].[Residents] ON 
INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [DateCreated], [DateUpdated], [ProfileId]) 
VALUES (1, N'Alma', N'Robinson', N'F', GetDate(), GetDate(), 1)

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [DateCreated], [DateUpdated], [ProfileId]) 
VALUES (2, N'Earl', N'Smith', N'M', GetDate(), GetDate(), 1)

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [DateCreated], [DateUpdated], [ProfileId]) 
VALUES (3, N'Ruth', N'Chambers', N'F', GetDate(), GetDate(), 1)
SET IDENTITY_INSERT [dbo].[Residents] OFF

-- custom profiles (update resident profileid)
SET IDENTITY_INSERT [dbo].[Profiles] ON 
INSERT [dbo].[Profiles] ([Id], [ResidentId], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (2, 1, N'Alma Robinson''s Profile', 1, GetDate(), GetDate())
UPDATE [dbo].[Residents] SET ProfileId = 2 WHERE Id = 1;
INSERT [dbo].[Profiles] ([Id], [ResidentId], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (3, 2, N'Earl Smith''s Profile', 2, GetDate(), GetDate())
UPDATE [dbo].[Residents] SET ProfileId = 3 WHERE Id = 2;
INSERT [dbo].[Profiles] ([Id], [ResidentId], [Description], [GameDifficultyLevel], [DateCreated], [DateUpdated]) VALUES (4, 3, N'Ruth Chambers'' Profile', 3, GetDate(), GetDate())
UPDATE [dbo].[Residents] SET ProfileId = 4 WHERE Id = 3;
SET IDENTITY_INSERT [dbo].[Profiles] OFF