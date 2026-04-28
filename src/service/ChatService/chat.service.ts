import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { OptionActions, OptionModel } from 'src/model/optionModel';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  mainMenu: OptionModel[] = [
    { label: 'Provide product information', action: OptionActions.REQUEST_PRODUCT_INFO, isOptionForExistingCustomer: true },
    { label: 'Attach brochure / provide product link', action: OptionActions.REQUEST_PRODUCT_INFO, isOptionForExistingCustomer: true },
    { label: 'Place an order online or speak to your rep', action: OptionActions.PLACE_ORDER, isOptionForExistingCustomer: false },
    { label: 'Provide contact information for your rep', action: OptionActions.REQUEST_REP_INFO, isOptionForExistingCustomer: true },
    { label: 'Leave a message for MedGyn', action: OptionActions.LEAVE_MESSAGE, isOptionForExistingCustomer: true }
  ];
  

  constructor(private http: HttpClient) {
  
  }

  apiUrl = environment.apiBaseUrl;

 
  SaveChatUserData(chatUserdata: ChatUserModel) {
    let url = `${this.apiUrl}Chat/RegisterChatUser`;
    return this.http.post(url, chatUserdata);
  }
  GetChatUserList() {
    let url = `${this.apiUrl}Chat/GetChatUsers`;
    return this.http.get(url);
  }
  GetUserChatMessageList(ChatRoomId:number) {
    let url = `${this.apiUrl}Chat/GetChatByRoomId/${ChatRoomId}`;
    return this.http.get(url);
  }

  GetChatUserDetailsByChatRoomId(ChatRoomId: number) {
    let url = `${this.apiUrl}Chat/GetChatUserDetailsByChatRoomId/${ChatRoomId}`;
    return this.http.get(url);
  }
  GetDepartmentDDList() {
    let url = `${this.apiUrl}Account/GetDepartmentListDD`;
    return this.http.get(url);
  }
 }


export interface chatMessage {
  message: string;
  chatRoomId: string;
  userId: string;
}
export interface chatMessageFromClient {
  message: string;
  chatRoomId: string;
  userId: string;
  companyId: string;
  departmentId?: string;
}
export interface ChatUserModel {
  name: string;
  phoneNumber: number;
  email: string;
  departmentId?: string;
  companyId: number;
  state?: string;
  country?: string;
  isCurrentCustomer?: boolean;
  customerId?: string;
}
export interface ChatUser {
  chatUserId: string;
  chatUserName: string;
  phoneNumber: number;
  email: string;
  departmentId: string;
  chatRoomId: string;
  unReadMessageCount: number;
}
