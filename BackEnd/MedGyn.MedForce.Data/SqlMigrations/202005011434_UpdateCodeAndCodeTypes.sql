


/* ======= This is 202005011434_UpdateCodeAndCodeTypes.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/

sp_rename 'Code.CodeTypeId', 'CodeTypeID', 'COLUMN';

IF COL_LENGTH('Code', 'IsRequired') IS NULL
	ALTER TABLE Code
	ADD IsRequired BIT NOT NULL
		CONSTRAINT D_Code_IsRequired DEFAULT(0)

-- Have to run this in an EXEC in order for the column to exist
EXEC('
UPDATE Code SET IsRequired = 1 WHERE
	(CodeTypeID = 1 AND CodeName = ''IL'')
	OR (CodeTypeID = 2 AND CodeName = ''USA'')
')


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



