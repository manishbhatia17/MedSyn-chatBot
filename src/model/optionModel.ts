export interface OptionModel {
    label: string;
    action: string;
    isOptionForExistingCustomer?: boolean;
}

export const OptionActions = {
    VIEW_ORDER_STATUS: 'view_order_status',
    REQUEST_PRODUCT_INFO: 'request_product_info',
    PLACE_ORDER: 'place_order',
    REQUEST_REP_INFO: 'request_rep_info',
    LEAVE_MESSAGE: 'leave_message',
} as const;
