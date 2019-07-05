-- DE-ACTIVATE ALL OTHER CONFIGURATIONS
UPDATE [dbo].[Configs] SET IsActive = 0
GO

--- JOHN'S CONFIGURATION
SET IDENTITY_INSERT [dbo].[Configs] ON 
INSERT [dbo].[Configs] ([Id], [Description], [IsActive], [IsActiveEventLog]) VALUES (3, N'John''s Configuration', 1, 0)
SET IDENTITY_INSERT [dbo].[Configs] OFF

-- sensors
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 1, 1, 1, 'Touch')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 2, 3, 5, 'Dial')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 3, 1, 11, 'Touch')
--INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 4, 1, 4, 'Not In Use')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 5, 4, 6, 'Slider')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 6, 1, 2, 'Touch')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 7, 1, 7, 'Touch')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 8, 1, 8, 'Touch')

-- inputs
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 9, 5, 4, 'Magnet')