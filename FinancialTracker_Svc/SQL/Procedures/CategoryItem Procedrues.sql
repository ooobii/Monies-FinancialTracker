-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 12:24PM
-- Description:	Create a new category item for the selected parent category.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[CategoryItem_Create]
	@CreatorId nvarchar(max),
	@Name nvarchar(25),
	@Description nvarchar(500),
	@Budget decimal(18, 2),
	@ParentCategoryId int
AS
BEGIN
	SET NOCOUNT ON;
		
	IF @Budget < 0 
		THROW 51000, 'The budget for a subcategory must be greater than or equal to 0.', 1;  

	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CreatorId) 
		THROW 51000, 'The CreatorId provided does not exist as a UserId.', 1;  

	IF (CASE WHEN (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId) IS NULL THEN 1 ELSE 0 END) = 1 
		THROW 51000, 'The user creating this category item does not belong to a household.', 1; 
		
	IF NOT EXISTS (SELECT [Id] FROM [Categories] WHERE [Id] = @ParentCategoryId) 
		THROW 51000, 'The parent category ID provided does not exist.', 1; 


	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId);
	DECLARE @categoryHouseId int = (SELECT [ParentHouseholdId] FROM [Categories] WHERE [Id] = @ParentCategoryId);
	
	IF @houseId != @categoryHouseId 
		THROW 51000, 'The parent category does not belong to the household of the creator.', 1; 



	INSERT INTO [CategoryItems]
	VALUES(@ParentCategoryId, @Name, @Description, @Budget)



	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/25/2020 4:20PM
-- Update date: 6/25/2020 12:24PM
-- Description:	Delete an existing subcategory.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[CategoryItem_Delete]
	@Id int,
	@CallerId nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CallerId) 
		THROW 51000, 'The CallerID does not exist as a user.', 1; 

	IF NOT EXISTS (SELECT [Id] FROM [CategoryItems] WHERE [Id] = @Id) 
		THROW 51000, 'The category item Id provided returned no records', 1;	 	


	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);
	DECLARE @categoryHouseId int = (SELECT c.[ParentHouseholdId] FROM [CategoryItems] ci LEFT JOIN [Categories] c ON ci.[ParentCategoryId] = c.[Id] WHERE ci.[Id] = @Id);
	IF @houseId != @categoryHouseId 
		THROW 51000, 'The parent category does not belong to the household of the creator.', 1; 


	DELETE FROM [CategoryItems] WHERE [Id] = @Id;
	
	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/25/2020 10:50AM
-- Update date:	6/25/2020 12:24PM
-- Description:	Edit an existing category item that belongs to a household the user is a member of.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[CategoryItem_Edit]
	@Id int,
	@CallerId nvarchar(max),
	@NewName nvarchar(25),
	@NewDescription nvarchar(500),
	@NewBudget decimal(18, 2),
	@NewParentCategoryId int
AS
BEGIN
	SET NOCOUNT ON;

	IF @NewBudget < 0 
		THROW 51000, 'The budget for a subcategory must be greater than or equal to 0.', 1; 

	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CallerId) 
		THROW 51000, 'The CallerID does not exist as a user.', 1; 

	IF NOT EXISTS (SELECT [Id] FROM [CategoryItems] WHERE [Id] = @Id) 
		THROW 51000, 'The ID provided did not locate a sub category.', 1; 
		
	IF @NewParentCategoryId IS NOT NULL AND NOT EXISTS (SELECT [Id] FROM [Categories] WHERE [Id] = @NewParentCategoryId) 
		THROW 51000, 'The new parent category ID provided does not exist.', 1; 
	

	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);
	DECLARE @oldCategoryHouseId int = (SELECT c.[ParentHouseholdId] FROM [CategoryItems] ci 
									   LEFT JOIN [Categories] c 
									   ON ci.[ParentCategoryId] = c.[Id] 
									   WHERE ci.[Id] = @Id);
	DECLARE @newCategoryHouseId int = (SELECT [ParentHouseholdId] FROM [Categories] 
								       WHERE [Id] = COALESCE(@NewParentCategoryId, @oldCategoryHouseId));


	IF @houseId != @oldCategoryHouseId
		THROW 51000, 'The parent category does not belong to the household of the editor.', 1; 
	IF @houseId != @newCategoryHouseId
		THROW 51000, 'The new parent category does not belong to the household of the editor.', 1; 


	DECLARE @oldName nvarchar(25);
	DECLARE @oldDescription nvarchar(500);
	DECLARE @oldBudget decimal(18, 2);
	DECLARE @oldParentCategoryId decimal(18, 2);
	SELECT @oldName = [Name]
		  ,@oldDescription = [Description]
		  ,@oldBudget = [AmountBudgeted]
		  ,@oldParentCategoryId = [ParentCategoryId]
	FROM [CategoryItems]
	WHERE [Id] = @Id	

  
	IF DATALENGTH(@NewName) !=0 AND @NewName != @oldName 
	UPDATE [CategoryItems] SET [Name] = @NewName WHERE [Id] = @Id;

	IF DATALENGTH(@NewDescription) !=0  AND @NewDescription != @oldDescription 
	UPDATE [CategoryItems] SET [Description] = @NewDescription WHERE [Id] = @Id;

	IF @NewBudget IS NOT NULL AND @NewBudget != @oldBudget 
	UPDATE [CategoryItems] SET [AmountBudgeted] = @NewBudget WHERE [Id] = @Id;

	IF @NewParentCategoryId IS NOT NULL AND @NewParentCategoryId != @oldParentCategoryId
	UPDATE [CategoryItems] SET [ParentCategoryId] = @NewParentCategoryId WHERE [Id] = @Id;


	return 0;
END
GO
