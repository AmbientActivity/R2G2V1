-- DE-ACTIVATE ALL OTHER CONFIGURATIONS
UPDATE [dbo].[Configs] SET IsActive = 0
GO

--- DUNNVILLE CONFIGURATION
SET IDENTITY_INSERT [dbo].[Configs] ON 
INSERT [dbo].[Configs] ([Id], [Description], [IsActive], [IsActiveEventLog]) VALUES (3, N'Development 1', 1, 0)
SET IDENTITY_INSERT [dbo].[Configs] OFF

-- sensors
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 1, 1, 10, 'Top Right Button')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 2, 1, 7, 'Top Left Button')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 3, 1, 3, 'Fur')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 4, 2, 5, 'Radio Knob')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 5, 2, 6, 'TV Knob')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 8, 1, 4, 'Side Button')

-- inputs
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 9, 5, 1, 'Wheel')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 10, 5, 2, 'Toggle Switch')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 12, 5, 5, 'Toggle Switch')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 13, 5, 6, 'Toggle Switch')
INSERT [dbo].[ConfigDetails] ([ConfigId], [PhidgetTypeId], [PhidgetStyleTypeId], [ResponseTypeId], [Description]) VALUES (3, 16, 5, 9, 'Left-Right Switch')