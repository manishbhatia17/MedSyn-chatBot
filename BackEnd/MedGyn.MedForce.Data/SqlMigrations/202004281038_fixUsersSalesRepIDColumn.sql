


/* ======= This is 202004281038_fixUsersSalesRepIDColumn.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


sp_rename '[User].SalesRepId', 'SalesRepID', 'COLUMN';

ALTER TABLE [User]
ALTER COLUMN SalesRepId VARCHAR(100)



/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



