-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 12:52 PM
-- Description:	Create a new transaction type for global use in all households.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[TransactionType_Create]
	@Name nvarchar(max),
	@Description nvarchar(125),
	@IsIncome bit
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS (SELECT [Name] FROM [TransactionTypes] WHERE [Name] = @Name)  THROW 51000, 'A transaction type with this name already exists.', 1; 

	INSERT INTO [TransactionTypes]
	VALUES(@Name, @Description, @IsIncome);

	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/19/2020 4:20PM
-- Update date: 6/25/2020 12:52 PM
-- Description:	Delete an existing transaction type globally accessable by all households.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[TransactionType_Delete]
	@Id int
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT [Id] FROM [TransactionTypes] WHERE [Id] = @Id) THROW 51000, 'The ID provided did not locate a transaction type.', 1;
	
	DELETE FROM [TransactionTypes] WHERE [Id] = @Id;
	
	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 12:52 PM
-- Description:	Modify an existing transaction type globally accessable by all households.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[TransactionType_Edit]
	@Id int,
	@NewName nvarchar(max),
	@NewDescription nvarchar(125),
	@IsStillIncome bit
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


	IF DATALENGTH(@NewName) != 0 AND @NewName != @oldName
	UPDATE [TransactionTypes] SET [Name] = @NewName WHERE [Id] = @Id;

	IF DATALENGTH(@NewDescription) != 0 AND @NewDescription != @oldDescription
	UPDATE [TransactionTypes] SET [Description] = @NewDescription WHERE [Id] = @Id;

	IF @IsStillIncome IS NOT NULL AND @IsStillIncome != @isIncome
	UPDATE [TransactionTypes] SET [IsIncome] = @IsStillIncome WHERE [Id] = @Id;


	return 0;
END
GO
