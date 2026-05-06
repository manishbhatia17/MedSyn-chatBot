UPDATE Customer
SET SearchField = convert(varchar(880), ISNULL(CustomerName, '') + ISNULL(CustomerCustomID, '') 
	+ ISNULL(Address1, '') + ISNULL(City, ''));
