-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/19/2020 3:50PM
-- Update date:	6/24/2020 4:26PM
-- Description:	Create new Household, and assign it to a user.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Household_Create]
	@Name nvarchar(25),
	@Greeting nvarchar(255),
	@CreatorId nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CreatorId) THROW 51000, 'The CreatorId provided does not exist as a UserId.', 1;  
	DECLARE @now datetime = GETDATE();

	INSERT INTO [Households]
	VALUES(@Name, @Greeting, @now, @CreatorId)

	UPDATE [AspNetUsers]
	SET [HouseholdId] = (SELECT [Id] FROM [Households] WHERE [Name] = @Name AND
															 [Greeting] = @Greeting AND
															 [CreatedAt] = @now AND
															 [CreatorId] = @CreatorId);

END
GO


-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/19/2020 4:20PM
-- Update date:	6/24/2020 4:26PM
-- Description:	Delete an existing household.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Household_Delete]
	@Id int
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [Households] WHERE [Id] = @Id) THROW 51000, 'The Household Id provided returned no records', 1;

	DELETE FROM [Households] WHERE [Id] = @Id;
	
	return 0;
END
GO


-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/19/2020 4:06PM
-- Update date:	6/24/2020 4:26PM
-- Description:	Modify details of an existing household.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Household_Edit]
	@Id int,
	@newName nvarchar(25),
	@newGreeting nvarchar(255)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [Households] WHERE [Id] = @Id) THROW 51000, 'The Household Id provided returned no records', 1;

	DECLARE @oldName nvarchar(25);
	DECLARE @oldGreeting nvarchar(25);
	SELECT @oldName = [Name], @oldGreeting = [Greeting] FROM [Households] WHERE [Id] = @Id;
  
	IF DATALENGTH(@newName) !=0 AND @newName != @oldName 
	UPDATE [Households] SET [Name] = @newName WHERE [Id] = @Id;

	IF DATALENGTH(@newGreeting) !=0 AND @newGreeting != @oldGreeting 
	UPDATE [Households] SET [Greeting] = @newGreeting WHERE [Id] = @Id;
	
	return 0;
END
GO
