SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nem Ekpunobi
-- Create date: April 3, 2019
-- Description:	Creates Orders
-- =============================================
USE [certitrack]
GO

CREATE PROCEDURE stpCreateOrder
	@customer_id int,
	@order_qty int = 0, -- defaults procedure to display customer orders only
	@certificate_price money = 30
AS
BEGIN
	SET NOCOUNT ON;

	IF @order_qty > 0
	BEGIN
		-- CREATE ORDER --
		INSERT INTO [order] ([customer_id])
		VALUES (@customer_id)

		-- GET LATEST ORDER ID --
		DECLARE @order_id int;
		--
		SELECT @order_id = [id] FROM [order]
		WHERE [customer_id] LIKE @customer_id

		-- GENERATE EACH ORDER ITEM --
		DECLARE @status int = 0
		--
		WHILE (@order_qty > 0)
		BEGIN
			-- GENERATE CERTIFICATE AND GET ID --
			DECLARE @genCertId int
			EXECUTE @genCertId = [stpGenerateCertificate]
				@price = @certificate_price

			-- INJECTS CERTIFICATE ID & CUSTOMER ID INTO NEW [certificate_link] RECORD --
			INSERT INTO [certificate_link] ([certificate_id], [customer_id])
			VALUES (@genCertId, @customer_id)

			-- CREATE ORDER ITEM --
			INSERT INTO [order_item] (order_id, certificate_id)
			VALUES (@order_id, @genCertId)

			-- DECREMENT ORDER QTY COUNTER --
			SET @order_qty -= 1

			-- SET SUCCESS STATUS --
			SET @status = 1
		END

		-- DISPLAY RESULTING ORDER ITEMS --
		SELECT * FROM [order_item] WHERE [order_id] LIKE @order_id
	END

	-- DISPLAY ALL CUSTOMER ORDERS --
	SELECT * FROM [order] WHERE [customer_id] LIKE @customer_id

	RETURN @status
END
GO
