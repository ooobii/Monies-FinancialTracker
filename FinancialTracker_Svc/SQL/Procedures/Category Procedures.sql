-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 11:24AM
-- Description:	Create a new category for the household the user is assigned to.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Category_Create]
	@CreatorId nvarchar(max),
	@Name nvarchar(25),
	@Description nvarchar(500)
AS
BEGIN
	SET NOCOUNT ON;
		
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CreatorId) THROW 51000, 'The CreatorId provided does not exist as a UserId.', 1;  
	IF (CASE WHEN (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId) IS NULL THEN 1 ELSE 0 END) = 1 THROW 51000, 'The user creating this account does not belong to a household.', 1;  

	DECLARE @now datetime = GETDATE();
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId);
	
	INSERT INTO [Categories]
	VALUES(@houseId, @Name, @Description, @now)


	return 0;
END
GO


-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/25/2020 4:20PM
-- Update date: 6/25/2020 11:25AM
-- Description:	Delete an existing category.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Category_Delete]
	@Id int,
	@CallerId nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CallerId) THROW 51000, 'The CallerID does not exist as a user.', 1; 
	IF NOT EXISTS (SELECT [Id] FROM [Categories] WHERE [Id] = @Id) THROW 51000, 'The category Id provided returned no records', 1;	 
	
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);
	IF (CASE WHEN (SELECT [ParentHouseholdId] FROM [Categories] WHERE [Id] = @Id) != @houseId THEN 1 ELSE 0 END) = 1 THROW 51000, 'The user removing this category does not belong to the parent household.', 1; 
	
	DELETE FROM [Categories] WHERE [Id] = @Id;
	
	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/25/2020 10:50AM
-- Update date:	6/25/2020 11:28AM
-- Description:	Edit an existing account that belongs to a household the user is a member of.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Category_Edit]
	@Id int,
	@CallerId nvarchar(max),
	@NewName nvarchar(25),
	@NewDescription nvarchar(500)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CallerId) THROW 51000, 'The CallerID does not exist as a user.', 1; 
	IF NOT EXISTS (SELECT [Id] FROM [Categories] WHERE [Id] = @Id) THROW 51000, 'The ID provided did not locate a category.', 1;  
	
	DECLARE @now datetime = GETDATE();
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);

	IF (CASE WHEN (SELECT [ParentHouseholdId] FROM [Categories] WHERE [Id] = @Id) != @houseId THEN 1 ELSE 0 END) = 1 THROW 51000, 'The user editing this category does not belong to the parent household.', 1; 
	
	DECLARE @oldName nvarchar(25);
	DECLARE @oldDescription nvarchar(500);
	SELECT @oldName = [Name]
		  ,@oldDescription = [Description]
	FROM [Categories]
	WHERE [Id] = @Id	

  
	IF DATALENGTH(@NewName) !=0 AND @NewName != @oldName 
	UPDATE [Categories] SET [Name] = @NewName WHERE [Id] = @Id;

	IF DATALENGTH(@NewDescription) !=0  AND @NewDescription != @oldDescription 
	UPDATE [Categories] SET [Description] = @NewDescription WHERE [Id] = @Id;


	return 0;
END
GO
