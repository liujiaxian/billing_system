USE [billing_system_db]
GO
/****** Object:  Table [dbo].[billing_log]    Script Date: 2021-03-28 21:12:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[billing_log](
	[billing_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[trade_type] [bit] NOT NULL,
	[participant_user_id] [nvarchar](50) NOT NULL,
	[money] [money] NOT NULL,
	[remark] [nvarchar](250) NOT NULL,
	[is_delete] [bit] NOT NULL,
	[trade_user] [nvarchar](50) NULL,
	[participant_user_name] [nvarchar](250) NULL,
	[trade_time] [datetime] NOT NULL,
	[team_id] [bigint] NOT NULL,
	[create_time] [datetime] NOT NULL,
	[update_time] [datetime] NOT NULL,
 CONSTRAINT [PK_billing_log] PRIMARY KEY CLUSTERED 
(
	[billing_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_notice]    Script Date: 2021-03-28 21:12:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_notice](
	[notice_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_id] [bigint] NOT NULL,
	[msg] [nvarchar](500) NOT NULL,
	[type_id] [int] NOT NULL,
	[is_delete] [bit] NOT NULL,
	[billing_id] [bigint] NULL,
	[create_time] [datetime] NULL,
	[update_time] [datetime] NULL,
 CONSTRAINT [PK_sys_notice] PRIMARY KEY CLUSTERED 
(
	[notice_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[sys_user]    Script Date: 2021-03-28 21:12:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sys_user](
	[user_id] [bigint] IDENTITY(1,1) NOT NULL,
	[user_name] [nvarchar](20) NULL,
	[user_nickname] [nvarchar](20) NULL,
	[user_email] [nvarchar](32) NULL,
	[user_password] [nvarchar](32) NULL,
	[user_role] [int] NULL,
	[user_face] [nvarchar](250) NULL,
	[apply_status] [int] NULL,
	[email_token] [nvarchar](50) NULL,
	[email_token_time] [datetime] NULL,
	[create_time] [datetime] NULL,
	[update_time] [datetime] NULL,
	[login_count] [int] NOT NULL,
	[login_time] [datetime] NULL,
 CONSTRAINT [PK_sys_user] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[team]    Script Date: 2021-03-28 21:12:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[team](
	[teamID] [bigint] IDENTITY(1,1) NOT NULL,
	[teamName] [varchar](50) NOT NULL,
	[userID] [bigint] NOT NULL,
	[addTime] [datetime] NULL,
	[updateTime] [datetime] NULL,
 CONSTRAINT [PK_sys_team] PRIMARY KEY CLUSTERED 
(
	[teamID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[team_user]    Script Date: 2021-03-28 21:12:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[team_user](
	[teamUserID] [bigint] IDENTITY(1,1) NOT NULL,
	[teamID] [bigint] NOT NULL,
	[userID] [bigint] NOT NULL,
	[parentID] [bigint] NULL,
	[status] [int] NOT NULL,
	[addTime] [datetime] NULL,
	[updateTime] [datetime] NULL,
 CONSTRAINT [PK_team_user] PRIMARY KEY CLUSTERED 
(
	[teamUserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[vyw_billing_log_team]    Script Date: 2021-03-28 21:12:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vyw_billing_log_team]
AS
SELECT   dbo.billing_log.billing_id, dbo.billing_log.user_id, dbo.billing_log.trade_type, dbo.billing_log.participant_user_id, 
                dbo.billing_log.money, dbo.billing_log.remark, dbo.billing_log.is_delete, dbo.billing_log.trade_user, 
                dbo.billing_log.participant_user_name, dbo.billing_log.trade_time, dbo.billing_log.team_id, dbo.billing_log.create_time, 
                dbo.billing_log.update_time, dbo.team.teamName, dbo.team_user.status
FROM      dbo.billing_log INNER JOIN
                dbo.team ON dbo.billing_log.team_id = dbo.team.teamID INNER JOIN
                dbo.team_user ON dbo.billing_log.user_id = dbo.team_user.userID AND dbo.team.teamID = dbo.team_user.teamID


GO
/****** Object:  View [dbo].[vyw_notice_user]    Script Date: 2021-03-28 21:12:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vyw_notice_user]
AS
SELECT   dbo.sys_notice.notice_id, dbo.sys_notice.user_id, dbo.sys_notice.msg, dbo.sys_notice.type_id, 
                dbo.sys_notice.is_delete, dbo.sys_notice.create_time, dbo.sys_notice.update_time, dbo.sys_user.user_name, 
                dbo.sys_user.user_nickname, dbo.sys_user.user_email, dbo.sys_user.user_face, dbo.sys_user.user_role, 
                dbo.sys_notice.billing_id
FROM      dbo.sys_notice INNER JOIN
                dbo.sys_user ON dbo.sys_notice.user_id = dbo.sys_user.user_id


GO
/****** Object:  View [dbo].[vyw_team_user]    Script Date: 2021-03-28 21:12:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[vyw_team_user]
AS
SELECT   dbo.team.teamName, dbo.team.addTime, dbo.team.updateTime, dbo.team_user.teamUserID, dbo.team_user.parentID, 
                dbo.team_user.status, dbo.sys_user.user_name, dbo.sys_user.user_nickname, dbo.sys_user.user_email, 
                dbo.sys_user.user_role, dbo.sys_user.user_face, dbo.sys_user.create_time, dbo.sys_user.login_count, 
                dbo.sys_user.login_time, dbo.team_user.userID, dbo.team_user.teamID, sys_user_1.user_name AS parentName, 
                sys_user_1.user_nickname AS parentNickName, dbo.sys_user.apply_status
FROM      dbo.team INNER JOIN
                dbo.team_user ON dbo.team.teamID = dbo.team_user.teamID INNER JOIN
                dbo.sys_user ON dbo.team_user.userID = dbo.sys_user.user_id LEFT OUTER JOIN
                dbo.sys_user AS sys_user_1 ON dbo.team_user.parentID = sys_user_1.user_id


GO
SET IDENTITY_INSERT [dbo].[billing_log] ON 

INSERT [dbo].[billing_log] ([billing_id], [user_id], [trade_type], [participant_user_id], [money], [remark], [is_delete], [trade_user], [participant_user_name], [trade_time], [team_id], [create_time], [update_time]) VALUES (1, 6, 1, N'6,8,9', -80.0000, N'买菜80', 0, N'测试号', N'测试号,test01,test02', CAST(0x0000ACF301593DA4 AS DateTime), 1, CAST(0x0000ACF901594715 AS DateTime), CAST(0x0000ACF901594715 AS DateTime))
INSERT [dbo].[billing_log] ([billing_id], [user_id], [trade_type], [participant_user_id], [money], [remark], [is_delete], [trade_user], [participant_user_name], [trade_time], [team_id], [create_time], [update_time]) VALUES (2, 6, 1, N'6,8,9', -65.0000, N'买菜65', 0, N'测试号', N'测试号,test01,test02', CAST(0x0000ACF4015959C4 AS DateTime), 1, CAST(0x0000ACF9015965B4 AS DateTime), CAST(0x0000ACF9015965B4 AS DateTime))
INSERT [dbo].[billing_log] ([billing_id], [user_id], [trade_type], [participant_user_id], [money], [remark], [is_delete], [trade_user], [participant_user_name], [trade_time], [team_id], [create_time], [update_time]) VALUES (3, 6, 1, N'6,8,9', -55.0000, N'买菜 55', 0, N'测试号', N'测试号,test01,test02', CAST(0x0000ACF501597710 AS DateTime), 1, CAST(0x0000ACF901597B6D AS DateTime), CAST(0x0000ACF901597B6D AS DateTime))
INSERT [dbo].[billing_log] ([billing_id], [user_id], [trade_type], [participant_user_id], [money], [remark], [is_delete], [trade_user], [participant_user_name], [trade_time], [team_id], [create_time], [update_time]) VALUES (4, 6, 1, N'6,8,9', -105.0000, N'买菜 105', 0, N'测试号', N'测试号,test01,test02', CAST(0x0000ACF601598E80 AS DateTime), 1, CAST(0x0000ACF9015992D3 AS DateTime), CAST(0x0000ACF9015992D3 AS DateTime))
INSERT [dbo].[billing_log] ([billing_id], [user_id], [trade_type], [participant_user_id], [money], [remark], [is_delete], [trade_user], [participant_user_name], [trade_time], [team_id], [create_time], [update_time]) VALUES (5, 6, 1, N'6,8,9', -98.0000, N'买菜 98', 0, N'测试号', N'测试号,test01,test02', CAST(0x0000ACF70159A4C4 AS DateTime), 1, CAST(0x0000ACF90159A964 AS DateTime), CAST(0x0000ACF90159A964 AS DateTime))
INSERT [dbo].[billing_log] ([billing_id], [user_id], [trade_type], [participant_user_id], [money], [remark], [is_delete], [trade_user], [participant_user_name], [trade_time], [team_id], [create_time], [update_time]) VALUES (6, 6, 1, N'6,8,9', -43.0000, N'买菜 43', 0, N'测试号', N'测试号,test01,test02', CAST(0x0000ACFA015A593C AS DateTime), 1, CAST(0x0000ACF9015A60B9 AS DateTime), CAST(0x0000ACF9015AAC26 AS DateTime))
INSERT [dbo].[billing_log] ([billing_id], [user_id], [trade_type], [participant_user_id], [money], [remark], [is_delete], [trade_user], [participant_user_name], [trade_time], [team_id], [create_time], [update_time]) VALUES (7, 6, 1, N'6,8,9', -68.0000, N'买菜 68', 0, N'测试号', N'测试号,test01,test02', CAST(0x0000ACEC015ACAE8 AS DateTime), 1, CAST(0x0000ACF9015AD095 AS DateTime), CAST(0x0000ACF9015AD095 AS DateTime))
INSERT [dbo].[billing_log] ([billing_id], [user_id], [trade_type], [participant_user_id], [money], [remark], [is_delete], [trade_user], [participant_user_name], [trade_time], [team_id], [create_time], [update_time]) VALUES (8, 6, 1, N'6,8,9', -97.0000, N'买菜 97', 0, N'测试号', N'测试号,test01,test02', CAST(0x0000ACED015ADED4 AS DateTime), 1, CAST(0x0000ACF9015AE371 AS DateTime), CAST(0x0000ACF9015AE371 AS DateTime))
INSERT [dbo].[billing_log] ([billing_id], [user_id], [trade_type], [participant_user_id], [money], [remark], [is_delete], [trade_user], [participant_user_name], [trade_time], [team_id], [create_time], [update_time]) VALUES (9, 6, 1, N'6,8,9', -23.0000, N'买菜 23', 0, N'测试号', N'测试号,test01,test02', CAST(0x0000ACF9015AFFA4 AS DateTime), 1, CAST(0x0000ACF9015B0381 AS DateTime), CAST(0x0000ACF9015B0381 AS DateTime))
SET IDENTITY_INSERT [dbo].[billing_log] OFF
SET IDENTITY_INSERT [dbo].[sys_notice] ON 

INSERT [dbo].[sys_notice] ([notice_id], [user_id], [msg], [type_id], [is_delete], [billing_id], [create_time], [update_time]) VALUES (3, 3, N'欢迎使用记账系统，目前测试阶段，欢迎大家测试，谢谢。', 1, 0, NULL, CAST(0x0000A78F00ED9450 AS DateTime), CAST(0x0000A78F00ED9450 AS DateTime))
SET IDENTITY_INSERT [dbo].[sys_notice] OFF
SET IDENTITY_INSERT [dbo].[sys_user] ON 

INSERT [dbo].[sys_user] ([user_id], [user_name], [user_nickname], [user_email], [user_password], [user_role], [user_face], [apply_status], [email_token], [email_token_time], [create_time], [update_time], [login_count], [login_time]) VALUES (6, N'test', N'测试号', N'', N'e10adc3949ba59abbe56e057f20f883e', 1, N'/Content/img/samples/scarlet-159.png', 2, NULL, NULL, CAST(0x0000A78600E631AC AS DateTime), CAST(0x0000A78600E631AC AS DateTime), 47, CAST(0x0000ACF90158B216 AS DateTime))
INSERT [dbo].[sys_user] ([user_id], [user_name], [user_nickname], [user_email], [user_password], [user_role], [user_face], [apply_status], [email_token], [email_token_time], [create_time], [update_time], [login_count], [login_time]) VALUES (8, N'test01', N'test01', N'test01@qq.com', N'e10adc3949ba59abbe56e057f20f883e', 2, N'/Content/img/default_headpic.png', 2, NULL, NULL, CAST(0x0000ACF90158CB5E AS DateTime), CAST(0x0000ACF90158CB5E AS DateTime), 0, NULL)
INSERT [dbo].[sys_user] ([user_id], [user_name], [user_nickname], [user_email], [user_password], [user_role], [user_face], [apply_status], [email_token], [email_token_time], [create_time], [update_time], [login_count], [login_time]) VALUES (9, N'test02', N'test02', N'test02@qq.com', N'e10adc3949ba59abbe56e057f20f883e', 2, N'/Content/img/default_headpic.png', 2, NULL, NULL, CAST(0x0000ACF90158EDB9 AS DateTime), CAST(0x0000ACF90158EDB9 AS DateTime), 0, NULL)
SET IDENTITY_INSERT [dbo].[sys_user] OFF
SET IDENTITY_INSERT [dbo].[team] ON 

INSERT [dbo].[team] ([teamID], [teamName], [userID], [addTime], [updateTime]) VALUES (1, N'测试团队', 6, CAST(0x0000ACF90157D86B AS DateTime), CAST(0x0000ACF90157D86B AS DateTime))
SET IDENTITY_INSERT [dbo].[team] OFF
SET IDENTITY_INSERT [dbo].[team_user] ON 

INSERT [dbo].[team_user] ([teamUserID], [teamID], [userID], [parentID], [status], [addTime], [updateTime]) VALUES (1, 1, 6, 0, 1, CAST(0x0000ACF90157D86E AS DateTime), CAST(0x0000ACF90157D86E AS DateTime))
INSERT [dbo].[team_user] ([teamUserID], [teamID], [userID], [parentID], [status], [addTime], [updateTime]) VALUES (2, 1, 8, 6, 2, CAST(0x0000ACF90159033B AS DateTime), CAST(0x0000ACF90159033B AS DateTime))
INSERT [dbo].[team_user] ([teamUserID], [teamID], [userID], [parentID], [status], [addTime], [updateTime]) VALUES (3, 1, 9, 6, 2, CAST(0x0000ACF901590D90 AS DateTime), CAST(0x0000ACF901590D90 AS DateTime))
SET IDENTITY_INSERT [dbo].[team_user] OFF
ALTER TABLE [dbo].[sys_user] ADD  CONSTRAINT [DF_sys_user_login_count]  DEFAULT ((0)) FOR [login_count]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "billing_log"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 277
               Right = 256
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "team"
            Begin Extent = 
               Top = 130
               Left = 321
               Bottom = 295
               Right = 479
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "team_user"
            Begin Extent = 
               Top = 6
               Left = 517
               Bottom = 277
               Right = 675
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 16
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vyw_billing_log_team'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vyw_billing_log_team'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "sys_notice"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 321
               Right = 198
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "sys_user"
            Begin Extent = 
               Top = 6
               Left = 236
               Bottom = 307
               Right = 412
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vyw_notice_user'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vyw_notice_user'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[49] 4[12] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "team"
            Begin Extent = 
               Top = 6
               Left = 264
               Bottom = 245
               Right = 422
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "team_user"
            Begin Extent = 
               Top = 6
               Left = 460
               Bottom = 326
               Right = 618
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "sys_user"
            Begin Extent = 
               Top = 5
               Left = 696
               Bottom = 352
               Right = 884
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "sys_user_1"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 226
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vyw_team_user'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vyw_team_user'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'vyw_team_user'
GO
