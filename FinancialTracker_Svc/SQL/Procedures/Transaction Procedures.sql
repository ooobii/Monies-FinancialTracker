-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 1:39PM
-- Description:	Record a new transaction.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Transaction_Create]
	@CreatorId nvarchar(max),
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

	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CreatorId) 
		THROW 51000, 'The CreatorId provided does not exist as a UserId.', 1;
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



	INSERT INTO [Transactions]
	VALUES(@ParentAccountId, @TransactionTypeId, @CategoryItemId, @CreatorId, @Name, @Memo, @Amount, @OccuredAt, @now)



	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/25/2020 4:20PM
-- Update date: 6/25/2020 1:39PM
-- Description:	Delete an existing transaction.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Transaction_Delete]
	@Id int,
	@CallerId nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CallerId) 
		THROW 51000, 'The CallerID does not exist as a user.', 1; 

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
		


	DELETE FROM [Transactions] WHERE [Id] = @Id;
	
	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/25/2020 10:50AM
-- Update date:	6/25/2020 1:39PM
-- Description:	Edit an existing transaction.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[Transaction_Edit]
	@Id int,
	@CallerId nvarchar(max),
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
		
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CallerId) 
		THROW 51000, 'The CallerID does not exist as a user.', 1; 
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


	return 0;
END
GO
