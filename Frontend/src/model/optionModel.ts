export interface OptionModel {
    label: string;
    action: string;
    value?: string;  // action value/identifier
    optionalData?: any;  // additional metadata
    rule?: string;  // reference to next rule id
}

export const RuleMeta = {
    MainMenuOption: {id:'main_menu_option', intent:'start_chat'},
    REQUEST_PRODUCT_INFO: {id:'request_product_info', intent:'request_product_info'},
    REQUEST_REP_INFO: {id:'request_rep_info', intent:'request_rep_info'},
    REQUEST_ORDER_STATUS: {id:'request_order_status', intent:'request_order_status'},
    REQUEST_ORDER_INVOICE: {id:'request_order_invoice', intent:'request_order_invoice'},
    REQUEST_ORDER_TRACKING: {id:'request_order_tracking', intent:'request_order_tracking'},    
    LEAVE_MESSAGE: {id:'leave_message', intent:'leave_message'},
};

export enum ActionType  {
    DisplayMessage = 'DisplayMessage',
    DisplayMenu = 'DisplayMenu',
    CollectInput = 'CollectInput',
    CallAPI =   'CallAPI',
    ProcessRule = 'ProcessRule'
}
export enum Priority {
    Critical,
    High,
    Medium,
    Low
}