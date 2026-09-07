import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import {  OptionModel } from 'src/model/optionModel';

@Injectable({
  providedIn: 'root'
})
export class ChatService {


  constructor(private http: HttpClient) {
  
  }

  apiUrl = environment.apiBaseUrl;
  private widgetHeaders = new HttpHeaders({ 'X-Widget-Token': environment.widgetToken });


  SaveChatUserData(chatUserdata: UserInfoModel) {
    let url = `${this.apiUrl}Chat/RegisterChatUser`;
    return this.http.post(url, chatUserdata);
  }

  LogChatCustomer(model: UserInfoModel) {
    let url = `${this.apiUrl}chatbot/logchatcustomer`;
    return this.http.post<ChatLogResponse>(url, model, { headers: this.widgetHeaders });
  }

  SendChatMessage(request: CustomerChatRequest) {
    let url = `${this.apiUrl}chatbot/chat`;
    return this.http.post<CustomerChatResponse>(url, request, { headers: this.widgetHeaders });
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

export interface UserInfoModel {
  name: string;
  phoneNumber: string;
  email: string;
  state: string;
  country: string;
  isExistingCustomer?: boolean;
  customerId?: string;
  companyId?: string;
}

export interface CustomerChatRequest {
  chatLogId: number;
  message: string;
  functionHint?: string;
  companyId?: string;
}

export interface CustomerChatResponse {
  functionName: string;
  message: string;
  data?: any;
}

export interface ChatLogResponse {
  chatLogId: number;
  isExistingCustomer: boolean;
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
