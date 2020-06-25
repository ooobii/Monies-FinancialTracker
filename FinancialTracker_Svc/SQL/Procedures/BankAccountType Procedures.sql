-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 11:08AM
-- Description:	Create a new bank account type for global use in all households.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[BankAccountType_Create]
	@Name nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS (SELECT [Name] FROM [BankAccountTypes] WHERE [Name] = @Name)  THROW 51000, 'A bank account type with this name already exists.', 1; 

	INSERT INTO [BankAccountTypes]
	VALUES(@Name);

	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date: 6/19/2020 4:20PM
-- Update date: 6/25/2020 11:14AM
-- Description:	Delete an existing bank account type globally accessable by all households.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[BankAccountType_Delete]
	@Id int
AS
BEGIN
	SET NOCOUNT ON;
	
	IF NOT EXISTS (SELECT [Id] FROM [BankAccountTypes] WHERE [Id] = @Id) THROW 51000, 'The ID provided did not locate a bank account type.', 1;
	
	DELETE FROM [BankAccountTypes] WHERE [Id] = @Id;
	
	return 0;
END
GO



-- =============================================
-- Author:		Matthew Wendel
-- Create date:	6/24/2020 4:32PM
-- Update date:	6/25/2020 11:16AM
-- Description:	Modify an existing bank account type globally accessable by all households.
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[BankAccountType_Edit]
	@Id int,
	@NewName nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
	
	IF EXISTS (SELECT [Id] FROM [BankAccountTypes] WHERE [Id] = @Id) THROW 51000, 'The ID provided did not locate a bank account type.', 1;
	IF EXISTS (SELECT [Name] FROM [BankAccountTypes] WHERE [Name] = @NewName)  THROW 51000, 'A bank account type with the new name provided already exists.', 1; 
	

	DECLARE @oldName nvarchar(max);
	SELECT @oldName = [Name]
	FROM [BankAccountTypes] WHERE [Id] = @Id

	IF DATALENGTH(@NewName) != 0 AND @NewName != @oldName
	UPDATE [BankAccountTypes] SET [Name] = @NewName WHERE [Id] = @Id;


	return 0;
END
GO
