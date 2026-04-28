import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import {  OptionModel } from 'src/model/optionModel';
import { chatRules } from 'src/chatRules/chatbotRule';
import { ChatRule } from 'src/model/chatMessage';

@Injectable({
  providedIn: 'root'
})
export class ChatEngineService {


  constructor(private http: HttpClient) {
  
  }

  detectIntent(ruleIntent: string): ChatRule {
    //find the rule based on the intent
    return chatRules.find(rule => rule.intent === ruleIntent);
  }
}
