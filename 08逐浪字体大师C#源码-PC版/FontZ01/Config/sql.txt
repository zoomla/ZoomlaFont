CREATE TABLE [dbo].[ZL_FZ_User](
	[Name] [nvarchar](200) NOT NULL,
	[UserPwd] [nvarchar](500) NULL,
	[CDate] [datetime] NULL,
	[Enddate] [datetime] NULL,
	[SiteInfo] [nvarchar](4000) NULL,
 CONSTRAINT [PK_ZL_FZ_User] PRIMARY KEY CLUSTERED 
([Name] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY]


CREATE TABLE [dbo].[ZL_FZ_Log](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ExMessage] [ntext] NULL,
	[Remind] [nvarchar](4000) NULL,
	[CDate] [datetime] NULL,
 CONSTRAINT [PK_ZL_FZ_Log] PRIMARY KEY CLUSTERED 
([ID] ASC)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


CREATE TABLE [dbo].[ZL_FZ_Server](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[address] [nvarchar](100) NULL,
	[uname] [nvarchar](50) NULL,
	[upwd] [nvarchar](50) NULL,
	[remind] [nvarchar](100) NULL,
	[cdate] [datetime] NULL,
	[rdp] [nvarchar](4000) NULL,
	[alias] [nvarchar](50) NULL,
 CONSTRAINT [PK_ZL_FZ_Server] PRIMARY KEY CLUSTERED 
([id] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]