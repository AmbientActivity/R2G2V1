IF NOT EXISTS (SELECT * FROM [dbo].[PhidgetStyleTypes] WHERE Id = 7)
BEGIN
	SET IDENTITY_INSERT [dbo].[PhidgetStyleTypes] ON
	INSERT [dbo].[PhidgetStyleTypes] ([Id], [Description], [IsIncremental]) VALUES (7, 'Non-rotational', 0)
	SET IDENTITY_INSERT [dbo].[PhidgetStyleTypes] OFF
END