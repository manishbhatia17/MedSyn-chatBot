import { ChatRule } from "src/model/chatMessage";
import { ActionType, RuleMeta, OptionModel, Priority } from "src/model/optionModel";

export const chatRules:ChatRule[] = [
    {
        id: RuleMeta.MainMenuOption.id,
        intent: RuleMeta.MainMenuOption.intent,
        response: `Hi, we're here to help you
        Please select one of the following options to get started:`,
        priority: Priority.Low,
        action: ActionType.DisplayMenu,
        actionPayload: [
            {
                label: "Receive product information",
                action: ActionType.ProcessRule,
                value: RuleMeta.REQUEST_PRODUCT_INFO.id,
                optionalData: {
                    isOptionForNewCustomer: true
                },
                rule: RuleMeta.REQUEST_PRODUCT_INFO.id
            },
            {
                label: "Receive information for your rep",
                action: RuleMeta.REQUEST_REP_INFO.intent,
                isOptionForExistingCustomer: false,
                value: RuleMeta.REQUEST_REP_INFO.id,
                optionalData: {
                    isOptionForNewCustomer: true
                },
                rule: RuleMeta.REQUEST_REP_INFO.id
            },
            {
                label: "Order status",
                action: RuleMeta.REQUEST_ORDER_STATUS.intent,
                value: RuleMeta.REQUEST_ORDER_STATUS.id,
                optionalData: {
                    isOptionForNewCustomer: false
                },
                rule: RuleMeta.REQUEST_ORDER_STATUS.id
            },
            {
                label: "PO Details",
                action: RuleMeta.REQUEST_ORDER_INVOICE.intent,
                value: RuleMeta.REQUEST_ORDER_INVOICE.id,
                optionalData: {
                    isOptionForNewCustomer: false
                },
                rule: RuleMeta.REQUEST_ORDER_INVOICE.id
            },
            {
                label: "Order tracking number",
                action: RuleMeta.REQUEST_ORDER_TRACKING.intent,
                value: RuleMeta.REQUEST_ORDER_TRACKING.id,
                optionalData: {
                    isOptionForNewCustomer: false
                },
                rule: RuleMeta.REQUEST_ORDER_TRACKING.id
            },
            {
                label: "Invoice Details",
                action: RuleMeta.REQUEST_INVOICE_VIEW.intent,
                value: RuleMeta.REQUEST_INVOICE_VIEW.id,
                optionalData: {
                    isOptionForNewCustomer: false
                },
                rule: RuleMeta.REQUEST_INVOICE_VIEW.id
            },
            {
                label: "Leave a message",
                action: RuleMeta.LEAVE_MESSAGE.intent,
                isOptionForExistingCustomer: false,
                value: RuleMeta.LEAVE_MESSAGE.id,
                optionalData: {
                    isOptionForNewCustomer: true
                },
                rule: RuleMeta.LEAVE_MESSAGE.id
            }
        ] as OptionModel[]
    },
    {
        id: RuleMeta.REQUEST_PRODUCT_INFO.id,
        intent: RuleMeta.REQUEST_PRODUCT_INFO.intent,
        response: `Please enter your product information or details.`,
        priority: Priority.High,
        action: ActionType.DisplayMessage
    },
    {
        id: RuleMeta.REQUEST_REP_INFO.id,
        intent: RuleMeta.REQUEST_REP_INFO.intent,
        response: ``,
        priority: Priority.High,
        action: ActionType.CallAPI,
        actionPayload: 'GetRepersentativeByCountryOrState'
    },
    {
        id: RuleMeta.REQUEST_ORDER_STATUS.id,
        intent: RuleMeta.REQUEST_ORDER_STATUS.intent,
        response: `Please provide your purchase order number.`,
        priority: Priority.High,
        action: ActionType.DisplayMessage
    },
    {
        id: RuleMeta.REQUEST_ORDER_INVOICE.id,
        intent: RuleMeta.REQUEST_ORDER_INVOICE.intent,
        response: `Please provide your purchase order number.`,
        priority: Priority.High,
        action: ActionType.DisplayMessage
    },
    {
        id: RuleMeta.REQUEST_ORDER_TRACKING.id,
        intent: RuleMeta.REQUEST_ORDER_TRACKING.intent,
        response: `Please provide your purchase order number.`,
        priority: Priority.High,
        action: ActionType.DisplayMessage
    },
    {
        id: RuleMeta.REQUEST_INVOICE_VIEW.id,
        intent: RuleMeta.REQUEST_INVOICE_VIEW.intent,
        response: `Please provide your purchase order number to view the invoice.`,
        priority: Priority.High,
        action: ActionType.DisplayMessage
    },
    {
        id: RuleMeta.LEAVE_MESSAGE.id,
        intent: RuleMeta.LEAVE_MESSAGE.intent,
        response: `Please provide your message and our team will get back to you as soon as possible.`,
        priority: Priority.High,
        action: ActionType.DisplayMessage
    },
]


