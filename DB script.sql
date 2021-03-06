USE [master]
GO
/****** Object:  Database [Banking]    Script Date: 19.03.2016 15:40:12 ******/
CREATE DATABASE [Banking]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Banking', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\Banking.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Banking_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\Banking_log.ldf' , SIZE = 2048KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Banking] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Banking].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Banking] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Banking] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Banking] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Banking] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Banking] SET ARITHABORT OFF 
GO
ALTER DATABASE [Banking] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Banking] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Banking] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Banking] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Banking] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Banking] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Banking] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Banking] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Banking] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Banking] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Banking] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Banking] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Banking] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Banking] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Banking] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Banking] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Banking] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Banking] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Banking] SET  MULTI_USER 
GO
ALTER DATABASE [Banking] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Banking] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Banking] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Banking] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Banking] SET DELAYED_DURABILITY = DISABLED 
GO
USE [Banking]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 19.03.2016 15:40:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Account](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[CurrencyId] [int] NOT NULL,
	[Number] [varchar](20) NOT NULL,
	[Amount] [money] NOT NULL,
 CONSTRAINT [PK_Acount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Currency]    Script Date: 19.03.2016 15:40:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Currency](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
	[Prefix] [varchar](3) NOT NULL,
 CONSTRAINT [PK_Currency] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Transaction]    Script Date: 19.03.2016 15:40:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Transaction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AccountFromId] [int] NOT NULL,
	[AccountToId] [int] NULL,
	[Amount] [money] NOT NULL,
	[Datetime] [datetime] NOT NULL,
	[Description] [nvarchar](80) NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 19.03.2016 15:40:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Login] [nvarchar](20) NOT NULL,
	[Password] [nvarchar](20) NOT NULL,
	[Pin] [varchar](4) NOT NULL,
	[Email] [varchar](50) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Account_Number]    Script Date: 19.03.2016 15:40:12 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Account_Number] ON [dbo].[Account]
(
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Currency_Name]    Script Date: 19.03.2016 15:40:12 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Currency_Name] ON [dbo].[Currency]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Currency_Prefix]    Script Date: 19.03.2016 15:40:12 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Currency_Prefix] ON [dbo].[Currency]
(
	[Prefix] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_User_Login]    Script Date: 19.03.2016 15:40:12 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_User_Login] ON [dbo].[User]
(
	[Login] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_Currency] FOREIGN KEY([CurrencyId])
REFERENCES [dbo].[Currency] ([Id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_Currency]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_User]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Account_From] FOREIGN KEY([AccountFromId])
REFERENCES [dbo].[Account] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_Account_From]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Account_To] FOREIGN KEY([AccountToId])
REFERENCES [dbo].[Account] ([Id])
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_Account_To]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [CK_Account_Amount] CHECK  (([Amount]>=(0)))
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [CK_Account_Amount]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [CK_Account_Number] CHECK  ((datalength([Number])=(20) AND NOT [Number] like '%[^0-9 ]%'))
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [CK_Account_Number]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [CK_Transaction_Amount] CHECK  (([Amount]>(0)))
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [CK_Transaction_Amount]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [CK_User_Pin] CHECK  ((datalength([Pin])=(4) AND NOT [Pin] like '%[^0-9 ]%'))
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [CK_User_Pin]
GO
/****** Object:  StoredProcedure [dbo].[TransferMoney]    Script Date: 19.03.2016 15:40:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TransferMoney] 
	@AccountFromId int,
	@AccountToNumber varchar(20),
	@Amount money,
	@AmountActual money,
	@Description nvarchar(80)
AS
BEGIN
	--BEGIN TRY
	--	BEGIN TRANSACTION 

		IF (NOT EXISTS (SELECT * FROM dbo.Account WHERE Id = @AccountFromId)) 
		THROW 51000, 'FROM Account does not exist', 1

		DECLARE @AccountToId INT
		SET @AccountToId = (SELECT Id FROM dbo.Account WHERE Number = @AccountToNumber)

		IF (@AccountToId IS NULL) 
		THROW 51000, 'TO Account does not exist', 1

		IF (@Amount > (SELECT Amount FROM dbo.Account WHERE Id = @AccountFromId)) 
		THROW 51000, 'Not enough money to transfer', 1


		INSERT INTO [dbo].[Transaction]
				   ([AccountFromId]
				   ,[AccountToId]
				   ,[Amount]
				   ,[Datetime]
				   ,[Description])
			 VALUES
				   (@AccountFromId,
					@AccountToId,
				    @Amount,
				    GETDATE(),
				    @Description)


		UPDATE [dbo].[Account]
		SET [Amount] = [Amount] - @Amount
		WHERE Id = @AccountFromId

		UPDATE [dbo].[Account]
		SET [Amount] = [Amount] + @AmountActual
		WHERE Id = @AccountToId
		
	--	COMMIT
	--END TRY
	--BEGIN CATCH
	--	ROLLBACK
	--END CATCH
END

GO
USE [master]
GO
ALTER DATABASE [Banking] SET  READ_WRITE 
GO

-- ***** DATA *****
USE [Banking]
GO
SET IDENTITY_INSERT [dbo].[Currency] ON 

GO
INSERT [dbo].[Currency] ([Id], [Name], [Prefix]) VALUES (1, N'US Dollar', N'USD')
GO
INSERT [dbo].[Currency] ([Id], [Name], [Prefix]) VALUES (2, N'Euro', N'EUR')
GO
INSERT [dbo].[Currency] ([Id], [Name], [Prefix]) VALUES (3, N'British Pound', N'GBP')
GO
SET IDENTITY_INSERT [dbo].[Currency] OFF
GO
