public enum CustomerOrderStatusEnum
{
	WaitingSubmission      = 1,
	WaitingManagerApproval = 2,
	WaitingVPApproval      = 3,
	ToBeFilled             = 4,
	ToBeShipped            = 5,
	// FilledToday            = 6,
	ToBeInvoiced           = 7,
	ShowMyOrders           = 8,
	OnBackOrder            = 9,
	DoNotFill              = 10,
	Filling                = 11,
	HasBeenInvoiced        = 12
}