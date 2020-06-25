-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 10:30AM
-- Description:	Create a new bank account for the household the user is assigned to.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[BankAccount_Create]
	@CreatorId nvarchar(max),
	@Name nvarchar(max),
	@Type int,
	@StartingBalance decimal(18, 2),
	@LowBalanceAlert decimal(18, 2)
AS
BEGIN
	SET NOCOUNT ON;
		
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CreatorId) THROW 51000, 'The CreatorId provided does not exist as a UserId.', 1;  
	IF (CASE WHEN (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId) IS NULL THEN 1 ELSE 0 END) = 1 THROW 51000, 'The user creating this account does not belong to a household.', 1;  

	DECLARE @now datetime = GETDATE();
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CreatorId);
	
	INSERT INTO [BankAccounts]
	VALUES(@CreatorId, @houseId, @Type, @Name, @now, null, @StartingBalance, @LowBalanceAlert)


	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/25/2020 10:50AM
-- Update date:	6/25/2020 10:50AM
-- Description:	Edit an existing account that belongs to a household the user is a member of.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[BankAccount_Edit]
	@Id int,
	@CallerId nvarchar(max),
	@NewName nvarchar(max),
	@NewType int,
	@NewLowBalanceAlert decimal(18, 2)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CallerId) THROW 51000, 'The CallerID does not exist as a user.', 1; 
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

  
	IF DATALENGTH(@NewName) !=0 AND @NewName != @oldName 
	UPDATE [BankAccounts] SET [AccountName] = @NewName WHERE [Id] = @Id;

	IF @NewType IS NOT NULL AND @NewType != @oldType 
	UPDATE [BankAccounts] SET [AccountTypeId] = @NewType WHERE [Id] = @Id;

	IF @NewLowBalanceAlert IS NOT NULL AND @NewLowBalanceAlert != @oldLowBalAlert 
	UPDATE [BankAccounts] SET [LowBalanceAlertThreshold] = @NewLowBalanceAlert WHERE [Id] = @Id;



	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/25/2020 4:20PM
-- Update date: 6/25/2020 11:02AM
-- Description:	Delete an existing bank account.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[BankAccount_Delete]
	@Id int,
	@CallerId nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	IF NOT EXISTS (SELECT [Id] FROM [AspNetUsers] WHERE [Id] = @CallerId) THROW 51000, 'The CallerID does not exist as a user.', 1; 
	IF NOT EXISTS (SELECT [Id] FROM [BankAccounts] WHERE [Id] = @Id) THROW 51000, 'The Bank Account Id provided returned no records', 1;	 
	
	DECLARE @houseId int = (SELECT [HouseholdId] FROM [AspNetUsers] WHERE [Id] = @CallerId);
	IF (CASE WHEN (SELECT [ParentHouseholdId] FROM [BankAccounts] WHERE [Id] = @Id) != @houseId THEN 1 ELSE 0 END) = 1 THROW 51000, 'The user editing this account does not belong to the parent household.', 1; 
	
	DELETE FROM [BankAccounts] WHERE [Id] = @Id;
	
	return 0;
END
GO
