import { OptionModel } from "./optionModel";

export interface ChatMessage {
  isIncoming: boolean;
  message: string;
  options?: OptionModel[];
  time?: Date;
}