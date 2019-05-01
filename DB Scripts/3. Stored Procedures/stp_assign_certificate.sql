SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 3, 2019
-- Description:	Populates [certificate_link] Table
-- =============================================
USE [certitrack]
GO

CREATE PROCEDURE stpAssignCertificate

	-- CUSTOMER CREATION PARAMS --
	@customer_name varchar(50),
	@customer_email varchar(50),
	@customer_phone varchar(15),

	-- STAFF PARAMS --
	@staff_name varchar(50),

	-- PROMO PARAMS --
	@promo_discount int,

	-- CHANNEL PARAMS --
	@channel varchar(30),

	-- ORDER CREATION PARAMS --
	@cert_price money = 30,
	@cert_qty int = 1

AS
BEGIN
	SET NOCOUNT ON;

    DECLARE -- Global Vars To Store Values for Table Population
			@staff_id int,
			@customer_id int,
			@promotion_id int,
			@channel_id int


	-- GET OR CREATE CUSTOMER --
	-- ========================= --
	/*	PASS IN CUSTOMER NAME, EMAIL, PHONE
		--
		@stpCC returns customer creation status
		-> success = 1, otherwise 0
		-> also outputs @id as @customer_id, matching customer details
	*/
	BEGIN
		DECLARE @stpGCC int;

		EXECUTE @stpGCC = [dbo].[stpGetCreateCustomer]
		   @name = @customer_name
		  ,@email = @customer_email
		  ,@phone = @customer_phone
		  ,@id = @customer_id output
	
		SELECT @stpGCC AS 'Customer Creation Status - stpAssignCertificate'
		SELECT @customer_id AS 'Customer ID - stpAssignCertificate'
	END

	-- GET SELECTED STAFF ID FROM PRE-POPULATED TABLE --
	-- ============================================== --
	/*	PASS IN STAFF NAME 
		--
		@stpGCS outputs @id as @staff_id, matching staff details
	*/
	BEGIN
		DECLARE @stpGCS int

		EXECUTE @stpGCS = [dbo].[stpGetCreateStaff] 
			@name = @staff_name,
			@email = null,
			@password = null,
			@id = @staff_id OUTPUT

		SELECT @staff_id AS 'Staff ID - stpAssignCertificate'
	END

	-- POPULATE TABLE VIA ORDER CREATION --
	-- ================================= --
	/*	PASS IN CUSTOMER ID
		--
		@stpCO returns order creation status
		-> success = 1, otherwise 0
		-> inserts [customer_id] and [certificate_id] into
		   [certificate_link] for each order item created
	*/
	IF @customer_id IS NOT NULL
	BEGIN
		DECLARE @stpCO int
	
		EXECUTE @stpCO = [dbo].[stpCreateOrder] 
		   @customer_id,
		   @order_qty = @cert_qty,
		   @certificate_price = @cert_price

		SELECT @stpCO AS 'Order Creation Status - stpAssignCertificate'
	END
	ELSE RETURN -- Ends execution if no customer id supplied

	-- POPULATE REMAINING FIELDS --
	-- ========================= --
	BEGIN
		/*
			Get [promotion_id]
		*/
		SELECT @promotion_id = [id] FROM [promotion]
		WHERE [discount] LIKE @promo_discount
		/*
			Get [channel_id]
		*/
		SELECT @channel_id = [id] FROM [channel]
		WHERE [channel] LIKE @channel

		DECLARE @certificate_id int,
				@count int = @cert_qty -- while loop iterator
		/*
			Populate Fields for Each Certificate Generated
		*/
		WHILE @count > 0
		BEGIN
			SET @count -= 1
			/*
				Get [certificate_id]
			*/
			SELECT TOP(1) @certificate_id = [certificate_id] FROM [certificate_link]
			WHERE [customer_id] LIKE @customer_id AND (
				[staff_id] IS NULL OR
				[promotion_id] IS NULL OR
				[channel_id] IS NULL)
			ORDER BY [certificate_id] DESC

			SELECT @certificate_id AS 'Cert ID - stpAssignCertificate'
			/*
				Populate [staff_id] Field
			*/
			UPDATE [certificate_link]
			SET [staff_id] = @staff_id
			WHERE [certificate_id] LIKE @certificate_id
			AND [staff_id] IS NULL
			/*
				Populate [promotion_id] Field
			*/
			UPDATE [certificate_link]
			SET [promotion_id] = @promotion_id
			WHERE [certificate_id] LIKE @certificate_id
			AND [promotion_id] IS NULL
			/*
				Populate [channel_id] Field
			*/
			UPDATE [certificate_link]
			SET [channel_id] = @channel_id
			WHERE [certificate_id] LIKE @certificate_id
			AND [channel_id] IS NULL
		END
	END
END
GO