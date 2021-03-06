USE [master]
GO
/****** Object:  Database [Eacc]    Script Date: 03/10/2016 18:00:57 ******/
CREATE DATABASE [Eacc]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Eacc', FILENAME = N'D:\RDSDBDATA\DATA\Eacc.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10%)
 LOG ON 
( NAME = N'Eacc_log', FILENAME = N'D:\RDSDBDATA\DATA\Eacc_log.ldf' , SIZE = 3456KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Eacc] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Eacc].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Eacc] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Eacc] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Eacc] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Eacc] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Eacc] SET ARITHABORT OFF 
GO
ALTER DATABASE [Eacc] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Eacc] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Eacc] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Eacc] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Eacc] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Eacc] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Eacc] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Eacc] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Eacc] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Eacc] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Eacc] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Eacc] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Eacc] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Eacc] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Eacc] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Eacc] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Eacc] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Eacc] SET RECOVERY FULL 
GO
ALTER DATABASE [Eacc] SET  MULTI_USER 
GO
ALTER DATABASE [Eacc] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Eacc] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Eacc] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Eacc] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Eacc] SET DELAYED_DURABILITY = DISABLED 
GO
USE [Eacc]
GO
/****** Object:  User [bbaumann]    Script Date: 03/10/2016 18:00:58 ******/
CREATE USER [bbaumann] FOR LOGIN [bbaumann] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [bbaumann]
GO
/****** Object:  UserDefinedTableType [dbo].[TVP_INT]    Script Date: 03/10/2016 18:00:58 ******/
CREATE TYPE [dbo].[TVP_INT] AS TABLE(
	[ID1] [int] NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[TVP_INT_INT_CHAR]    Script Date: 03/10/2016 18:00:58 ******/
CREATE TYPE [dbo].[TVP_INT_INT_CHAR] AS TABLE(
	[ID1] [int] NULL,
	[ID2] [int] NULL,
	[LABEL1] [nvarchar](4000) NULL
)
GO
/****** Object:  Table [dbo].[ACH_ACHIEVMENT]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ACH_ACHIEVMENT](
	[ACH_ID] [int] IDENTITY(1,1) NOT NULL,
	[ACH_NAME_CH] [nvarchar](500) NOT NULL,
	[ACH_DESCR_CH] [nvarchar](500) NULL,
	[ACH_IMG_CH] [nvarchar](500) NULL,
	[ACH_ECC_RWRD_NU] [int] NULL,
	[ACH_POS_RWRD_NU] [int] NULL,
 CONSTRAINT [PK_ACH_ACHIEVMENT] PRIMARY KEY CLUSTERED 
(
	[ACH_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MOD_MODULE]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MOD_MODULE](
	[MOD_ID] [int] IDENTITY(1,1) NOT NULL,
	[MOD_NAME_CH] [nvarchar](500) NOT NULL,
	[MOD_TYPE_NU] [int] NOT NULL,
	[VCL_ID] [int] NOT NULL,
 CONSTRAINT [PK_MOD_MODULE] PRIMARY KEY CLUSTERED 
(
	[MOD_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TEA_ACH]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TEA_ACH](
	[ACH_ID] [int] NOT NULL,
	[TEA_ID] [int] NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TEA_TEAM]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TEA_TEAM](
	[TEA_ID] [int] IDENTITY(1,1) NOT NULL,
	[TEA_NAME_CH] [nvarchar](500) NOT NULL,
	[TEA_COLOR_CH] [nvarchar](500) NOT NULL,
	[TEA_MONEY_NU] [int] NOT NULL,
	[TEA_SCORE_NU] [int] NOT NULL,
	[TEA_SECRET_CH] [nvarchar](500) NULL,
 CONSTRAINT [PK_TEA_TEAM] PRIMARY KEY CLUSTERED 
(
	[TEA_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[VCL_VEHICLE]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VCL_VEHICLE](
	[VCL_ID] [int] IDENTITY(1,1) NOT NULL,
	[TEA_ID] [int] NOT NULL,
	[VCL_NAME_CH] [nvarchar](500) NOT NULL,
	[VCL_SQUARE_NU] [int] NOT NULL,
	[VCL_DOMAIN_CH] [nvarchar](2048) NOT NULL,
	[DEL_BL] [bit] NOT NULL,
	[VCL_TURNS_TO_MISS_NU] [int] NOT NULL DEFAULT ((0)),
	[VCL_FIGHT_WON_NU] [int] NOT NULL DEFAULT ((0)),
 CONSTRAINT [PK_VCL_VEHICLE] PRIMARY KEY CLUSTERED 
(
	[VCL_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  StoredProcedure [dbo].[PI_ACH_TEA]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PI_ACH_TEA]
	@ACH_ID								INT,
	@TEA_ID								INT

AS

INSERT INTO [dbo].[TEA_ACH]
           ([ACH_ID]
           ,[TEA_ID])
     VALUES
	 (	
	 @ACH_ID,
	 @TEA_ID		
	 )

GO
/****** Object:  StoredProcedure [dbo].[PI_VEHICLE]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PI_VEHICLE]
	@VCL_ID								INT,
	@VCL_DOMAIN_CH						nvarchar(2000),
	@VCL_NAME_CH						nvarchar(2000),
	@VCL_SQUARE_NU						INT,
	@TEA_ID								INT,
	@DEL_BL								BIT,
	@TVP_MODULES						TVP_INT_INT_CHAR readonly,
	@VCL_TURNS_TO_MISS_NU				INT,
	@VCL_FIGHT_WON_NU					INT,
	@OUTPUT_VCL_ID						INT output
AS

if @VCL_ID is null
begin
	insert into VCL_VEHICLE(TEA_ID, VCL_NAME_CH, VCL_SQUARE_NU, VCL_DOMAIN_CH, DEL_BL, VCL_TURNS_TO_MISS_NU, VCL_FIGHT_WON_NU)
	values (@TEA_ID, @VCL_NAME_CH, @VCL_SQUARE_NU, @VCL_DOMAIN_CH, @DEL_BL, @VCL_TURNS_TO_MISS_NU,@VCL_FIGHT_WON_NU)
	
	set @OUTPUT_VCL_ID = SCOPE_IDENTITY()
end
else
begin
	update VCL_VEHICLE
	set
		TEA_ID 			= @TEA_ID,
		VCL_NAME_CH 	= @VCL_NAME_CH,
		VCL_SQUARE_NU 	= @VCL_SQUARE_NU,
		VCL_DOMAIN_CH 	= @VCL_DOMAIN_CH,
		DEL_BL 			= @DEL_BL,
		VCL_TURNS_TO_MISS_NU   = @VCL_TURNS_TO_MISS_NU,
		VCL_FIGHT_WON_NU   = @VCL_FIGHT_WON_NU
	where VCL_ID = @VCL_ID
	
	set @OUTPUT_VCL_ID = @VCL_ID
	
	-- delete associations
	delete MOD_MODULE
	where VCL_ID = @OUTPUT_VCL_ID
		
end

insert into MOD_MODULE(MOD_NAME_CH, MOD_TYPE_NU, VCL_ID)
select tvp.LABEL1, tvp.ID2,@OUTPUT_VCL_ID
from @TVP_MODULES tvp


GO
/****** Object:  StoredProcedure [dbo].[PS_ACHIEVEMENTS]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PS_ACHIEVEMENTS]
  @TEA_ID int
AS

select
	   t.TEA_ID,
	   a.ACH_ID,
	   a.ACH_NAME_CH,
	   a.ACH_DESCR_CH,
	   a.ACH_IMG_CH,
	   a.ACH_ECC_RWRD_NU,
	   a.ACH_POS_RWRD_NU
from
	TEA_ACH t
	inner join ACH_ACHIEVMENT a on a.ACH_ID = t.ACH_ID
where TEA_ID = @TEA_ID
	

GO
/****** Object:  StoredProcedure [dbo].[PS_ALL_ACH_TEA]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PS_ALL_ACH_TEA]
AS

select
	   TEA_ID,
	   ACH_ID
from
	TEA_ACH
	

GO
/****** Object:  StoredProcedure [dbo].[PS_ALL_ACHIEVMENTS]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PS_ALL_ACHIEVMENTS]
AS

select
ACH_ID
      ,ACH_NAME_CH
      ,ACH_DESCR_CH
      ,ACH_IMG_CH
      ,ACH_ECC_RWRD_NU
      ,ACH_POS_RWRD_NU
from
	ACH_ACHIEVMENT
	

GO
/****** Object:  StoredProcedure [dbo].[PS_ALL_TEAMS]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PS_ALL_TEAMS]
AS

select
	   TEA_ID
      ,TEA_NAME_CH
      ,TEA_COLOR_CH
      ,TEA_MONEY_NU
      ,TEA_SCORE_NU
      ,TEA_SECRET_CH
from
	TEA_TEAM
	

GO
/****** Object:  StoredProcedure [dbo].[PS_ALL_VEHICLE_ONLY]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PS_ALL_VEHICLE_ONLY]

AS

SELECT 
	   [VCL_ID]
      ,[TEA_ID]
      ,[VCL_NAME_CH]
      ,[VCL_SQUARE_NU]
      ,[VCL_DOMAIN_CH]
      ,[DEL_BL]
      ,[VCL_TURNS_TO_MISS_NU]
      ,[VCL_FIGHT_WON_NU]
  FROM [Eacc].[dbo].[VCL_VEHICLE]


GO
/****** Object:  StoredProcedure [dbo].[PS_ALL_VEHICLES]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PS_ALL_VEHICLES]
	@DEL_BL	bit = NULL
AS

select
	vcl.VCL_ID,
	vcl.TEA_ID,
	vcl.VCL_NAME_CH,
	vcl.VCL_SQUARE_NU,
	vcl.VCL_DOMAIN_CH,
	vcl.DEL_BL,
	m.MOD_ID,
	m.MOD_NAME_CH,
	m.MOD_TYPE_NU,
	vcl.VCL_TURNS_TO_MISS_NU,
	vcl.VCL_FIGHT_WON_NU,
	t.TEA_SECRET_CH
from
	VCL_VEHICLE vcl
	inner join MOD_MODULE m on m.VCL_ID = vcl.VCL_ID
	inner join TEA_TEAM t on t.TEA_ID = vcl.TEA_ID
where
	@DEL_BL IS NULL OR vcl.DEL_BL = @DEL_BL
order by vcl.VCL_ID
	



GO
/****** Object:  StoredProcedure [dbo].[PS_ALL_VEHICLES_ONLY]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PS_ALL_VEHICLES_ONLY]

AS

SELECT 
	   [VCL_ID]
      ,[TEA_ID]
      ,[VCL_NAME_CH]
      ,[VCL_SQUARE_NU]
      ,[VCL_DOMAIN_CH]
      ,[DEL_BL]
      ,[VCL_TURNS_TO_MISS_NU]
      ,[VCL_FIGHT_WON_NU]
  FROM [Eacc].[dbo].[VCL_VEHICLE]


GO
/****** Object:  StoredProcedure [dbo].[PS_HALLOFFAME]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PS_HALLOFFAME]
	@DEL_BL	bit = NULL
AS

select tea_id, SUM(VCL_FIGHT_WON_NU) AS SUM_FIGHT,MAX(VCL_FIGHT_WON_NU) AS MAX_FIGHT, SUM(VCL_SQUARE_NU) AS SUM_DISTANCE, MAX(VCL_SQUARE_NU) AS MAX_DISTANCE
from VCL_VEHICLE
group by TEA_ID



GO
/****** Object:  StoredProcedure [dbo].[PS_VEHICLE]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PS_VEHICLE]
	@VCL_ID		int
AS

select
	VCL_ID,
	v.TEA_ID,
	VCL_NAME_CH,
	VCL_SQUARE_NU,
	VCL_DOMAIN_CH,
	DEL_BL,
	VCL_TURNS_TO_MISS_NU,
	VCL_FIGHT_WON_NU,
	t.TEA_SECRET_CH
from
	VCL_VEHICLE v
	inner join TEA_TEAM t on t.TEA_ID = v.TEA_ID
where
	VCL_ID = @VCL_ID
	
select 	m.MOD_ID,
		m.MOD_NAME_CH,
		m.MOD_TYPE_NU,
		m.VCL_ID
from
	MOD_MODULE m
where 
	m.VCL_ID = @VCL_ID



GO
/****** Object:  StoredProcedure [dbo].[PU_FIGHT_WON]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PU_FIGHT_WON]
	@VCL_ID		int,
	@VCL_FIGHT_WON_NU int
AS

update VCL_VEHICLE
set VCL_FIGHT_WON_NU = @VCL_FIGHT_WON_NU
where
	VCL_ID = @VCL_ID


GO
/****** Object:  StoredProcedure [dbo].[PU_TEAM]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PU_TEAM]
@TEA_ID int,
@TEA_MONEY_NU int,
@TEA_SCORE_NU int
AS

update TEA_TEAM
set
      TEA_MONEY_NU = @TEA_MONEY_NU,
      TEA_SCORE_NU = @TEA_SCORE_NU
where TEA_ID = @TEA_ID
	

GO
/****** Object:  StoredProcedure [dbo].[PU_TURNS_TO_MISS]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[PU_TURNS_TO_MISS]
	@VCL_ID		int,
	@VCL_TURNS_TO_MISS_NU int
AS

update VCL_VEHICLE
set VCL_TURNS_TO_MISS_NU = @VCL_TURNS_TO_MISS_NU
where
	VCL_ID = @VCL_ID
	



GO
/****** Object:  DdlTrigger [rds_deny_backups_trigger]    Script Date: 03/10/2016 18:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [rds_deny_backups_trigger] ON DATABASE WITH EXECUTE AS 'dbo' FOR
 ADD_ROLE_MEMBER, GRANT_DATABASE AS BEGIN
   SET NOCOUNT ON;
   SET ANSI_PADDING ON;
 
   DECLARE @data xml;
   DECLARE @user sysname;
   DECLARE @role sysname;
   DECLARE @type sysname;
   DECLARE @sql NVARCHAR(MAX);
   DECLARE @permissions TABLE(name sysname PRIMARY KEY);
   
   SELECT @data = EVENTDATA();
   SELECT @type = @data.value('(/EVENT_INSTANCE/EventType)[1]', 'sysname');
    
   IF @type = 'ADD_ROLE_MEMBER' BEGIN
      SELECT @user = @data.value('(/EVENT_INSTANCE/ObjectName)[1]', 'sysname'),
       @role = @data.value('(/EVENT_INSTANCE/RoleName)[1]', 'sysname');

      IF @role IN ('db_owner', 'db_backupoperator') BEGIN
         SELECT @sql = 'DENY BACKUP DATABASE, BACKUP LOG TO ' + QUOTENAME(@user);
         EXEC(@sql);
      END
   END ELSE IF @type = 'GRANT_DATABASE' BEGIN
      INSERT INTO @permissions(name)
      SELECT Permission.value('(text())[1]', 'sysname') FROM
       @data.nodes('/EVENT_INSTANCE/Permissions/Permission')
      AS DatabasePermissions(Permission);
      
      IF EXISTS (SELECT * FROM @permissions WHERE name IN ('BACKUP DATABASE',
       'BACKUP LOG'))
         RAISERROR('Cannot grant backup database or backup log', 15, 1) WITH LOG;       
   END
END


GO
ENABLE TRIGGER [rds_deny_backups_trigger] ON DATABASE
GO
USE [master]
GO
ALTER DATABASE [Eacc] SET  READ_WRITE 
GO
