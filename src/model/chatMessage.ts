import { ActionType, OptionModel, Priority } from "./optionModel";

export interface ChatMessage {
  isIncoming: boolean;
  message: string;
  options?: OptionModel[];
  time?: Date;
}

// Discriminated union type for actionPayload based on ActionType
export type ActionPayload = 
  | { action: ActionType.DisplayMenu; payload: OptionModel[] }
  | { action: ActionType.DisplayMessage; payload: string }
  | { action: ActionType.CollectInput; payload: Record<string, any> }
  | { action: ActionType.CallAPI; payload: Record<string, any> };

export interface ChatRule {
  id: string;
  intent: string;
  action: ActionType;
  actionPayload?: OptionModel[] | string | Record<string, any>;  // Type varies by action
  response: string; 
  priority: Priority; // lower number means higher priority
}