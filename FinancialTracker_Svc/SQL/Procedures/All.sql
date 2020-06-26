-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 10:30AM
-- Description:	Create a new bank account for the household the user is assigned to.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[BankAccount_Create]
	@Secret nvarchar(max),
	@Name nvarchar(max),
	@Type int,
	@StartingBalance decimal(18, 2),
	@LowBalanceAlert decimal(18, 2)
AS
BEGIN
	SET NOCOUNT ON;
		
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret', 1;
	DECLARE @CreatorId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);

	IF (CASE WHEN (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId) IS NULL THEN 1 ELSE 0 END) = 1 THROW 51000, 'The user creating this account does not belong to a household.', 1;  

	DECLARE @now datetime = GETDATE();
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId);
	
	INSERT INTO [BankAccounts]
	VALUES(@CreatorId, @houseId, @Type, @Name, @now, null, @StartingBalance, @LowBalanceAlert)


	RETURN @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/25/2020 4:20PM
-- Update date: 6/25/2020 11:02AM
-- Description:	Delete an existing bank account.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[BankAccount_Delete]
	@Id int,
	@Secret nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret.', 1;
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);
	IF NOT EXISTS (SELECT [Id] FROM [BankAccounts] WHERE [Id] = @Id) THROW 51000, 'The Bank Account Id provided returned no records', 1;	 
	
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);
	IF (CASE WHEN (SELECT [ParentHouseholdId] FROM [BankAccounts] WHERE [Id] = @Id) != @houseId THEN 1 ELSE 0 END) = 1 THROW 51000, 'The user removing this account does not belong to the parent household.', 1; 
	
	SET NOCOUNT OFF;
	DELETE FROM [BankAccounts] WHERE [Id] = @Id;
	
	return @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/25/2020 10:50AM
-- Update date:	6/25/2020 10:50AM
-- Description:	Edit an existing account that belongs to a household the user is a member of.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[BankAccount_Edit]
	@Id int,
	@Secret nvarchar(max),
	@NewName nvarchar(max),
	@NewType int,
	@NewLowBalanceAlert decimal(18, 2)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret.', 1;
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);
IF NOT EXISTS (SELECT [Id] FROM [BankAccounts] WHERE [Id] = @Id) THROW 51000, 'The ID provided did not locate a bank account.', 1;  
	
	DECLARE @now datetime = GETDATE();
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);

	IF (CASE WHEN (SELECT [ParentHouseholdId] FROM [BankAccounts] WHERE [Id] = @Id) != @houseId THEN 1 ELSE 0 END) = 1 THROW 51000, 'The user editing this account does not belong to the parent household.', 1; 
	
	DECLARE @oldName nvarchar(max);
	DECLARE @oldType int;
	DECLARE @oldLowBalAlert decimal(18, 2);
	SELECT @oldName = [AccountName]
		  ,@oldType = [AccountTypeId]
		  ,@oldLowBalAlert = [LowBalanceAlertThreshold]
	FROM [BankAccounts]
	WHERE [Id] = @Id	

  
	SET NOCOUNT OFF;
	IF DATALENGTH(@NewName) !=0 AND @NewName != @oldName 
	UPDATE [BankAccounts] SET [AccountName] = @NewName WHERE [Id] = @Id;

	IF @NewType IS NOT NULL AND @NewType != @oldType 
	UPDATE [BankAccounts] SET [AccountTypeId] = @NewType WHERE [Id] = @Id;

	IF @NewLowBalanceAlert IS NOT NULL AND @NewLowBalanceAlert != @oldLowBalAlert 
	UPDATE [BankAccounts] SET [LowBalanceAlertThreshold] = @NewLowBalanceAlert WHERE [Id] = @Id;



	return @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/19/2020 4:06PM
-- Update date: 6/25/2020 2:20PM
-- Description:	Fetch details of an bank account.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[BankAccount_Fetch]
	@Id int = null,
	@Secret nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API Secret.', 1;
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);

	IF @Id IS NULL BEGIN
		DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);
		IF @houseId IS NULL  THROW 51000, 'You must be a member of a household to view accounts.', 1;

		SELECT [Id]
				,[OwnerId]
				,[ParentHouseholdId]
				,[AccountTypeId]
				,[AccountName]
				,[CreatedAt]
				,[ModifiedAt]
				,[StartingBalance]
				,[LowBalanceAlertThreshold]
		FROM [BankAccounts] WHERE [ParentHouseholdId] = @houseId
	END

	IF @Id IS NOT NULL BEGIN
		
		IF NOT EXISTS (SELECT [Id] FROM [BankAccounts] WHERE [Id] = @Id) THROW 51000, 'The Bank Account Id provided returned no records', 1;
		IF NOT (SELECT [ParentHouseholdId] FROM [BankAccounts] WHERE [Id] = @Id) != (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId) THROW 51000, 'You must be a member of the household to view this account.', 1;

		SELECT [Id]
				,[OwnerId]
				,[ParentHouseholdId]
				,[AccountTypeId]
				,[AccountName]
				,[CreatedAt]
				,[ModifiedAt]
				,[StartingBalance]
				,[LowBalanceAlertThreshold]
		FROM [BankAccounts] WHERE [Id] = @Id
	END

	
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 11:08AM
-- Description:	Create a new bank account type for global use in all households.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[BankAccountType_Create]
	@Name nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS (SELECT [Name] FROM [BankAccountTypes] WHERE [Name] = @Name)  THROW 51000, 'A bank account type with this name already exists.', 1; 

	INSERT INTO [BankAccountTypes]
	VALUES(@Name);

	RETURN @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/19/2020 4:20PM
-- Update date: 6/25/2020 11:14AM
-- Description:	Delete an existing bank account type globally accessable by all households.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[BankAccountType_Delete]
	@Id int
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT [Id] FROM [BankAccountTypes] WHERE [Id] = @Id) THROW 51000, 'The ID provided did not locate a bank account type.', 1;
	IF EXISTS (SELECT [Id] FROM [BankAccounts] WHERE [AccountTypeId] = @Id) THROW 51000, 'The Bank Account Type cannot be deleted, as it is still in use.', 1;


	DELETE FROM [BankAccountTypes] WHERE [Id] = @Id;
	
	RETURN @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 11:16AM
-- Description:	Modify an existing bank account type globally accessable by all households.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[BankAccountType_Edit]
	@Id int,
	@NewName nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT [Id] FROM [BankAccountTypes] WHERE [Id] = @Id) THROW 51000, 'The ID provided did not locate a bank account type.', 1;
	IF EXISTS (SELECT [Name] FROM [BankAccountTypes] WHERE [Name] = @NewName)  THROW 51000, 'A bank account type with the new name provided already exists.', 1; 
	

	DECLARE @oldName nvarchar(max);
	SELECT @oldName = [Name]
	FROM [BankAccountTypes] WHERE [Id] = @Id

	SET NOCOUNT OFF;
	IF DATALENGTH(@NewName) != 0 AND @NewName != @oldName
	UPDATE [BankAccountTypes] SET [Name] = @NewName WHERE [Id] = @Id;


	RETURN @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/19/2020 4:06PM
-- Update date: 6/25/2020 2:20PM
-- Description:	Fetch details of an bank account type.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[BankAccountType_Fetch]
	@Id int = null
AS
BEGIN
	SET NOCOUNT ON;

	IF @Id IS NULL BEGIN

		SELECT [Id], [Name] FROM [BankAccountTypes]

	END

	IF @Id IS NOT NULL BEGIN
		IF NOT EXISTS (SELECT [Id] FROM [BankAccountTypes] WHERE [Id] = @Id) THROW 51000, 'The Bank Account Type Id provided returned no records', 1;
		
		SELECT [Id], [Name] FROM [BankAccountTypes] WHERE [Id] = @Id

	END
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 11:24AM
-- Description:	Create a new category for the household the user is assigned to.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[Category_Create]
	@Secret nvarchar(max),
	@Name nvarchar(25),
	@Description nvarchar(500)
AS
BEGIN
	SET NOCOUNT ON;
		
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret', 1; 
	DECLARE @CreatorId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);

	IF (CASE WHEN (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId) IS NULL THEN 1 ELSE 0 END) = 1 THROW 51000, 'The user creating this account does not belong to a household.', 1;  

	DECLARE @now datetime = GETDATE();
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId);
	
	SET NOCOUNT OFF;
	INSERT INTO [Categories]
	VALUES(@houseId, @Name, @Description, @now)


	return @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/25/2020 4:20PM
-- Update date: 6/25/2020 11:25AM
-- Description:	Delete an existing category.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[Category_Delete]
	@Id int,
	@Secret nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret.', 1;
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);
IF NOT EXISTS (SELECT [Id] FROM [Categories] WHERE [Id] = @Id) THROW 51000, 'The category Id provided returned no records', 1;	 
	
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);
	IF (CASE WHEN (SELECT [ParentHouseholdId] FROM [Categories] WHERE [Id] = @Id) != @houseId THEN 1 ELSE 0 END) = 1 THROW 51000, 'The user removing this category does not belong to the parent household.', 1; 
	
	SET NOCOUNT OFF;
	DELETE FROM [Categories] WHERE [Id] = @Id;
	
	return @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/25/2020 10:50AM
-- Update date:	6/25/2020 11:28AM
-- Description:	Edit an existing account that belongs to a household the user is a member of.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[Category_Edit]
	@Id int,
	@Secret nvarchar(max),
	@NewName nvarchar(25),
	@NewDescription nvarchar(500)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret.', 1;
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);
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

    SET NOCOUNT OFF;
	IF DATALENGTH(@NewName) !=0 AND @NewName != @oldName 
	UPDATE [Categories] SET [Name] = @NewName WHERE [Id] = @Id;

	IF DATALENGTH(@NewDescription) !=0  AND @NewDescription != @oldDescription 
	UPDATE [Categories] SET [Description] = @NewDescription WHERE [Id] = @Id;


	return @@ROWCOUNT;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 11:24AM
-- Description:	Create a new category for the household the user is assigned to.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[CategoryItem_Create]
	@Secret nvarchar(max),
	@Name nvarchar(25),
	@Description nvarchar(500),
	@Budget decimal(18, 2),
	@ParentCategoryId int
AS
BEGIN
	SET NOCOUNT ON;
		
	IF @Budget < 0 
		THROW 51000, 'The budget for a subcategory must be greater than or equal to 0.', 1;  

	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) 
		THROW 51000, 'The CreatorId provided does not exist as a UserId.', 1;	
	DECLARE @CreatorId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);

	IF (CASE WHEN (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId) IS NULL THEN 1 ELSE 0 END) = 1 
		THROW 51000, 'The user creating this category item does not belong to a household.', 1; 
		
	IF NOT EXISTS (SELECT [Id] FROM [Categories] WHERE [Id] = @ParentCategoryId) 
		THROW 51000, 'The parent category ID provided does not exist.', 1; 


	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId);
	DECLARE @categoryHouseId int = (SELECT [ParentHouseholdId] FROM [Categories] WHERE [Id] = @ParentCategoryId);
	
	IF @houseId != @categoryHouseId 
		THROW 51000, 'The parent category does not belong to the household of the creator.', 1; 


	SET NOCOUNT OFF;
	INSERT INTO [CategoryItems]
	VALUES(@ParentCategoryId, @Name, @Description, @Budget)



	return @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/25/2020 4:20PM
-- Update date: 6/25/2020 12:00PM
-- Description:	Delete an existing subcategory.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[CategoryItem_Delete]
	@Id int,
	@Secret nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) 
		THROW 51000, 'The CreatorId provided does not exist as a UserId.', 1;	
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);

	IF NOT EXISTS (SELECT [Id] FROM [CategoryItems] WHERE [Id] = @Id) 
		THROW 51000, 'The category item Id provided returned no records', 1;	 	


	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);
	DECLARE @categoryHouseId int = (SELECT c.[ParentHouseholdId] FROM [CategoryItems] ci LEFT JOIN [Categories] c ON ci.[ParentCategoryId] = c.[Id] WHERE ci.[Id] = @Id);
	IF @houseId != @categoryHouseId 
		THROW 51000, 'The parent category does not belong to the household of the creator.', 1; 

	SET NOCOUNT OFF;
	DELETE FROM [CategoryItems] WHERE [Id] = @Id;
	
	return @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/25/2020 10:50AM
-- Update date:	6/25/2020 12:10pM
-- Description:	Edit an existing category item that belongs to a household the user is a member of.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[CategoryItem_Edit]
	@Id int,
	@Secret nvarchar(max),
	@NewName nvarchar(25),
	@NewDescription nvarchar(500),
	@NewBudget decimal(18, 2),
	@NewParentCategoryId int
AS
BEGIN
	SET NOCOUNT ON;

	IF @NewBudget < 0 
		THROW 51000, 'The budget for a subcategory must be greater than or equal to 0.', 1; 
		
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) 
		THROW 51000, 'The CreatorId provided does not exist as a UserId.', 1;	
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);

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

  
	SET NOCOUNT OFF;
	IF DATALENGTH(@NewName) !=0 AND @NewName != @oldName 
	UPDATE [CategoryItems] SET [Name] = @NewName WHERE [Id] = @Id;

	IF DATALENGTH(@NewDescription) !=0  AND @NewDescription != @oldDescription 
	UPDATE [CategoryItems] SET [Description] = @NewDescription WHERE [Id] = @Id;

	IF @NewBudget IS NOT NULL AND @NewBudget != @oldBudget 
	UPDATE [CategoryItems] SET [AmountBudgeted] = @NewBudget WHERE [Id] = @Id;

	IF @NewParentCategoryId IS NOT NULL AND @NewParentCategoryId != @oldParentCategoryId
	UPDATE [CategoryItems] SET [ParentCategoryId] = @NewParentCategoryId WHERE [Id] = @Id;


	RETURN @@ROWCOUNT;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/19/2020 3:50PM
-- Update date: 6/24/2020 5:47PM
-- Description:	Create new Household, and assign it to a user.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[Household_Create]
	@Name nvarchar(25),
	@Greeting nvarchar(255),
	@Secret nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret', 1; 
DECLARE @CreatorId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);
DECLARE @now datetime = GETDATE();

	INSERT INTO [Households]
	VALUES(@Name, @Greeting, @now, @CreatorId)
	
	UPDATE [AspNetUsers]
	SET [HouseholdId] = (SELECT [Id] FROM [Households] WHERE [Name] = @Name AND
															 [Greeting] = @Greeting AND
															 [CreatedAt] = @now AND
															 [CreatorId] = @CreatorId);

	SELECT [Id] FROM [Households] WHERE [Name] = @Name AND
										[Greeting] = @Greeting AND
										[CreatedAt] = @now AND
										[CreatorId] = @CreatorId;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/19/2020 4:20PM
-- Update date: 6/24/2020 4:26PM
-- Description:	Delete an existing household.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[Household_Delete]
	@Id int,
	@Secret nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret.', 1;
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);
	IF NOT EXISTS (SELECT [Id] FROM [Households] WHERE [Id] = @Id) THROW 51000, 'The Household Id provided returned no records', 1;
	IF ((CASE WHEN (SELECT [CreatorId] FROM [Households] WHERE [Id] = @Id) != @CallerId THEN 1 ELSE 0 END) = 1)  THROW 51000, 'The user calling this action is not the owner, and cannot delete the household.', 1;
	
	SET NOCOUNT OFF;
	UPDATE [AspNetUsers] SET [HouseholdId] = NULL WHERE [HouseholdId] = @Id;
	DELETE FROM [Households] WHERE [Id] = @Id;
	
	return @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/19/2020 4:06PM
-- Update date: 6/25/2020 3:03PM
-- Description:	Modify details of an existing household.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[Household_Edit]
	@Id int,
	@Secret nvarchar(max),
	@newName nvarchar(25) = NULL,
	@newGreeting nvarchar(255) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret.', 1;
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);
	IF NOT EXISTS (SELECT [Id] FROM [Households] WHERE [Id] = @Id) THROW 51000, 'The Household Id provided returned no records', 1;
	IF NOT (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId) = @Id  THROW 51000, 'You must be a member of the household to edit it.', 1;


	DECLARE @oldName nvarchar(25);
	DECLARE @oldGreeting nvarchar(25);
	SELECT @oldName = [Name], @oldGreeting = [Greeting] FROM [Households] WHERE [Id] = @Id;
  
	SET NOCOUNT OFF;
	IF DATALENGTH(@newName) !=0 AND @newName != @oldName 
	UPDATE [Households] SET [Name] = @newName WHERE [Id] = @Id;

	IF DATALENGTH(@newGreeting) !=0 AND @newGreeting != @oldGreeting 
	UPDATE [Households] SET [Greeting] = @newGreeting WHERE [Id] = @Id;	

	RETURN @@ROWCOUNT
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/19/2020 4:06PM
-- Update date: 6/25/2020 2:20PM
-- Description:	Fetch details of an existing household.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[Household_Fetch]
	@Id int = null
AS
BEGIN
	IF @Id IS NULL BEGIN
		
		SELECT [Id]
				,[Name]
				,[Greeting]
				,[CreatedAt]
				,[CreatorId]
		FROM [Households]

	END

	IF @Id IS NOT NULL BEGIN
		IF NOT EXISTS (SELECT [Id] FROM [Households] WHERE [Id] = @Id) THROW 51000, 'The Household Id provided returned no records', 1;

		SELECT [Id]
				,[Name]
				,[Greeting]
				,[CreatedAt]
				,[CreatorId]
		FROM [Households] WHERE [Id] = @Id
	END
	
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 1:39PM
-- Description:	Record a new transaction.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[Transaction_Create]
	@Secret nvarchar(max),
	@Name nvarchar(45),
	@Memo nvarchar(120),
	@Amount decimal(18, 2),
	@OccuredAt datetime,
	@ParentAccountId int,
	@TransactionTypeId int,
	@CategoryItemId int
AS
BEGIN
	SET NOCOUNT ON;
		
	IF @Amount <= 0 
		THROW 51000, 'The amount of a transaction must always be greater than 0. If the transaction is an expense, select the appropriate transaction type.', 1;  
		
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret.', 1;
	DECLARE @CreatorId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);

	IF (CASE WHEN (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId) IS NULL THEN 1 ELSE 0 END) = 1 
		THROW 51000, 'The user creating this transaction does not belong to a household.', 1; 		
	IF NOT EXISTS (SELECT [Id] FROM [BankAccounts] WHERE [Id] = @ParentAccountId) 
		THROW 51000, 'The parent bank account ID provided does not exist.', 1; 
	IF NOT EXISTS (SELECT [Id] FROM [TransactionTypes] WHERE [Id] = @TransactionTypeId) 
		THROW 51000, 'The transaction type ID provided does not exist.', 1; 
	IF NOT EXISTS (SELECT [Id] FROM [CategoryItems] WHERE [Id] = @CategoryItemId) 
		THROW 51000, 'The category item ID provided does not exist.', 1; 

	DECLARE @now datetime = (SELECT GETDATE());
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId);
	DECLARE @accountHouseId int = (SELECT [ParentHouseholdId] FROM [BankAccounts] WHERE [Id] = @ParentAccountId);
	DECLARE @categoryItemHouseId int = (SELECT c.[ParentHouseholdId] FROM [CategoryItems] ci 
										LEFT JOIN [Categories] c ON ci.[ParentCategoryId] = c.[Id] 
										WHERE ci.[Id] = @CategoryItemId);
	
	IF @houseId != @CategoryItemId 
		THROW 51000, 'The assigned category does not belong to the household of the creator.', 1; 
	IF @houseId != @accountHouseId 
		THROW 51000, 'The assigned bank account does not belong to the household of the creator.', 1; 


	SET NOCOUNT OFF;
	INSERT INTO [Transactions]
	VALUES(@ParentAccountId, @TransactionTypeId, @CategoryItemId, @CreatorId, @Name, @Memo, @Amount, @OccuredAt, @now)



	RETURN @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/25/2020 4:20PM
-- Update date: 6/25/2020 1:39PM
-- Description:	Delete an existing transaction.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[Transaction_Delete]
	@Id int,
	@Secret nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret.', 1;
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);

	IF NOT EXISTS (SELECT [Id] FROM [Transactions] WHERE [Id] = @Id) 
		THROW 51000, 'The transaction id provided returned no records', 1;	 	
	


	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);
	DECLARE @transHouseId int = (SELECT h.[Id] FROM [Transactions] t LEFT JOIN [BankAccounts] b ON t.[ParentAccountId] = b.[Id]
																	 LEFT JOIN [Households] h ON b.[ParentHouseholdId] = h.[Id]
																	 WHERE t.[Id] = @Id);
	IF @houseId != @transHouseId 
		THROW 51000, 'The transaction does not belong to the household of the caller.', 1; 



	DECLARE @houseOwner nvarchar(max) = (SELECT h.[CreatorId] FROM [Transactions] t LEFT JOIN [BankAccounts] b ON t.[ParentAccountId] = b.[Id]
																					LEFT JOIN [Households] h ON b.[ParentHouseholdId] = h.[Id]
																					WHERE t.[Id] = @Id);
	DECLARE @accOwner nvarchar(max) = (SELECT b.[OwnerId] FROM [Transactions] t LEFT JOIN [BankAccounts] b ON t.[ParentAccountId] = b.[Id]
																				WHERE t.[Id] = @Id);
	DECLARE @transOwner nvarchar(max) = (SELECT [OwnerId] FROM [Transactions] WHERE [Id] = @Id);

	IF @houseOwner != @CallerId AND @accOwner != @CallerId AND @transOwner != @CallerId
		THROW 51000, 'Only the owner of the household, bank account, or the transaction may delete transaction records.', 1;	 	
		

		
	SET NOCOUNT OFF;
	DELETE FROM [Transactions] WHERE [Id] = @Id;
	
	RETURN @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/25/2020 10:50AM
-- Update date:	6/25/2020 1:39PM
-- Description:	Edit an existing transaction.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[Transaction_Edit]
	@Id int,
	@Secret nvarchar(max),
	@NewName nvarchar(45),
	@NewMemo nvarchar(120),
	@NewAmount decimal(18, 2),
	@NewOccuredAt datetime,
	@NewParentAccountId int,
	@NewTransactionTypeId int,
	@NewCategoryItemId int
AS
BEGIN
	SET NOCOUNT ON;
	
	IF @NewAmount IS NOT NULL AND @NewAmount <= 0 
		THROW 51000, 'The amount of a transaction must always be greater than 0. If the transaction is an expense, select the appropriate transaction type.', 1;  
			
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret) THROW 51000, 'Bad API secret.', 1;
	DECLARE @CallerId nvarchar(max) = (SELECT [Id] FROM [AspNetUsers] WHERE [ApiSecret] = @Secret);

	IF (CASE WHEN (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId) IS NULL THEN 1 ELSE 0 END) = 1 
		THROW 51000, 'The user editing this transaction does not belong to a household.', 1; 		
	IF @NewParentAccountId IS NOT NULL AND NOT EXISTS (SELECT [Id] FROM [BankAccounts] WHERE [Id] = @NewParentAccountId) 
		THROW 51000, 'The new parent bank account ID provided does not exist.', 1; 
	IF @NewTransactionTypeId IS NOT NULL AND NOT EXISTS (SELECT [Id] FROM [TransactionTypes] WHERE [Id] = @NewTransactionTypeId) 
		THROW 51000, 'The new transaction type ID provided does not exist.', 1; 
	IF @NewCategoryItemId IS NOT NULL AND NOT EXISTS (SELECT [Id] FROM [CategoryItems] WHERE [Id] = @NewCategoryItemId) 
		THROW 51000, 'The new category item ID provided does not exist.', 1; 
	 
	 
	DECLARE @houseOwner nvarchar(max) = (SELECT h.[CreatorId] FROM [Transactions] t LEFT JOIN [BankAccounts] b ON t.[ParentAccountId] = b.[Id]
																					LEFT JOIN [Households] h ON b.[ParentHouseholdId] = h.[Id]
																					WHERE t.[Id] = @Id);
	DECLARE @accOwner nvarchar(max) = (SELECT b.[OwnerId] FROM [Transactions] t LEFT JOIN [BankAccounts] b ON t.[ParentAccountId] = b.[Id]
																				WHERE t.[Id] = @Id);
	DECLARE @transOwner nvarchar(max) = (SELECT [OwnerId] FROM [Transactions] WHERE [Id] = @Id);
	IF @houseOwner != @CallerId AND @accOwner != @CallerId AND @transOwner != @CallerId
		THROW 51000, 'Only the owner of the household, bank account, or the transaction may edit transaction records.', 1;



	DECLARE @oldName nvarchar(25);
	DECLARE @oldMemo nvarchar(500);
	DECLARE @oldAmount decimal(18, 2);
	DECLARE @oldOccuredAt datetime;
	DECLARE @oldParentAccountId int;
	DECLARE @oldTransTypeId int;
	DECLARE @oldCategoryItemId int;
	SELECT @oldName = [Name]
		  ,@oldMemo = [Memo]
		  ,@oldAmount = [Amount]
		  ,@oldOccuredAt = [OccuredAt]
		  ,@oldParentAccountId = [ParentAccountId]
		  ,@oldTransTypeId = [TransactionTypeId]
		  ,@oldCategoryItemId = [CategoryItemId]
	FROM [Transactions]
	WHERE [Id] = @Id	

  
	SET NOCOUNT OFF;
	IF DATALENGTH(@NewName) !=0 AND @NewName != @oldName 
	UPDATE [Transactions] SET [Name] = @NewName WHERE [Id] = @Id;

	IF DATALENGTH(@NewMemo) !=0  AND @NewMemo != @oldMemo 
	UPDATE [Transactions] SET [Memo] = @NewMemo WHERE [Id] = @Id;

	IF @NewAmount IS NOT NULL AND @NewAmount != @oldAmount 
	UPDATE [Transactions] SET [Amount] = @NewAmount WHERE [Id] = @Id;

	IF @NewOccuredAt IS NOT NULL AND @NewOccuredAt != @oldOccuredAt 
	UPDATE [Transactions] SET [OccuredAt] = @NewOccuredAt WHERE [Id] = @Id;

	IF @NewParentAccountId IS NOT NULL AND @NewParentAccountId != @oldParentAccountId 
	UPDATE [Transactions] SET [ParentAccountId] = @NewParentAccountId WHERE [Id] = @Id;

	IF @NewTransactionTypeId IS NOT NULL AND @NewTransactionTypeId != @oldTransTypeId 
	UPDATE [Transactions] SET [TransactionTypeId] = @NewTransactionTypeId WHERE [Id] = @Id;

	IF @NewCategoryItemId IS NOT NULL AND @NewCategoryItemId != @oldCategoryItemId
	UPDATE [Transactions] SET [CategoryItemId] = @NewCategoryItemId WHERE [Id] = @Id;


	RETURN @@ROWCOUNT;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 12:52 PM
-- Description:	Create a new transaction type for global use in all households.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[TransactionType_Create]
	@Name nvarchar(max),
	@Description nvarchar(125),
	@IsIncome bit
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS (SELECT [Name] FROM [TransactionTypes] WHERE [Name] = @Name)  THROW 51000, 'A transaction type with this name already exists.', 1; 
	
	SET NOCOUNT OFF;
	INSERT INTO [TransactionTypes]
	VALUES(@Name, @Description, @IsIncome);

	RETURN @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/19/2020 4:20PM
-- Update date: 6/25/2020 12:52 PM
-- Description:	Delete an existing transaction type globally accessable by all households.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[TransactionType_Delete]
	@Id int
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT [Id] FROM [TransactionTypes] WHERE [Id] = @Id) THROW 51000, 'The ID provided did not locate a transaction type.', 1;
	IF EXISTS (SELECT TOP(1) [Id] FROM [Transactions] WHERE [TransactionTypeId] = @Id) THROW 51000, 'This transaction type is still in use, and cannot be deleted.', 1;
	
	SET NOCOUNT OFF;
	DELETE FROM [TransactionTypes] WHERE [Id] = @Id;
	
	RETURN @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 12:52 PM
-- Description:	Modify an existing transaction type globally accessable by all households.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[TransactionType_Edit]
	@Id int,
	@NewName nvarchar(max) = NULL,
	@NewDescription nvarchar(125) = NULL,
	@IsStillIncome bit = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS (SELECT [Id] FROM [TransactionTypes] WHERE [Id] = @Id) THROW 51000, 'The ID provided did not locate a transaction type.', 1;
	IF EXISTS (SELECT [Name] FROM [TransactionTypes] WHERE [Name] = @NewName)  THROW 51000, 'A transaction type with the new name provided already exists.', 1; 
	

	DECLARE @oldName nvarchar(max);
	DECLARE @oldDescription nvarchar(125);
	Declare @isIncome bit;
	SELECT @oldName = [Name]
		  ,@oldDescription = [Description]
		  ,@isIncome = [IsIncome]
	FROM [TransactionTypes] WHERE [Id] = @Id

	
	SET NOCOUNT OFF;
	IF DATALENGTH(@NewName) != 0 AND @NewName != @oldName
	UPDATE [TransactionTypes] SET [Name] = @NewName WHERE [Id] = @Id;

	IF DATALENGTH(@NewDescription) != 0 AND @NewDescription != @oldDescription
	UPDATE [TransactionTypes] SET [Description] = @NewDescription WHERE [Id] = @Id;

	IF @IsStillIncome IS NOT NULL AND @IsStillIncome != @isIncome
	UPDATE [TransactionTypes] SET [IsIncome] = @IsStillIncome WHERE [Id] = @Id;


	RETURN @@ROWCOUNT;
END
GO
-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/25/2020 10:28PM
-- Update date: 6/25/2020 10:29PM
-- Description:	Fetch details of an bank account type.
-- =============================================
CREATE OR ALTER  PROCEDURE [dbo].[TransactionType_Fetch]
	@Id int = null
AS
BEGIN
	SET NOCOUNT ON;

	IF @Id IS NULL BEGIN

		SELECT [Id], [Name], [Description], [IsIncome] FROM [TransactionTypes]

	END

	IF @Id IS NOT NULL BEGIN
		IF NOT EXISTS (SELECT [Id] FROM [TransactionTypes] WHERE [Id] = @Id) THROW 51000, 'The Transaction Type Id provided returned no records', 1;
		
		SELECT [Id], [Name], [Description], [IsIncome] FROM [TransactionTypes] WHERE [Id] = @Id

	END
END
GO
