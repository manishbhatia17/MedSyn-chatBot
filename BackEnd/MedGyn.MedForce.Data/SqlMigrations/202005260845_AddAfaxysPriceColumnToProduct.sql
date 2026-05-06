


/* ======= This is 202005260845_AddAfaxysPriceColumnToProduct.sql === */
/* ======= Your  SQL code goes here =================================*/
/* ==========================================================================*/


IF COL_LENGTH('Product', 'PriceDomesticAfaxys') IS NULL
	ALTER TABLE [Product]
		ADD [PriceDomesticAfaxys] MONEY


/* ==========================================================================*/
/* ======== Your SQL code was above here ====================================*/



