USE [certitrack]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 10, 2019
-- Description:	Populates [staff_link] Table
--	1. Creates Staff if Doesn't Exist
--	2. Get Staff Role
--	3. Get Staff Type Id
--	4. Create and/or Get a Staff Link Set
-- =============================================
alter PROCEDURE stpAssignStaff
	-- STAFF PARAMS --
	@staff_name varchar(45),
	@staff_email varchar(50),
	@staff_pw varchar(255),
	-- STAFF ROLE PARAMS --
	@role_title varchar(30),
	-- STAFF TYPE PARAMS --
	@staff_type varchar(45),
	-- MESSAGE OUT PARAM --
	@message_out varchar(50) OUTPUT,
	-- STAFF CREATION STATUS OUT PARAM --
	@staff_created INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	-- GLOBAL VARS --
	DECLARE @staff_id int,
			@role_id int,
			@staff_type_id int,
			@message varchar(50)

	SET		@staff_created = 0


	-- 1. CREATES STAFF IF DOESN'T EXIST --
	-- ================================= --
	/*
		@stpGCS returns staff creation status
		-> success = 1, otherwise 0
		-> outputs staff @id as @staff_id
	*/
	IF NOT EXISTS (
		SELECT 1 FROM [staff]
		WHERE [email] LIKE @staff_email )
	BEGIN
		DECLARE @stpGCS int

		EXECUTE @stpGCS = [dbo].[stpGetCreateStaff] 
		   @name = @staff_name
		  ,@email = @staff_email
		  ,@password = @staff_pw
		  ,@id = @staff_id OUTPUT

		SELECT @staff_id AS 'Staff ID - stpAssignStaff'
		SELECT @stpGCS AS 'Staff Creation Status - stpAssignStaff'

		SET @staff_created = 1
	END
	/*
		SETS @staff_id TO EXISTING STAFF MATCHING SUPPLIED DETAILS
	*/
	ELSE
	BEGIN
		SELECT @staff_id = [id] FROM [staff]
		WHERE [email] LIKE @staff_email

		SET @message = CONCAT('Staff Already Exists - ID: ', @staff_id, ' - stpAssignStaff')
		SELECT @message AS 'Message - stpAssignStaff'

		SET @message_out = CONCAT('Staff Already Exists - ID: ', @staff_id)

		SET @staff_created = 0
	END

	-- 2. GETS STAFF ROLE --
	-- ================== --
	SELECT @role_id = [id] FROM [role]
	WHERE [title] LIKE @role_title

	-- 3. GETS STAFF TYPE --
	-- ================== --
	SELECT @staff_type_id = [id] FROM [staff_type]
	WHERE [type] LIKE @staff_type

	-- 4. CREATE AND/OR GET A STAFF LINK SET --
	-- ===================================== --
	/*
		CREATE [staff_link] RECORD IF DOESN'T EXIST FOR SELECTED STAFF
	*/
	IF NOT EXISTS ( 
		SELECT 1 FROM [staff_link]
		WHERE [staff_id] LIKE @staff_id )
	BEGIN
		INSERT INTO [staff_link] ([staff_id], [role_id], [staff_type_id])
		VALUES (@staff_id, @role_id, @staff_type_id) 
	END
	/*
		GET [staff_link] RECORD FOR STAFF
	*/
	SELECT * FROM [staff_link]
	WHERE [staff_id] LIKE @staff_id

	RETURN @staff_created
END
GO
