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

 
  SaveChatUserData(chatUserdata: UserInfoModel) {
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

export interface UserInfoModel {
  name: string;
  phoneNumber: number;
  email: string;
  state: string;
  country: string;
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
