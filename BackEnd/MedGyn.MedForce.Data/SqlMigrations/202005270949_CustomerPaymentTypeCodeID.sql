


/* ======= This is 202005270949_CustomerPaymentTypeCodeID.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/



IF EXISTS ( SELECT *
					FROM INFORMATION_SCHEMA.COLUMNS
					WHERE TABLE_NAME = 'Customer'
						AND COLUMN_NAME = 'OtherPaymentType' )
BEGIN
	UPDATE [Customer] SET [OtherPaymentType] = NULL;
	exec sp_rename '[Customer].OtherPaymentType', 'PaymentTypeCodeID', 'COLUMN';
END

ALTER TABLE [Customer]
	ALTER COLUMN [PaymentTypeCodeID] INT NULL



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



