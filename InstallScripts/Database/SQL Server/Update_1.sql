-- response types
SET IDENTITY_INSERT [dbo].[ResponseTypes] ON 
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (9, 5, 'Off Screen', 0, 1)
INSERT [dbo].[ResponseTypes] ([Id], [ResponseTypeCategoryId], [Description], [IsInteractive], [IsSystem]) VALUES (10, 5, 'Volume Control', 0, 1)
SET IDENTITY_INSERT [dbo].[ResponseTypes] OFF

-- config details (default)
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 1, 5, 10, 'Side Button')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (1, 16, 5, 9, 'Toggle Switch')